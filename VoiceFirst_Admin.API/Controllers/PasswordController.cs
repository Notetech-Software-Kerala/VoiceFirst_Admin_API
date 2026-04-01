using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Auth;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.API.Controllers;

[Route("api/password")]
[ApiController]
public class PasswordController : ControllerBase
{
    private readonly IPasswordService _passwordService;
    private readonly IUserContext _userContext;

    public PasswordController(IPasswordService passwordService, IUserContext userContext)
    {
        _passwordService = passwordService;
        _userContext = userContext;
    }

    [HttpPost("forgot")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordDto request,
        CancellationToken cancellationToken)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(ApiResponse<object>.Fail(
                Messages.PayloadRequired,
                StatusCodes.Status400BadRequest,
                ErrorCodes.Payload));
        }

        var response = await _passwordService.ForgotPasswordAsync(
            request, cancellationToken);

        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("validate-reset-token/{resetToken}")]
    public async Task<IActionResult> ValidateResetToken(
        [FromRoute] string resetToken,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(resetToken))
        {
            return BadRequest(ApiResponse<object>.Fail(
                Messages.PayloadRequired,
                StatusCodes.Status400BadRequest,
                ErrorCodes.Payload));
        }

        var response = await _passwordService.ValidateResetTokenAsync(
            resetToken, cancellationToken);

        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("reset")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordDto request,
        CancellationToken cancellationToken)
    {
        if (request == null
            || string.IsNullOrWhiteSpace(request.NewPassword)
            || string.IsNullOrWhiteSpace(request.PasswordResetGrant))
        {
            return BadRequest(ApiResponse<object>.Fail(
                Messages.PayloadRequired,
                StatusCodes.Status400BadRequest,
                ErrorCodes.Payload));
        }

        var response = await _passwordService.ResetPasswordAsync(
            request, cancellationToken);

        return StatusCode(response.StatusCode, response);
    }

    [Authorize]
    [HttpPost("change")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordDto request,
        CancellationToken cancellationToken)
    {
        if (request == null
            || string.IsNullOrWhiteSpace(request.OldPassword)
            || string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return BadRequest(ApiResponse<object>.Fail(
                Messages.PayloadRequired,
                StatusCodes.Status400BadRequest,
                ErrorCodes.Payload));
        }

        var response = await _passwordService.ChangePasswordAsync(
            _userContext.UserId, _userContext.SessionId, request, cancellationToken);

        return StatusCode(response.StatusCode, response);
    }
}
