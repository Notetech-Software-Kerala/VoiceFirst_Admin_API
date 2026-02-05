using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.DTOs.Features.Auth;

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
            return Ok();
        }



        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordDto request,
            CancellationToken cancellationToken)
        {
            return Ok();
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
