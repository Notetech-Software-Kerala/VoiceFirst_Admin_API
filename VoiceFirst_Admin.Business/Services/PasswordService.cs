using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Auth;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Security;

namespace VoiceFirst_Admin.Business.Services;

public class PasswordService : IPasswordService
{
    private readonly IAuthRepo _authRepo;
    private readonly IUserRepo _userRepo;
    private readonly ISessionService _sessionService;
    private readonly IConnectionMultiplexer _redis;
    private readonly IConfiguration _configuration;

    private static readonly TimeSpan OtpExpiry = TimeSpan.FromMinutes(5);
    private const string OtpKeyPrefix = "pwd_reset:";
    private const string RateLimitKeyPrefix = "pwd_reset_count:";
    private const string OtpAttemptKeyPrefix = "otp_attempts:";
    private const string CooldownKeyPrefix = "pwd_reset_lock:";
    private const int MaxForgotPasswordPerDay = 3;
    private const int MaxOtpAttempts = 5;
    private static readonly TimeSpan RateLimitExpiry = TimeSpan.FromHours(24);
    private static readonly TimeSpan CooldownExpiry = TimeSpan.FromSeconds(30);

    public PasswordService(
        IAuthRepo authRepo,
        IUserRepo userRepo,
        ISessionService sessionService,
        IConnectionMultiplexer redis,
        IConfiguration configuration)
    {
        _authRepo = authRepo;
        _userRepo = userRepo;
        _sessionService = sessionService;
        _redis = redis;
        _configuration = configuration;
    }

    public async Task<ApiResponse<object>> ForgotPasswordAsync(
        ForgotPasswordDto request,
        CancellationToken cancellationToken)
    {
        var db = _redis.GetDatabase();
        var email = request.Email!;

        // 1. Rate limit: max 3 requests per day per email
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

        var redisKey = $"{OtpKeyPrefix}{email}";
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

        // 1. Check OTP attempt limit (max 5 attempts)
        var attemptKey = $"{OtpAttemptKeyPrefix}{email}";
        var attempts = await db.StringGetAsync(attemptKey);

        if (attempts.HasValue && (int)attempts >= MaxOtpAttempts)
        {
            var otpKey = $"{OtpKeyPrefix}{email}";
            await db.KeyDeleteAsync(otpKey);
            await db.KeyDeleteAsync(attemptKey);

            return ApiResponse<object>.Fail(
                Messages.OtpAttemptsExceeded,
                StatusCodes.Status429TooManyRequests,
                ErrorCodes.OtpAttemptsExceeded);
        }

        // 2. Validate hashed OTP from Redis
        var redisKey = $"{OtpKeyPrefix}{email}";
        var storedOtpHash = await db.StringGetAsync(redisKey);

        var incomingOtpHash = HashOtp(request.Otp);

        if (storedOtpHash.IsNullOrEmpty
            || !CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(storedOtpHash!),
                Encoding.UTF8.GetBytes(incomingOtpHash)))
        {
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

        // 3. Hash the new password
        var hashResult = await PasswordHasher.HashPasswordAsync(request.NewPassword);
        var hashKey = Convert.FromBase64String(hashResult.Hash);
        var saltKey = Convert.FromBase64String(hashResult.Salt);

        // 4. Update password in database
        var updated = await _authRepo.UpdatePasswordAsync(
            user.UserId, hashKey, saltKey, cancellationToken);

        if (!updated)
        {
            return ApiResponse<object>.Fail(
                Messages.PasswordResetFailed,
                StatusCodes.Status500InternalServerError,
                ErrorCodes.PasswordResetFailed);
        }

        // 5. Cleanup: remove OTP and attempt counter from Redis
        await db.KeyDeleteAsync(redisKey);
        await db.KeyDeleteAsync(attemptKey);

        // 6. Invalidate all active sessions for this user (force re-login)
        await _authRepo.InvalidateAllSessionsAsync(user.UserId, cancellationToken);
        await _sessionService.InvalidateAllUserSessionsAsync(user.UserId);

        return ApiResponse<object>.Ok(
            null!,
            Messages.ResetPasswordSuccess,
            StatusCodes.Status200OK);
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
        //    but keep the current session's Redis keys intact
        await _authRepo.InvalidateAllSessionsAsync(userId, cancellationToken);
        await _sessionService.InvalidateAllUserSessionsAsync(userId, excludeSessionId: sessionId);

        return ApiResponse<object>.Ok(
            null!,
            Messages.ChangePasswordSuccess,
            StatusCodes.Status200OK);
    }

    private static string HashOtp(string otp)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(otp));
        return Convert.ToBase64String(bytes);
    }
}
