using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Auth;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;
using VoiceFirst_Admin.Utilities.Security;
using Microsoft.AspNetCore.Http;

namespace VoiceFirst_Admin.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepo _authRepo;
        private readonly IConnectionMultiplexer _redis;
        private readonly IConfiguration _configuration;
        private readonly IUserRepo _userRepo;
        private readonly JwtSettings _jwtSettings;

        private static readonly TimeSpan OtpExpiry = TimeSpan.FromMinutes(5);
        private const string RedisKeyPrefix = "pwd_reset:";
        private const string RateLimitKeyPrefix = "pwd_reset_count:";
        private const string OtpAttemptKeyPrefix = "otp_attempts:";
        private const string CooldownKeyPrefix = "pwd_reset_lock:";
        private const string RefreshTokenKeyPrefix = "refresh_token:";
        private const string ActiveSessionKeyPrefix = "active_session:";
        private const int MaxForgotPasswordPerDay = 3;
        private const int MaxOtpAttempts = 5;
        private static readonly TimeSpan RateLimitExpiry = TimeSpan.FromHours(24);
        private static readonly TimeSpan CooldownExpiry = TimeSpan.FromSeconds(30);


        public AuthService(
            IAuthRepo authRepo,
            IConnectionMultiplexer redis,
            IConfiguration configuration,
            IUserRepo userRepo,
            JwtSettings jwtSettings)
        {
            _authRepo = authRepo;
            _redis = redis;
            _configuration = configuration;
            _userRepo = userRepo;
            _jwtSettings = jwtSettings;
        }


        public async Task<ApiResponse<LoginResultDto>> LoginAsync(
            LoginRequestDto request,
            CancellationToken cancellationToken)
        {
            // 1. Validate device info     
            if (request.Device == null)
            {
                return ApiResponse<LoginResultDto>.Fail(
                    Messages.DeviceInfoRequired,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.DeviceInfoRequired);
            }

            // 2. Fetch user (including deleted/inactive for proper error messages)
            var user = await _authRepo.GetUserForLoginAsync(
                request.Email, cancellationToken);

            if (user is null)
            {
                return ApiResponse<LoginResultDto>.Fail(
                    Messages.InvalidCredentials,
                    StatusCodes.Status401Unauthorized,
                    ErrorCodes.InvalidCredentials);
            }

            // 3. Check account status
            if (user.IsDeleted == true)
            {
                return ApiResponse<LoginResultDto>.Fail(
                    Messages.AccountDeleted,
                    StatusCodes.Status403Forbidden,
                    ErrorCodes.AccountDeleted);
            }

            if (user.IsActive == false)
            {
                return ApiResponse<LoginResultDto>.Fail(
                    Messages.AccountInactive,
                    StatusCodes.Status403Forbidden,
                    ErrorCodes.AccountInactive);
            }

            // 4. Verify password
            var storedHash = Convert.ToBase64String(user.HashKey);
            var storedSalt = Convert.ToBase64String(user.SaltKey);

            var isPasswordValid = await PasswordHasher.VerifyPasswordAsync(
                request.Password, storedHash, storedSalt);

            if (!isPasswordValid)
            {
                return ApiResponse<LoginResultDto>.Fail(
                    Messages.InvalidCredentials,
                    StatusCodes.Status401Unauthorized,
                    ErrorCodes.InvalidCredentials);
            }

            // 5. Resolve ApplicationVersionId
            var appVersionId = await _authRepo.GetApplicationVersionIdAsync(
                request.Device.Version, cancellationToken);

            if (appVersionId is null)
            {
                return ApiResponse<LoginResultDto>.Fail(
                    Messages.PayloadInvalid,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.ValidationFailed);
            }

            // 6. Upsert device
            var device = new UserDevice
            {
                DeviceID = request.Device.DeviceID,
                ApplicationVersionId = appVersionId.Value,
                DeviceName = request.Device.DeviceName,
                DeviceType = request.Device.DeviceType,
                OS = request.Device.OS,
                OSVersion = request.Device.OSVersion,
                Manufacturer = request.Device.Manufacturer,
                Model = request.Device.Model
            };

            var userDeviceId = await _authRepo.UpsertDeviceAsync(
                device, cancellationToken);

            // 7. Create login session
            var sessionId = await _authRepo.CreateSessionAsync(
                user.UserId, userDeviceId, cancellationToken);

            // 8. Fetch active roles
            var roles = await _authRepo.GetActiveRolesByUserIdAsync(
                user.UserId, cancellationToken);

            // 9. Generate JWT tokens
            var claims = BuildClaims(user, sessionId, userDeviceId, roles);
            var tokenPair = JwtTokenHelper.CreateTokenPair(claims, _jwtSettings);

            // 10. Store refresh token hash in Redis
            await StoreRefreshTokenAsync(user.UserId, sessionId, tokenPair.RefreshToken);

            // 11. Mark session as active in Redis (TTL = access token lifetime)
            await ActivateSessionAsync(user.UserId, sessionId);

            // 10. Build response (refresh token NOT in JSON — controller sets it as cookie)
            var result = new LoginResultDto
            {
                Response = new LoginResponseDto
                {
                    AccessToken = tokenPair.AccessToken,
                    AccessTokenExpiresAtUtc = tokenPair.AccessTokenExpiresAtUtc
                },
                RefreshToken = tokenPair.RefreshToken,
                RefreshTokenExpiresAtUtc = tokenPair.RefreshTokenExpiresAtUtc
            };

            return ApiResponse<LoginResultDto>.Ok(
                result,
                Messages.LoginSuccess,
                StatusCodes.Status200OK);
        }


        public async Task<ApiResponse<LoginResultDto>> RefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken)
        {
            // 1. Validate refresh token signature + expiry + tokenType
            ClaimsPrincipal principal;
            try
            {
                principal = JwtTokenHelper.ValidateRefreshToken(refreshToken, _jwtSettings);
            }
            catch
            {
                return ApiResponse<LoginResultDto>.Fail(
                    Messages.InvalidOrExpiredToken,
                    StatusCodes.Status401Unauthorized,
                    ErrorCodes.InvalidOrExpiredToken);
            }
            
            var userIdStr = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var sessionIdStr = principal.FindFirst("sessionId")?.Value;


            if (string.IsNullOrWhiteSpace(userIdStr) || string.IsNullOrWhiteSpace(sessionIdStr))
            {
                return ApiResponse<LoginResultDto>.Fail(
                    Messages.InvalidOrExpiredToken,
                    StatusCodes.Status401Unauthorized,
                    ErrorCodes.InvalidOrExpiredToken);
            }

            var userId = int.Parse(userIdStr);
            var sessionId = int.Parse(sessionIdStr);

            // 2. Verify refresh token hash in Redis
            var db = _redis.GetDatabase();
            var refreshKey = $"{RefreshTokenKeyPrefix}{userId}:{sessionId}";
            var storedHash = await db.StringGetAsync(refreshKey);

            if (storedHash.IsNullOrEmpty)
            {
                return ApiResponse<LoginResultDto>.Fail(
                    Messages.InvalidOrExpiredToken,
                    StatusCodes.Status401Unauthorized,
                    ErrorCodes.InvalidOrExpiredToken);
            }

            var incomingHash = HashToken(refreshToken);
            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(storedHash!),
                    Encoding.UTF8.GetBytes(incomingHash)))
            {
                // Token mismatch — possible token theft, invalidate session
                await db.KeyDeleteAsync(refreshKey);
                await _authRepo.InvalidateSessionAsync(sessionId, cancellationToken);

                return ApiResponse<LoginResultDto>.Fail(
                    Messages.InvalidOrExpiredToken,
                    StatusCodes.Status401Unauthorized,
                    ErrorCodes.InvalidOrExpiredToken);
            }

            // 3. Fetch user to ensure still active
            var user = await _authRepo.GetUserForLoginAsync(
                principal.FindFirst(ClaimTypes.Email)?.Value ?? "",
                cancellationToken);

            if (user is null || user.IsDeleted == true || user.IsActive == false)
            {
                await db.KeyDeleteAsync(refreshKey);
                return ApiResponse<LoginResultDto>.Fail(
                    Messages.InvalidOrExpiredToken,
                    StatusCodes.Status401Unauthorized,
                    ErrorCodes.InvalidOrExpiredToken);
            }

            // 4. Rotate: generate new token pair
            var deviceIdStr = principal.FindFirst("deviceId")?.Value ?? "0";
            var deviceId = int.Parse(deviceIdStr);

            var roles = await _authRepo.GetActiveRolesByUserIdAsync(
                userId, cancellationToken);

            var claims = BuildClaims(user, sessionId, deviceId, roles);
            var tokenPair = JwtTokenHelper.CreateTokenPair(claims, _jwtSettings);

            // 5. Replace old refresh token hash in Redis (rotation)
            await StoreRefreshTokenAsync(userId, sessionId, tokenPair.RefreshToken);

            // 6. Re-activate session in Redis
            await ActivateSessionAsync(userId, sessionId);

            var result = new LoginResultDto
            {
                Response = new LoginResponseDto
                {
                    AccessToken = tokenPair.AccessToken,
                    AccessTokenExpiresAtUtc = tokenPair.AccessTokenExpiresAtUtc
                },
                RefreshToken = tokenPair.RefreshToken,
                RefreshTokenExpiresAtUtc = tokenPair.RefreshTokenExpiresAtUtc
            };

            return ApiResponse<LoginResultDto>.Ok(
                result,
                Messages.Success,
                StatusCodes.Status200OK);
        }


        public async Task<ApiResponse<object>> LogoutAsync(
            int userId,
            int sessionId,
            CancellationToken cancellationToken)
        {
            // 1. Invalidate DB session
            await _authRepo.InvalidateSessionAsync(sessionId, cancellationToken);

            // 2. Remove refresh token and active session from Redis
            var db = _redis.GetDatabase();
            var refreshKey = $"{RefreshTokenKeyPrefix}{userId}:{sessionId}";
            var sessionKey = $"{ActiveSessionKeyPrefix}{userId}:{sessionId}";
            await db.KeyDeleteAsync(refreshKey);
            await db.KeyDeleteAsync(sessionKey);

            return ApiResponse<object>.Ok(
                null!,
                Messages.LogoutSuccess,
                StatusCodes.Status200OK);
        }



        public async Task<ApiResponse<object>> ForgotPasswordAsync(
            ForgotPasswordDto request,
            CancellationToken cancellationToken)
        {
            var db = _redis.GetDatabase();
            var email = request.Email!;

            // 1. Rate limit: max 3 requests per day per email (uses request.Email to prevent enumeration)
            var rateLimitKey = $"{RateLimitKeyPrefix}{email}";
            var currentCount = await db.StringGetAsync(rateLimitKey);

            if (currentCount.HasValue && (int)currentCount >= MaxForgotPasswordPerDay)
            {
                return ApiResponse<object>.Fail(
                    Messages.ForgotPasswordLimitExceeded,
                    StatusCodes.Status429TooManyRequests,
                    ErrorCodes.ForgotPasswordLimitExceeded);
            }

            // 2. Distributed lock: prevent OTP spam (1 OTP per 30 seconds)
            var cooldownKey = $"{CooldownKeyPrefix}{email}";
            var lockAcquired = await db.StringSetAsync(cooldownKey, "1", CooldownExpiry, When.NotExists);

            if (!lockAcquired)
            {
                return ApiResponse<object>.Fail(
                    Messages.ForgotPasswordCooldown,
                    StatusCodes.Status429TooManyRequests,
                    ErrorCodes.ForgotPasswordCooldown);
            }

            // Increment rate limit counter (set 24h expiry on first request)
            var newCount = await db.StringIncrementAsync(rateLimitKey);
            if (newCount == 1)
            {
                await db.KeyExpireAsync(rateLimitKey, RateLimitExpiry);
            }

            var user = await _userRepo.GetUserByEmailAsync(email, cancellationToken);

            if (user is null)
            {
                // Return success to prevent email enumeration
                return ApiResponse<object>.Ok(
                    null!,
                    Messages.ForgotPasswordEmailSent,
                    StatusCodes.Status200OK);
            }

            // 3. Generate 8-digit OTP
            var otp = RandomNumberGenerator.GetInt32(10000000, 99999999).ToString();

            // 4. Hash OTP before storing in Redis
            var otpHash = HashOtp(otp);

            var redisKey = $"{RedisKeyPrefix}{email}";
            await db.StringSetAsync(redisKey, otpHash, OtpExpiry);

            // Reset OTP attempt counter for this email
            var attemptKey = $"{OtpAttemptKeyPrefix}{email}";
            await db.KeyDeleteAsync(attemptKey);

            // Send OTP via email
            var emailDto = new EmailDTO
            {
                from_email = _configuration["Email:FromEmail"]!,
                from_email_password = _configuration["Email:FromPassword"],
                to_email = user.Email,
                email_subject = "Password Reset OTP",
                email_html_body = $@"
                    <h2>Password Reset</h2>
                    <p>Hi {user.FirstName},</p>
                    <p>Your password reset OTP is:</p>
                    <h1 style='letter-spacing:8px;font-weight:bold;'>{otp}</h1>
                    <p>This OTP will expire in 60 seconds.</p>
                    <p>If you did not request this, please ignore this email.</p>"
            };

            EmailHelper.SendMail(emailDto);

            return ApiResponse<object>.Ok(
                null!,
                Messages.ForgotPasswordEmailSent,
                StatusCodes.Status200OK);
        }





        public async Task<ApiResponse<object>> ResetPasswordAsync(
            ResetPasswordDto request,
            CancellationToken cancellationToken)
        {
            var db = _redis.GetDatabase();
            var email = request.Email;

            // 5. Check OTP attempt limit (max 5 attempts)
            var attemptKey = $"{OtpAttemptKeyPrefix}{email}";
            var attempts = await db.StringGetAsync(attemptKey);

            if (attempts.HasValue && (int)attempts >= MaxOtpAttempts)
            {
                // Lockout: delete OTP so attacker can't keep trying
                var otpKey = $"{RedisKeyPrefix}{email}";
                await db.KeyDeleteAsync(otpKey);
                await db.KeyDeleteAsync(attemptKey);

                return ApiResponse<object>.Fail(
                    Messages.OtpAttemptsExceeded,
                    StatusCodes.Status429TooManyRequests,
                    ErrorCodes.OtpAttemptsExceeded);
            }

            // Validate hashed OTP from Redis
            var redisKey = $"{RedisKeyPrefix}{email}";
            var storedOtpHash = await db.StringGetAsync(redisKey);

            var incomingOtpHash = HashOtp(request.Otp);

            if (storedOtpHash.IsNullOrEmpty
                || !CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(storedOtpHash!),
                    Encoding.UTF8.GetBytes(incomingOtpHash)))
            {
                // Increment attempt counter
                var newAttempts = await db.StringIncrementAsync(attemptKey);
                if (newAttempts == 1)
                {
                    await db.KeyExpireAsync(attemptKey, OtpExpiry);
                }

                return ApiResponse<object>.Fail(
                    Messages.InvalidOrExpiredToken,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.InvalidOrExpiredToken);
            }

            var user = await _userRepo.GetUserByEmailAsync(email, cancellationToken);

            if (user is null)
            {
                return ApiResponse<object>.Fail(
                    Messages.InvalidOrExpiredToken,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.InvalidOrExpiredToken);
            }

            // Hash the new password
            var hashResult = await PasswordHasher.HashPasswordAsync(request.NewPassword);

            var hashKey = Convert.FromBase64String(hashResult.Hash);
            var saltKey = Convert.FromBase64String(hashResult.Salt);

            // Update password in database
            var updated = await _authRepo.UpdatePasswordAsync(
                user.UserId, hashKey, saltKey, cancellationToken);

            if (!updated)
            {
                return ApiResponse<object>.Fail(
                    Messages.PasswordResetFailed,
                    StatusCodes.Status500InternalServerError,
                    ErrorCodes.PasswordResetFailed);
            }

            // Cleanup: remove OTP and attempt counter from Redis
            await db.KeyDeleteAsync(redisKey);
            await db.KeyDeleteAsync(attemptKey);

            // Invalidate all active sessions for this user (force re-login)
            await _authRepo.InvalidateAllSessionsAsync(user.UserId, cancellationToken);
            await InvalidateAllUserSessionsAsync(user.UserId);

            return ApiResponse<object>.Ok(
                null!,
                Messages.ResetPasswordSuccess,
                StatusCodes.Status200OK);
        }

        private static string HashOtp(string otp)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(otp));
            return Convert.ToBase64String(bytes);
        }

        private static string HashToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

        private static Dictionary<string, object?> BuildClaims(
            Users user, int sessionId, int deviceId, IEnumerable<string> roles)
        {
            return new Dictionary<string, object?>
            {
                [JwtRegisteredClaimNames.Sub] = user.UserId.ToString(),
                [JwtRegisteredClaimNames.Email] = user.Email,
                ["firstName"] = user.FirstName,
                ["lastName"] = user.LastName,
                ["sessionId"] = sessionId.ToString(),
                ["deviceId"] = deviceId.ToString(),
                ["roles"] = roles
            };
        }

        private async Task StoreRefreshTokenAsync(
            int userId, int sessionId, string refreshToken)
        {
            var db = _redis.GetDatabase();
            var hash = HashToken(refreshToken);
            var key = $"{RefreshTokenKeyPrefix}{userId}:{sessionId}";
            await db.StringSetAsync(
                key, hash, TimeSpan.FromDays(_jwtSettings.RefreshTokenExpiryDays));
        }

        private async Task ActivateSessionAsync(int userId, int sessionId)
        {
            var db = _redis.GetDatabase();
            var key = $"{ActiveSessionKeyPrefix}{userId}:{sessionId}";
            await db.StringSetAsync(key, "1",
                TimeSpan.FromMinutes(_jwtSettings.AccessTokenExpiryMinutes + 1));
        }

        private async Task InvalidateAllUserSessionsAsync(int userId)
        {
            var db = _redis.GetDatabase();
            var server = _redis.GetServers().First();
            var pattern = $"{ActiveSessionKeyPrefix}{userId}:*";

            await foreach (var key in server.KeysAsync(pattern: pattern))
            {
                await db.KeyDeleteAsync(key);
            }

            var refreshPattern = $"{RefreshTokenKeyPrefix}{userId}:*";
            await foreach (var key in server.KeysAsync(pattern: refreshPattern))
            {
                await db.KeyDeleteAsync(key);
            }
        }


        public async Task<ApiResponse<object>> ChangePasswordAsync(
           int userId,
           int sessionId,
           ChangePasswordDto request,
           CancellationToken cancellationToken)
        {
            // 1. Fetch user by ID
            var user = await _authRepo.GetUserByIdForAuthAsync(userId, cancellationToken);

            if (user is null)
            {
                return ApiResponse<object>.Fail(
                    Messages.UserNotFound,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.UserNotFound);
            }

            // 2. Verify old password
            var storedHash = Convert.ToBase64String(user.HashKey);
            var storedSalt = Convert.ToBase64String(user.SaltKey);

            var isOldPasswordValid = await PasswordHasher.VerifyPasswordAsync(
                request.OldPassword, storedHash, storedSalt);

            if (!isOldPasswordValid)
            {
                return ApiResponse<object>.Fail(
                    Messages.OldPasswordIncorrect,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.OldPasswordIncorrect);
            }

            // 3. Ensure new password is different from old
            var isNewSameAsOld = await PasswordHasher.VerifyPasswordAsync(
                request.NewPassword, storedHash, storedSalt);

            if (isNewSameAsOld)
            {
                return ApiResponse<object>.Fail(
                    Messages.NewPasswordSameAsOld,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.NewPasswordSameAsOld);
            }

            // 4. Hash new password
            var hashResult = await PasswordHasher.HashPasswordAsync(request.NewPassword);
            var hashKey = Convert.FromBase64String(hashResult.Hash);
            var saltKey = Convert.FromBase64String(hashResult.Salt);

            // 5. Update password in DB
            var updated = await _authRepo.UpdatePasswordAsync(
                userId, hashKey, saltKey, cancellationToken);

            if (!updated)
            {
                return ApiResponse<object>.Fail(
                    Messages.ChangePasswordFailed,
                    StatusCodes.Status500InternalServerError,
                    ErrorCodes.ChangePasswordFailed);
            }

            // 6. Invalidate ALL other sessions (force re-login on other devices)
            //    but keep the current session active
            await _authRepo.InvalidateAllSessionsAsync(userId, cancellationToken);
            await InvalidateAllUserSessionsAsync(userId);

            // 7. Re-activate ONLY the current session so user stays logged in on this device
            await ActivateSessionAsync(userId, sessionId);

            return ApiResponse<object>.Ok(
                null!,
                Messages.ChangePasswordSuccess,
                StatusCodes.Status200OK);
        }
    }
}
