using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Auth;
using VoiceFirst_Admin.Utilities.Enums;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;
using VoiceFirst_Admin.Utilities.Security;

namespace VoiceFirst_Admin.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepo _authRepo;
        private readonly ISessionService _sessionService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IAuthRepo authRepo,
            ISessionService sessionService,
            JwtSettings jwtSettings)
        {
            _authRepo = authRepo;
            _sessionService = sessionService;
            _jwtSettings = jwtSettings;
        }

        public async Task<ApiResponse<LoginResultDto>> LoginAsync(
            LoginRequestDto request,
            string fingerprint,
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

            // 4. Check login lockout
            if (await _sessionService.IsLoginLockedOutAsync(request.Email))
            {
                var minutesLeft = await _sessionService.GetLockoutMinutesRemainingAsync(request.Email);
                return ApiResponse<LoginResultDto>.Fail(
                    string.Format(Messages.AccountLockedOut, minutesLeft),
                    StatusCodes.Status429TooManyRequests,
                    ErrorCodes.AccountLockedOut);
            }

            // 5. Verify password
            var storedHash = Convert.ToBase64String(user.HashKey);
            var storedSalt = Convert.ToBase64String(user.SaltKey);

            var isPasswordValid = await PasswordHasher.VerifyPasswordAsync(
                request.Password, storedHash, storedSalt);

            if (!isPasswordValid)
            {
                var (lockedOut, remaining) = await _sessionService.RecordFailedLoginAsync(request.Email);

                if (lockedOut)
                {
                    var minutesLeft = await _sessionService.GetLockoutMinutesRemainingAsync(request.Email);
                    return ApiResponse<LoginResultDto>.Fail(
                        string.Format(Messages.AccountLockedOut, minutesLeft),
                        StatusCodes.Status429TooManyRequests,
                        ErrorCodes.AccountLockedOut);
                }

                return ApiResponse<LoginResultDto>.Fail(
                    string.Format(Messages.InvalidCredentialsWithAttempts, remaining),
                    StatusCodes.Status401Unauthorized,
                    ErrorCodes.InvalidCredentials);
            }

            // 6. Login success — clear failed attempt counter
            await _sessionService.ClearLoginAttemptsAsync(request.Email);

            // 7. Resolve ApplicationVersionId
            var appVersionId = await _authRepo.GetApplicationVersionIdAsync(
                request.Device.Version, cancellationToken);

            if (appVersionId is null)
            {
                return ApiResponse<LoginResultDto>.Fail(
                    Messages.PayloadInvalid,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.ValidationFailed);
            }

            // 8. Upsert device
            var device = new UserDevice
            {
                DeviceID = request.Device.DeviceID,
                ApplicationVersionId = appVersionId.Value,
                DeviceName = request.Device.DeviceName,
                DeviceType = request.Device.DeviceType,
                OS = request.Device.OS,
                OSVersion = request.Device.OSVersion,
                Manufacturer = request.Device.Manufacturer,
                Model = request.Device.Model,
                ClientType = (int)request.ClientType
            };

            var deviceResult = await _authRepo.UpsertDeviceAsync(
                device, cancellationToken);

            var userDeviceId = deviceResult.UserDeviceId;

            // Use the stored ClientType from DB — not the request value.
            // First registration locks the device type; subsequent logins use the stored one.
            var clientType = Enum.IsDefined(typeof(ClientType), deviceResult.ClientType)
                ? (ClientType)deviceResult.ClientType
                : ClientType.Web;

            // 9. Create login session
            var sessionId = await _authRepo.CreateSessionAsync(
                user.UserId, userDeviceId, cancellationToken);

            // 10. Fetch active roles
            var roles = await _authRepo.GetActiveRolesByUserIdAsync(
                user.UserId, cancellationToken);

            // 11. Initialize token version in Redis
            var tokenVersion = await _sessionService.InitTokenVersionAsync(user.UserId, sessionId);

            // 12. Generate JWT tokens
            var claims = BuildClaims(user, sessionId, userDeviceId, roles, tokenVersion, clientType);
            var tokenPair = _sessionService.GenerateTokens(claims);

            // 13. Store refresh token hash in Redis
            await _sessionService.StoreRefreshTokenAsync(user.UserId, sessionId, tokenPair.RefreshToken);

            // 14. Mark session as active in Redis
            await _sessionService.ActivateSessionAsync(user.UserId, sessionId, userDeviceId, fingerprint);

            // 15. Build response
            var result = new LoginResultDto
            {
                Response = new LoginResponseDto
                {
                    AccessToken = tokenPair.AccessToken,
                    AccessTokenExpiresAtUtc = tokenPair.AccessTokenExpiresAtUtc
                },
                RefreshToken = tokenPair.RefreshToken,
                RefreshTokenExpiresAtUtc = tokenPair.RefreshTokenExpiresAtUtc,
                ClientType = clientType
            };

            return ApiResponse<LoginResultDto>.Ok(
                result,
                Messages.LoginSuccess,
                StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<LoginResultDto>> RefreshTokenAsync(
            string refreshToken,
            string fingerprint,
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
            var isValid = await _sessionService.VerifyRefreshTokenAsync(userId, sessionId, refreshToken);

            if (!isValid)
            {
                // Token mismatch or missing — possible token theft, invalidate session
                await _sessionService.DeleteRefreshTokenAsync(userId, sessionId);
                await _authRepo.InvalidateSessionAsync(sessionId, cancellationToken);

                return ApiResponse<LoginResultDto>.Fail(
                    Messages.InvalidOrExpiredToken,
                    StatusCodes.Status401Unauthorized,
                    ErrorCodes.InvalidOrExpiredToken);
            }

            // 2b. Verify the refresh token is being used from the same device
            var deviceIdStr = principal.FindFirst("deviceId")?.Value ?? "0";
            var deviceId = int.Parse(deviceIdStr);

            var deviceMatch = await _sessionService.VerifySessionDeviceAsync(userId, sessionId, deviceId);
            if (!deviceMatch)
            {
                // Device mismatch — stolen token used from a different device
                await _sessionService.InvalidateSessionKeysAsync(userId, sessionId);
                await _authRepo.InvalidateSessionAsync(sessionId, cancellationToken);

                return ApiResponse<LoginResultDto>.Fail(
                    Messages.InvalidOrExpiredToken,
                    StatusCodes.Status401Unauthorized,
                    ErrorCodes.InvalidOrExpiredToken);
            }

            // 2c. Verify browser/client fingerprint matches the login origin
            var fingerprintMatch = await _sessionService.VerifyFingerprintAsync(userId, sessionId, fingerprint);
            if (!fingerprintMatch)
            {
                // Fingerprint mismatch — token used from a different browser/client
                await _sessionService.InvalidateSessionKeysAsync(userId, sessionId);
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
                await _sessionService.DeleteRefreshTokenAsync(userId, sessionId);
                return ApiResponse<LoginResultDto>.Fail(
                    Messages.InvalidOrExpiredToken,
                    StatusCodes.Status401Unauthorized,
                    ErrorCodes.InvalidOrExpiredToken);
            }

            // 4. Rotate: generate new token pair
            var clientTypeStr = principal.FindFirst("clientType")?.Value;
            var clientType = Enum.TryParse<ClientType>(clientTypeStr, true, out var ct)
                ? ct
                : ClientType.Web;

            var roles = await _authRepo.GetActiveRolesByUserIdAsync(
                userId, cancellationToken);

            // 4b. Increment token version to invalidate old access tokens
            var tokenVersion = await _sessionService.IncrementTokenVersionAsync(userId, sessionId);

            var claims = BuildClaims(user, sessionId, deviceId, roles, tokenVersion, clientType);
            var tokenPair = _sessionService.GenerateTokens(claims);

            // 5. Replace old refresh token hash in Redis (rotation)
            await _sessionService.StoreRefreshTokenAsync(userId, sessionId, tokenPair.RefreshToken);

            // 6. Re-activate session in Redis
            await _sessionService.ActivateSessionAsync(userId, sessionId, deviceId, fingerprint);

            var result = new LoginResultDto
            {
                Response = new LoginResponseDto
                {
                    AccessToken = tokenPair.AccessToken,
                    AccessTokenExpiresAtUtc = tokenPair.AccessTokenExpiresAtUtc
                },
                RefreshToken = tokenPair.RefreshToken,
                RefreshTokenExpiresAtUtc = tokenPair.RefreshTokenExpiresAtUtc,
                ClientType = clientType
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

            // 2. Remove refresh token, active session, and token version from Redis
            await _sessionService.InvalidateSessionKeysAsync(userId, sessionId);

            return ApiResponse<object>.Ok(
                null!,
                Messages.LogoutSuccess,
                StatusCodes.Status200OK);
        }

        private static Dictionary<string, object?> BuildClaims(
            Users user, int sessionId, int deviceId, IEnumerable<string> roles, long tokenVersion, ClientType clientType)
        {
            return new Dictionary<string, object?>
            {
                [JwtRegisteredClaimNames.Sub] = user.UserId.ToString(),
                [JwtRegisteredClaimNames.Email] = user.Email,
                ["firstName"] = user.FirstName,
                ["lastName"] = user.LastName,
                ["sessionId"] = sessionId.ToString(),
                ["deviceId"] = deviceId.ToString(),
                ["tokenVer"] = tokenVersion.ToString(),
                ["clientType"] = clientType.ToString(),
                ["roles"] = roles
            };
        }
    }
}
