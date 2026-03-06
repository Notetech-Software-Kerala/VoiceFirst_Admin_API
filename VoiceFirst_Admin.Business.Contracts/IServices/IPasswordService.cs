using VoiceFirst_Admin.Utilities.DTOs.Features.Auth;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices;

public interface IPasswordService
{
    Task<ApiResponse<ForgotPasswordResultDto>> ForgotPasswordAsync(
        ForgotPasswordDto request,
        CancellationToken cancellationToken);

    Task<ApiResponse<object>> ValidateResetTokenAsync(
        ValidateResetTokenDto request,
        CancellationToken cancellationToken);

    Task<ApiResponse<object>> ResetPasswordAsync(
        ResetPasswordDto request,
        CancellationToken cancellationToken);

    Task<ApiResponse<object>> ChangePasswordAsync(
        int userId,
        int sessionId,
        ChangePasswordDto request,
        CancellationToken cancellationToken);
}
