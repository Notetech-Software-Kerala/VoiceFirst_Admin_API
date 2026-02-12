using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Auth;
using VoiceFirst_Admin.Utilities.Enums;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private const string RefreshTokenCookieName = "refreshToken";

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
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

            if (response.StatusCode != StatusCodes.Status200OK || response.Data is null)
                return StatusCode(response.StatusCode, response);

            return BuildTokenResponse(response.Data, response.Message, response.StatusCode);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenRequestDto? body,
            CancellationToken cancellationToken)
        {
            // Mobile sends refresh token in body, Web sends via cookie
            var refreshToken = body?.RefreshToken;

            if (string.IsNullOrWhiteSpace(refreshToken))
                refreshToken = Request.Cookies[RefreshTokenCookieName];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized(ApiResponse<object>.Fail(
                    Messages.Unauthorized,
                    StatusCodes.Status401Unauthorized,
                    ErrorCodes.Unauthorized));
            }

            var response = await _authService.RefreshTokenAsync(
                refreshToken, cancellationToken);

            if (response.StatusCode != StatusCodes.Status200OK || response.Data is null)
            {
                ClearRefreshTokenCookie();
                return StatusCode(response.StatusCode, response);
            }

            return BuildTokenResponse(response.Data, response.Message, response.StatusCode);
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

            ClearRefreshTokenCookie();

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Web  → refresh token in HttpOnly cookie, access token in body.
        /// Mobile → both tokens in body (no cookie).
        /// </summary>
        private IActionResult BuildTokenResponse(LoginResultDto data, string message, int statusCode)
        {
            if (data.ClientType == ClientType.Mobile)
            {
                var mobileResponse = new MobileLoginResponseDto
                {
                    AccessToken = data.Response.AccessToken,
                    AccessTokenExpiresAtUtc = data.Response.AccessTokenExpiresAtUtc,
                    RefreshToken = data.RefreshToken,
                    RefreshTokenExpiresAtUtc = data.RefreshTokenExpiresAtUtc
                };

                var result = ApiResponse<MobileLoginResponseDto>.Ok(
                    mobileResponse, message, statusCode);

                return StatusCode(result.StatusCode, result);
            }

            // Web: refresh token in secure cookie, only access token in body
            SetRefreshTokenCookie(data.RefreshToken, data.RefreshTokenExpiresAtUtc);

            var clientResponse = ApiResponse<LoginResponseDto>.Ok(
                data.Response, message, statusCode);

            return StatusCode(clientResponse.StatusCode, clientResponse);
        }

        private void SetRefreshTokenCookie(string token, DateTime expiresAtUtc)
        {
            Response.Cookies.Append(RefreshTokenCookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = expiresAtUtc
            });
        }

        private void ClearRefreshTokenCookie()
        {
            Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });
        }
    }
}
