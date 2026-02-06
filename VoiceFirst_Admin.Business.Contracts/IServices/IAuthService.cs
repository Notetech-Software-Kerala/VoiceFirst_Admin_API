using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.Auth;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface IAuthService
    {
        Task LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken);

        Task ForgotPasswordAsync(
            ForgotPasswordRequest request,
            CancellationToken cancellationToken);

        Task ResetPasswordAsync(
            ResetPasswordRequest request,
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
