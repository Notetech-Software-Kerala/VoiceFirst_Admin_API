using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
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
    private const string UserTokenKeyPrefix = "pwd_reset_user:";
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

    public async Task<ApiResponse<object>> ForgotPasswordAsync(
        ForgotPasswordDto request,
        CancellationToken cancellationToken)
    {
        var db = _redis.GetDatabase();
        var email = request.Email!.Trim().ToLowerInvariant();
        var emailHash = HashValue(email);

        // 1. Rate limit: max 3 requests per 2 minutes per email (atomic increment-first)
        var rateLimitKey = $"{RateLimitKeyPrefix}{emailHash}";
        var newCount = await db.StringIncrementAsync(rateLimitKey);

        if (newCount == 1)
        {
            await db.KeyExpireAsync(rateLimitKey, RateLimitExpiry);
        }

        if (newCount > MaxForgotPasswordRequests)
        {
            return ApiResponse<object>.Fail(
                Messages.ForgotPasswordLimitExceeded,
                StatusCodes.Status429TooManyRequests,
                ErrorCodes.ForgotPasswordLimitExceeded);
        }

        // 2. Distributed lock: prevent OTP spam (1 OTP per 30 seconds)
        var cooldownKey = $"{CooldownKeyPrefix}{emailHash}";
        var lockAcquired = await db.StringSetAsync(cooldownKey, "1", CooldownExpiry, When.NotExists);

        if (!lockAcquired)
        {
            return ApiResponse<object>.Fail(
                Messages.ForgotPasswordCooldown,
                StatusCodes.Status429TooManyRequests,
                ErrorCodes.ForgotPasswordCooldown);
        }

        // 3. Look up user (including deleted/inactive for proper notification)
        var user = await _userRepo.GetUserByEmailUnfilteredAsync(email, cancellationToken);

        if (user is null)
        {
            // Return success to prevent email enumeration
            return ApiResponse<object>.Ok(
                null!,
                Messages.ForgotPasswordEmailSent(email),
                StatusCodes.Status200OK);
        }

        // 3b. If user is suspended (deleted or inactive), send suspension notice instead
        if (user.IsDeleted == true || user.IsActive == false)
        {
            var suspendedTemplate = EmailTemplateHelper.GetTemplate("AccountSuspendedEmail");
            var suspendedBody = suspendedTemplate
                .Replace("{{UserFullName}}", System.Net.WebUtility.HtmlEncode(user.FirstName + " " + user.LastName))
                .Replace("{{UserEmail}}", System.Net.WebUtility.HtmlEncode(user.Email))
                .Replace("{{SupportDocsUrl}}", _configuration["Support:DocsUrl"] ?? "#");

            var suspendedEmailDto = new EmailDTO
            {
                from_email = _configuration["Email:FromEmail"]!,
                from_email_password = _configuration["Email:FromPassword"],
                to_email = user.Email,
                email_subject = "Account Suspended – Password Reset Unavailable",
                email_html_body = suspendedBody
            };

            EmailHelper.SendMail(suspendedEmailDto);

            // Same response as normal flow to prevent email enumeration
            return ApiResponse<object>.Ok(
                null!,
                Messages.AccountSuspendedEmailSent(email),
                StatusCodes.Status200OK);
        }

        // 4. Generate opaque reset token
        var resetToken = GenerateResetToken();
        var tokenHash = HashValue(resetToken);

        // 5. Invalidate any previous active token for this user (only latest token valid)
        var userTokenKey = $"{UserTokenKeyPrefix}{user.UserId}";
        var previousTokenHash = await db.StringGetDeleteAsync(userTokenKey);

        if (!previousTokenHash.IsNullOrEmpty)
        {
            await db.KeyDeleteAsync($"{GrantKeyPrefix}{previousTokenHash}");
        }

        // 6. Store new token with dual-key pattern
        var grantKey = $"{GrantKeyPrefix}{tokenHash}";
        await db.StringSetAsync(grantKey, email, GrantExpiry);
        await db.StringSetAsync(userTokenKey, tokenHash, GrantExpiry);

        // Build reset link (token in path to avoid leaking in logs/analytics/browser history)
        var resetLink =
            $"{_configuration["Frontend:BaseUrl"]?.TrimEnd('/')}/reset-password/" +
            $"{Uri.EscapeDataString(resetToken)}";

  

        // Load email template and populate placeholders
        var template = EmailTemplateHelper.GetTemplate("ResetPasswordEmail");
        var emailBody = template
            .Replace("{{UserFullName}}", System.Net.WebUtility.HtmlEncode(user.FirstName + " " + user.LastName))
            .Replace("{{UserEmail}}", System.Net.WebUtility.HtmlEncode(user.Email))
            .Replace("{{ResetLink}}", resetLink)

            .Replace("{{SupportDocsUrl}}", _configuration["Support:DocsUrl"] ?? "#");

        // Send password reset link via email
        var emailDto = new EmailDTO
        {
            from_email = _configuration["Email:FromEmail"]!,
            from_email_password = _configuration["Email:FromPassword"],
            to_email = user.Email,
            email_subject = "Reset your password",
            email_html_body = emailBody
        };

        EmailHelper.SendMail(emailDto);

        return ApiResponse<object>.Ok(
            null!,
            Messages.ForgotPasswordEmailSent(email),
            StatusCodes.Status200OK);
    }

    public async Task<ApiResponse<object>> ValidateResetTokenAsync(
 
        string resetToken,
        CancellationToken cancellationToken)
    {

        // Look up token in Redis (keyed by hash of token)
        var db = _redis.GetDatabase();
        var grantKey = $"{GrantKeyPrefix}{HashValue(resetToken)}";
        var storedEmail = await db.StringGetAsync(grantKey);

        if (storedEmail.IsNullOrEmpty)
        {
            return ApiResponse<object>.Fail(
                Messages.InvalidOrExpiredResetLink,
                StatusCodes.Status410Gone,
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

        // 1. Atomically retrieve and consume the grant token (single-use, prevents replay)
        var db = _redis.GetDatabase();
        var grantKey = $"{GrantKeyPrefix}{HashValue(grantToken)}";
        var storedEmail = await db.StringGetDeleteAsync(grantKey);

        if (storedEmail.IsNullOrEmpty)
        {
            return ApiResponse<object>.Fail(
                Messages.InvalidResetGrant,
                StatusCodes.Status410Gone,
                ErrorCodes.InvalidResetGrant);
        }

        var email = (string)storedEmail!;
        var user = await _userRepo.GetUserByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            return ApiResponse<object>.Fail(
                Messages.InvalidResetGrant,
                StatusCodes.Status400BadRequest,
                ErrorCodes.InvalidResetGrant);
        }

        // 2. Hash the new password
        var hashResult = await PasswordHasher.HashPasswordAsync(request.NewPassword);
        var hashKey = Convert.FromBase64String(hashResult.Hash);
        var saltKey = Convert.FromBase64String(hashResult.Salt);

        // 3. Update password in database
        var updated = await _authRepo.UpdatePasswordAsync(
            user.UserId, hashKey, saltKey, cancellationToken);

        if (!updated)
        {
            return ApiResponse<object>.Fail(
                Messages.PasswordResetFailed,
                StatusCodes.Status500InternalServerError,
                ErrorCodes.PasswordResetFailed);
        }

        // 4. Clean up the per-user token tracking key
        var userTokenKey = $"{UserTokenKeyPrefix}{user.UserId}";
        await db.KeyDeleteAsync(userTokenKey);

        // 5. Invalidate all active sessions for this user (force re-login)
        await _authRepo.InvalidateAllSessionsAsync(user.UserId, cancellationToken);
        await _sessionService.InvalidateAllUserSessionsAsync(user.UserId);

        // 6. Send password changed notification email
        SendPasswordChangedNotification(user.FirstName, user.LastName, user.Email);

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

        // 7. Send password changed notification email
        SendPasswordChangedNotification(user.FirstName, user.LastName, user.Email);

        return ApiResponse<object>.Ok(
            null!,
            Messages.ChangePasswordSuccess,
            StatusCodes.Status200OK);
    }

    private void SendPasswordChangedNotification(string firstName, string lastName, string email)
    {
        var template = EmailTemplateHelper.GetTemplate("PasswordChangedNotificationEmail");
        var body = template
            .Replace("{{UserFullName}}", System.Net.WebUtility.HtmlEncode(firstName + " " + lastName))
            .Replace("{{UserEmail}}", System.Net.WebUtility.HtmlEncode(email))
            .Replace("{{ChangedAtUtc}}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm UTC"))
            .Replace("{{SupportDocsUrl}}", _configuration["Support:DocsUrl"] ?? "#");

        var emailDto = new EmailDTO
        {
            from_email = _configuration["Email:FromEmail"]!,
            from_email_password = _configuration["Email:FromPassword"],
            to_email = email,
            email_subject = "Your password was changed",
            email_html_body = body
        };

        EmailHelper.SendMail(emailDto);
    }

    private static string HashValue(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Generates a cryptographically random, opaque reset token.
    /// The email is stored server-side in Redis, never embedded in the token.
    /// </summary>
    private static string GenerateResetToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return WebEncoders.Base64UrlEncode(bytes);
    }
}
