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

    public PasswordController(IPasswordService passwordService)
    {
        _passwordService = passwordService;
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

        // Don't expose the reset token — it is delivered via the email link
        var result = ApiResponse<object>.Ok(
            null!, response.Message, response.StatusCode);

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("validate-reset-token")]
    public async Task<IActionResult> ValidateResetToken(
        [FromQuery] string ResetToken,
        CancellationToken cancellationToken)
    {
        if (ResetToken == null
            || string.IsNullOrWhiteSpace(ResetToken))
        {
            return BadRequest(ApiResponse<object>.Fail(
                Messages.PayloadRequired,
                StatusCodes.Status400BadRequest,
                ErrorCodes.Payload));
        }

        var response = await _passwordService.ValidateResetTokenAsync(
            ResetToken, cancellationToken);

        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("reset")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordDto request,
        CancellationToken cancellationToken)
    {
        return Ok(ApiResponse<object>.Fail(
                Messages.Success,
                StatusCodes.Status200OK));
        //if (request == null
        //    || string.IsNullOrWhiteSpace(request.NewPassword)
        //    || string.IsNullOrWhiteSpace(request.PasswordResetGrant))
        //{
        //    return BadRequest(ApiResponse<object>.Fail(
        //        Messages.PayloadRequired,
        //        StatusCodes.Status400BadRequest,
        //        ErrorCodes.Payload));
        //}

        //var response = await _passwordService.ResetPasswordAsync(
        //    request, cancellationToken);

        //return StatusCode(response.StatusCode, response);
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

        var userIdClaim = User.FindFirst("sub")?.Value;
        var sessionIdClaim = User.FindFirst("sessionId")?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim)
            || string.IsNullOrWhiteSpace(sessionIdClaim))
        {
            return Unauthorized(ApiResponse<object>.Fail(
                Messages.Unauthorized,
                StatusCodes.Status401Unauthorized,
                ErrorCodes.Unauthorized));
        }

        var userId = int.Parse(userIdClaim);
        var sessionId = int.Parse(sessionIdClaim);

        var response = await _passwordService.ChangePasswordAsync(
            userId, sessionId, request, cancellationToken);

        return StatusCode(response.StatusCode, response);
    }
}
