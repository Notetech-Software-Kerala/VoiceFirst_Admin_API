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
        private const string RefreshTokenCookieName = "refreshToken";

        private readonly IAuthService _authService;
      

        public AuthController(IAuthService authService, IWebHostEnvironment env)
        {
            _authService = authService;
           
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(
          [FromBody] LoginRequestDto request,
          CancellationToken cancellationToken)
        {
            if (request == null
                || string.IsNullOrWhiteSpace(request.Email)
                || string.IsNullOrWhiteSpace(request.Password)
                || request.Device == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    Messages.PayloadRequired,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.Payload));
            }

            var response = await _authService.LoginAsync(
                request, cancellationToken);

            if (response.StatusCode == StatusCodes.Status200OK && response.Data != null)
            {
                SetRefreshTokenCookie(
                    response.Data.RefreshToken,
                    response.Data.RefreshTokenExpiresAtUtc);

                // Return only access token in JSON body
                var clientResponse = ApiResponse<LoginResponseDto>.Ok(
                    response.Data.Response,
                    response.Message,
                    response.StatusCode);

                return StatusCode(clientResponse.StatusCode, clientResponse);
            }

            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(
            CancellationToken cancellationToken)
        {
    
            var refreshToken = Request.Cookies[RefreshTokenCookieName];

         

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized(ApiResponse<object>.Fail(
                    Messages.Unauthorized,
                    StatusCodes.Status401Unauthorized,
                    ErrorCodes.Unauthorized));
            }

            var response = await _authService.RefreshTokenAsync(
                refreshToken, cancellationToken);

            if (response.StatusCode == StatusCodes.Status200OK && response.Data != null)
            {
                SetRefreshTokenCookie(
                    response.Data.RefreshToken,
                    response.Data.RefreshTokenExpiresAtUtc);

                var clientResponse = ApiResponse<LoginResponseDto>.Ok(
                    response.Data.Response,
                    response.Message,
                    response.StatusCode);

                return StatusCode(clientResponse.StatusCode, clientResponse);
            }

            // On failure, clear the cookie
            ClearRefreshTokenCookie();
            return StatusCode(response.StatusCode, response);
        }


        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(
        CancellationToken cancellationToken)
        {
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

            var response = await _authService.LogoutAsync(
                userId, sessionId, cancellationToken);

            // Clear refresh token cookie
            ClearRefreshTokenCookie();

            return StatusCode(response.StatusCode, response);
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
                || string.IsNullOrWhiteSpace(request.Otp)
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


        private void SetRefreshTokenCookie(string token, DateTime expiresAtUtc)
        {
            
            Response.Cookies.Append(RefreshTokenCookieName, token);
        }

        private void ClearRefreshTokenCookie()
        {          
            Response.Cookies.Delete(RefreshTokenCookieName);
        }
    }
}
