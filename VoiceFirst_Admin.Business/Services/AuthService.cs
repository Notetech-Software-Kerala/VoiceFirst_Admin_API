using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Auth;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Security;
using Microsoft.AspNetCore.Http;

namespace VoiceFirst_Admin.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepo _authRepo;
        private readonly IConnectionMultiplexer _redis;
        private readonly IConfiguration _configuration;

        private static readonly TimeSpan TokenExpiry = TimeSpan.FromMinutes(15);
        private const string RedisKeyPrefix = "pwd_reset:";

        public AuthService(
            IAuthRepo authRepo,
            IConnectionMultiplexer redis,
            IConfiguration configuration)
        {
            _authRepo = authRepo;
            _redis = redis;
            _configuration = configuration;
        }

        public Task ChangePasswordAsync(
            string userId,
            ChangePasswordDto request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<object>> ForgotPasswordAsync(
            ForgotPasswordDto request,
            CancellationToken cancellationToken)
        {
            var user = await _authRepo.GetUserByEmailAsync(
                request.Email!, cancellationToken);

            if (user is null)
            {
                // Return success even if user not found to prevent email enumeration
                return ApiResponse<object>.Ok(
                    null!,
                    Messages.ForgotPasswordEmailSent,
                    StatusCodes.Status200OK);
            }

            // Generate a cryptographically secure token
            var tokenBytes = RandomNumberGenerator.GetBytes(32);
            var token = Convert.ToBase64String(tokenBytes);

            // Store token in Redis with expiry
            var db = _redis.GetDatabase();
            var redisKey = $"{RedisKeyPrefix}{token}";
            await db.StringSetAsync(redisKey, user.UserId.ToString(), TokenExpiry);

            // Build reset link
            var baseUrl = _configuration["Email:ResetPasswordBaseUrl"];
            var resetLink = $"{baseUrl}?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

            // Send email
            var emailDto = new EmailDTO
            {
                from_email = _configuration["Email:FromEmail"]!,
                from_email_password = _configuration["Email:FromPassword"],
                to_email = user.Email,
                email_subject = "Password Reset Request",
                email_html_body = $@"
                    <h2>Password Reset</h2>
                    <p>Hi {user.FirstName},</p>
                    <p>You requested a password reset. Click the link below to reset your password:</p>
                    <p><a href='{resetLink}'>Reset Password</a></p>
                    <p>This link will expire in 15 minutes.</p>
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
            // Validate token from Redis
            var db = _redis.GetDatabase();
            var redisKey = $"{RedisKeyPrefix}{request.Token}";
            var storedUserId = await db.StringGetAsync(redisKey);

            if (storedUserId.IsNullOrEmpty)
            {
                return ApiResponse<object>.Fail(
                    Messages.InvalidOrExpiredToken,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.InvalidOrExpiredToken);
            }

            // Verify the email matches
            var userId = int.Parse(storedUserId!);
            var user = await _authRepo.GetUserByEmailAsync(
                request.Email, cancellationToken);

            if (user is null || user.UserId != userId)
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
                userId, hashKey, saltKey, cancellationToken);

            if (!updated)
            {
                return ApiResponse<object>.Fail(
                    Messages.PasswordResetFailed,
                    StatusCodes.Status500InternalServerError,
                    ErrorCodes.PasswordResetFailed);
            }

            // Remove the token from Redis so it cannot be reused
            await db.KeyDeleteAsync(redisKey);

            return ApiResponse<object>.Ok(
                null!,
                Messages.ResetPasswordSuccess,
                StatusCodes.Status200OK);
        }

        public Task LoginAsync(
            LoginDto request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task LogoutAsync(
            string userId,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
