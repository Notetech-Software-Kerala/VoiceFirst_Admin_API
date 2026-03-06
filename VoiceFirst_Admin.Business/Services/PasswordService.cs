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

    private static readonly TimeSpan GrantExpiry = TimeSpan.FromMinutes(5);
    private const string RateLimitKeyPrefix = "pwd_reset_count:";
    private const string CooldownKeyPrefix = "pwd_reset_lock:";
    private const string GrantKeyPrefix = "pwd_reset_grant:";
    private const int MaxForgotPasswordRequests = 3;
    private static readonly TimeSpan RateLimitExpiry = TimeSpan.FromMinutes(2);
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

    public async Task<ApiResponse<ForgotPasswordResultDto>> ForgotPasswordAsync(
        ForgotPasswordDto request,
        CancellationToken cancellationToken)
    {
        var db = _redis.GetDatabase();
        var email = request.Email!;

        // 1. Rate limit: max 3 requests per 2 minutes per email
        var rateLimitKey = $"{RateLimitKeyPrefix}{email}";
        var currentCount = await db.StringGetAsync(rateLimitKey);

        if (currentCount.HasValue && (int)currentCount >= MaxForgotPasswordRequests)
        {
            return ApiResponse<ForgotPasswordResultDto>.Fail(
                Messages.ForgotPasswordLimitExceeded,
                StatusCodes.Status429TooManyRequests,
                ErrorCodes.ForgotPasswordLimitExceeded);
        }

        // 2. Distributed lock: prevent OTP spam (1 OTP per 30 seconds)
        var cooldownKey = $"{CooldownKeyPrefix}{email}";
        var lockAcquired = await db.StringSetAsync(cooldownKey, "1", CooldownExpiry, When.NotExists);

        if (!lockAcquired)
        {
            return ApiResponse<ForgotPasswordResultDto>.Fail(
                Messages.ForgotPasswordCooldown,
                StatusCodes.Status429TooManyRequests,
                ErrorCodes.ForgotPasswordCooldown);
        }

        // Increment rate limit counter (set expiry on first request)
        var newCount = await db.StringIncrementAsync(rateLimitKey);
        if (newCount == 1)
        {
            await db.KeyExpireAsync(rateLimitKey, RateLimitExpiry);
        }

        // Generate HMAC-signed reset token (unforgeable, tied to email)
        var resetToken = GenerateGrantToken(email);

        var user = await _userRepo.GetUserByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            // Return success with a dummy token to prevent email enumeration
            return ApiResponse<ForgotPasswordResultDto>.Ok(
                new ForgotPasswordResultDto { ResetToken = resetToken },
                Messages.ForgotPasswordEmailSent,
                StatusCodes.Status200OK);
        }

        // 3. Store hashed token in Redis (single-use, 5-minute expiry)
        var resetTokenHash = HashValue(resetToken);
        var grantKey = $"{GrantKeyPrefix}{email}";
        await db.StringSetAsync(grantKey, resetTokenHash, GrantExpiry);

        // Build reset link
        var resetLink =
            $"{_configuration["Frontend:BaseUrl"]?.TrimEnd('/')}/reset-password?" +
            $"reset-token={Uri.EscapeDataString(resetToken)}";

        // Send password reset link via email
        var emailDto = new EmailDTO
        {
            from_email = _configuration["Email:FromEmail"]!,
            from_email_password = _configuration["Email:FromPassword"],
            to_email = user.Email,
            email_subject = "Reset your password",
            email_html_body = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"" />
  <meta name=""viewport"" content=""width=device-width,initial-scale=1"" />
  <title>Reset your password</title>
</head>
<body style=""margin:0;padding:0;background:#ffffff;font-family:Arial,Helvetica,sans-serif;color:#111827;"">
  <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background:#ffffff;"">
    <tr>
      <td align=""center"" style=""padding:32px 16px;"">
        <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""680"" style=""max-width:680px;width:100%;"">
          <tr>
            <td style=""padding:0 0 18px 0;"">
              <!-- Optional: your logo -->
              <!-- <img src=""http://localhost:8090/logo-vf.png"" alt=""VoiceFirst"" height=""28"" style=""display:block;"" /> -->
            </td>
          </tr>

          <tr>
            <td style=""padding:0 0 12px 0;"">
              <h2 style=""margin:0;font-size:24px;line-height:32px;font-weight:700;color:#111827;"">
                Reset your password
              </h2>
            </td>
          </tr>

          <tr>
            <td style=""padding:0 0 18px 0;font-size:14px;line-height:22px;color:#111827;"">
              <p style=""margin:0 0 10px 0;"">Hi {System.Net.WebUtility.HtmlEncode(user.FirstName)},</p>
              <p style=""margin:0;"">
                Click on the button below within the next <strong>5 minutes</strong> to reset your password for your Account
                <a href=""mailto:{System.Net.WebUtility.HtmlEncode(user.Email)}"" style=""color:#1d4ed8;text-decoration:underline;"">
                  {System.Net.WebUtility.HtmlEncode(user.Email)}
                </a>.
              </p>
            </td>
          </tr>

          <tr>
            <td style=""padding:12px 0 22px 0;"">
              <!-- Button (email-client friendly) -->
              <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                <tr>
                  <td align=""center"" bgcolor=""#e11d48"" style=""border-radius:4px;"">
                    <a href=""{resetLink}""
                       style=""display:inline-block;padding:12px 18px;font-size:14px;font-weight:700;color:#ffffff;text-decoration:none;border-radius:4px;"">
                      Reset your password
                    </a>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <tr>
            <td style=""padding:0 0 16px 0;"">
              <hr style=""border:0;border-top:1px solid #d1d5db;margin:0;"" />
            </td>
          </tr>

          <tr>
            <td style=""font-size:13px;line-height:20px;color:#374151;"">
              <p style=""margin:0 0 10px 0;"">
                If you are having any issues with your account, check out our
                <a href=""{_configuration["Support:DocsUrl"] ?? "#"}"" style=""color:#1d4ed8;text-decoration:underline;"">support docs</a>.
              </p>
              <p style=""margin:0;"">
                If this was a mistake, please ignore this email and nothing will happen.
              </p>
            </td>
          </tr>

          <tr>
            <td style=""padding-top:18px;font-size:12px;line-height:18px;color:#6b7280;"">
              <p style=""margin:0;"">
                If the button doesn’t work, copy and paste this link into your browser:
              </p>
              <p style=""margin:6px 0 0 0;word-break:break-all;"">
                <a href=""{resetLink}"" style=""color:#1d4ed8;text-decoration:underline;"">{resetLink}</a>
              </p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>"
        };

        EmailHelper.SendMail(emailDto);

        return ApiResponse<ForgotPasswordResultDto>.Ok(
            new ForgotPasswordResultDto { ResetToken = resetToken },
            Messages.ForgotPasswordEmailSent,
            StatusCodes.Status200OK);
    }

    public async Task<ApiResponse<object>> ValidateResetTokenAsync(
        ValidateResetTokenDto request,
        CancellationToken cancellationToken)
    {
        var resetToken = request.ResetToken;

        // 1. Extract email and validate HMAC signature in one step
        var email = ExtractEmailFromGrantToken(resetToken);
        if (email is null)
        {
            return ApiResponse<object>.Fail(
                Messages.InvalidOrExpiredResetLink,
                StatusCodes.Status400BadRequest,
                ErrorCodes.InvalidOrExpiredResetLink);
        }

        // 2. Check token hash exists in Redis (not consumed / not expired)
        var db = _redis.GetDatabase();
        var grantKey = $"{GrantKeyPrefix}{email}";
        var storedGrantHash = await db.StringGetAsync(grantKey);
        var incomingGrantHash = HashValue(resetToken);

        if (storedGrantHash.IsNullOrEmpty
            || !CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(storedGrantHash!),
                Encoding.UTF8.GetBytes(incomingGrantHash)))
        {
            return ApiResponse<object>.Fail(
                Messages.InvalidOrExpiredResetLink,
                StatusCodes.Status400BadRequest,
                ErrorCodes.InvalidOrExpiredResetLink);
        }

        return ApiResponse<object>.Ok(
            null!,
            Messages.ResetTokenValid,
            StatusCodes.Status200OK);
    }

    public async Task<ApiResponse<object>> ResetPasswordAsync(
        ResetPasswordDto request,
        CancellationToken cancellationToken)
    {
        var grantToken = request.PasswordResetGrant!;

        // 1. Extract email and validate HMAC integrity in one step
        var email = ExtractEmailFromGrantToken(grantToken);
        if (email is null)
        {
            return ApiResponse<object>.Fail(
                Messages.InvalidResetGrant,
                StatusCodes.Status400BadRequest,
                ErrorCodes.InvalidResetGrant);
        }

        // 2. Validate grant token hash in Redis (ensures single-use + expiry)
        var db = _redis.GetDatabase();
        var grantKey = $"{GrantKeyPrefix}{email}";
        var storedGrantHash = await db.StringGetAsync(grantKey);
        var incomingGrantHash = HashValue(grantToken);

        if (storedGrantHash.IsNullOrEmpty
            || !CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(storedGrantHash!),
                Encoding.UTF8.GetBytes(incomingGrantHash)))
        {
            return ApiResponse<object>.Fail(
                Messages.InvalidResetGrant,
                StatusCodes.Status400BadRequest,
                ErrorCodes.InvalidResetGrant);
        }

        // 3. Consume the grant token (single-use)
        await db.KeyDeleteAsync(grantKey);

        var user = await _userRepo.GetUserByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            return ApiResponse<object>.Fail(
                Messages.InvalidResetGrant,
                StatusCodes.Status400BadRequest,
                ErrorCodes.InvalidResetGrant);
        }

        // 4. Hash the new password
        var hashResult = await PasswordHasher.HashPasswordAsync(request.NewPassword);
        var hashKey = Convert.FromBase64String(hashResult.Hash);
        var saltKey = Convert.FromBase64String(hashResult.Salt);

        // 5. Update password in database
        var updated = await _authRepo.UpdatePasswordAsync(
            user.UserId, hashKey, saltKey, cancellationToken);

        if (!updated)
        {
            return ApiResponse<object>.Fail(
                Messages.PasswordResetFailed,
                StatusCodes.Status500InternalServerError,
                ErrorCodes.PasswordResetFailed);
        }

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

    private static string HashValue(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Generates an HMAC-SHA256 signed token with the email embedded in the payload.
    /// Format: Base64(email|random_hex).Base64(HMAC-SHA256)
    /// </summary>
    private string GenerateGrantToken(string email)
    {
        var random = RandomNumberGenerator.GetBytes(32);
        var payload = $"{email}|{Convert.ToHexString(random)}";
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        var hmacKey = GetHmacKey();
        using var hmac = new HMACSHA256(hmacKey);
        var signature = hmac.ComputeHash(payloadBytes);

        return $"{Convert.ToBase64String(payloadBytes)}.{Convert.ToBase64String(signature)}";
    }

    /// <summary>
    /// Validates the HMAC signature and extracts the email from the token.
    /// Returns null if the token is malformed or the signature is invalid.
    /// </summary>
    private string? ExtractEmailFromGrantToken(string grantToken)
    {
        var parts = grantToken.Split('.');
        if (parts.Length != 2)
            return null;

        byte[] payloadBytes;
        byte[] signature;

        try
        {
            payloadBytes = Convert.FromBase64String(parts[0]);
            signature = Convert.FromBase64String(parts[1]);
        }
        catch (FormatException)
        {
            return null;
        }

        var hmacKey = GetHmacKey();
        using var hmac = new HMACSHA256(hmacKey);
        var expectedSignature = hmac.ComputeHash(payloadBytes);

        if (!CryptographicOperations.FixedTimeEquals(signature, expectedSignature))
            return null;

        var payload = Encoding.UTF8.GetString(payloadBytes);
        var separatorIndex = payload.IndexOf('|');
        if (separatorIndex <= 0)
            return null;

        return payload[..separatorIndex];
    }

    private byte[] GetHmacKey()
    {
        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured.");

        return Encoding.UTF8.GetBytes(key);
    }
}
