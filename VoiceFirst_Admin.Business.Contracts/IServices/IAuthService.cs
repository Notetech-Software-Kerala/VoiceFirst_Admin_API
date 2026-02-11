using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.Auth;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResultDto>> LoginAsync(
            LoginRequestDto request,
            CancellationToken cancellationToken);

        Task<ApiResponse<LoginResultDto>> RefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken);

        Task<ApiResponse<object>> ForgotPasswordAsync(
            ForgotPasswordDto request,
            CancellationToken cancellationToken);

        Task<ApiResponse<object>> ResetPasswordAsync(
            ResetPasswordDto request,
            CancellationToken cancellationToken);

        Task<ApiResponse<object>> ChangePasswordAsync(
            int userId,
            int sessionId,
            ChangePasswordDto request,
            CancellationToken cancellationToken);

        Task<ApiResponse<object>> LogoutAsync(
            int userId,
            int sessionId,
            CancellationToken cancellationToken);
    }
}
