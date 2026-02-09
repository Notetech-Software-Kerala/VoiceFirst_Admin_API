using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.Auth;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface IAuthService
    {
        Task LoginAsync(
            LoginDto request,
            CancellationToken cancellationToken);

        Task<ApiResponse<object>> ForgotPasswordAsync(
            ForgotPasswordDto request,
            CancellationToken cancellationToken);

        Task<ApiResponse<object>> ResetPasswordAsync(
            ResetPasswordDto request,
            CancellationToken cancellationToken);

        Task ChangePasswordAsync(
            string userId,
            ChangePasswordDto request,
            CancellationToken cancellationToken);

        Task LogoutAsync(
            string userId,
            CancellationToken cancellationToken);
    }
}
