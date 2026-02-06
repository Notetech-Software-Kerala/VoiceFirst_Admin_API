using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.DTOs.Features.Auth;

namespace VoiceFirst_Admin.Business.Services
{
    public class AuthService : IAuthService
    {
        public  Task ChangePasswordAsync
            (string userId, 
            ChangePasswordDto request, 
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public  Task ForgotPasswordAsync(
            ForgotPasswordRequest request, 
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task LoginAsync(
            LoginRequest request,
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

        public Task ResetPasswordAsync(
            ResetPasswordRequest request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
