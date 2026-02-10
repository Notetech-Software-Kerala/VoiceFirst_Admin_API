using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Auth;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(
          [FromBody] LoginDto request,
          CancellationToken cancellationToken)
        {
            return Ok();
        }


        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(
        CancellationToken cancellationToken)
        {
            var userId = User.FindFirst("sub")?.Value;

            await _authService.LogoutAsync(userId, cancellationToken);

            return NoContent();
        }



        [HttpPost("forgot-password")]
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

            var response = await _authService.ForgotPasswordAsync(
                request, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }



        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordDto request,
            CancellationToken cancellationToken)
        {
            if (request == null
                || string.IsNullOrWhiteSpace(request.Email)
                || string.IsNullOrWhiteSpace(request.Token)
                || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest(ApiResponse<object>.Fail(
                    Messages.PayloadRequired,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.Payload));
            }

            var response = await _authService.ResetPasswordAsync(
                request, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }



        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordDto request,
            CancellationToken cancellationToken)
        {
            return Ok();
        }



    }
}
