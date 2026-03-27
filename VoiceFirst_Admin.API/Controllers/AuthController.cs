using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using VoiceFirst_Admin.API.Security;
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
        [HttpGet("ask")]
        public async Task<IActionResult> Ask(string prompt)
        {
            try
            {
                using var client = new HttpClient
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };

                
                var requestBody = new
                {
                    model = "llama3:8b",
                    prompt = prompt,
                    stream = false,
                    keep_alive = "5m"
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync("http://localhost:11434/api/generate", content);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Error calling Ollama");
                }

                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);

                var root = doc.RootElement;

                var aiResponse = root.GetProperty("response").GetString();
                var doneReason = root.GetProperty("done_reason").GetString();

                // 🔁 Retry if model just loaded
                if (string.IsNullOrEmpty(aiResponse) && doneReason == "load")
                {
                    var retryResponse = await client.PostAsync("http://localhost:11434/api/generate", content);

                    var retryJson = await retryResponse.Content.ReadAsStringAsync();
                    var retryDoc = JsonDocument.Parse(retryJson);

                    aiResponse = retryDoc.RootElement.GetProperty("response").GetString();
                }

                return Ok(new
                {
                    prompt,
                    response = aiResponse
                });
            }
            catch (TaskCanceledException)
            {
                return StatusCode(408, "Request timeout. Ollama may be loading the model.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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

            var fingerprint = ComputeFingerprint();

            var response = await _authService.LoginAsync(
                request, fingerprint, cancellationToken);

            if (response.StatusCode != StatusCodes.Status200OK || response.Data is null)
                return StatusCode(response.StatusCode, response);

            return BuildTokenResponse(response.Data, response.Message, response.StatusCode);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenRequestDto? body,
            CancellationToken cancellationToken)
        {
            var fromBody = !string.IsNullOrWhiteSpace(body?.RefreshToken);
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

            // Enforce delivery channel: Web = cookie only, Mobile = body only
            var clientType = PeekClientType(refreshToken);
            if (clientType == ClientType.Web && fromBody)
            {
                return Unauthorized(ApiResponse<object>.Fail(
                    Messages.Unauthorized,
                    StatusCodes.Status401Unauthorized,
                    ErrorCodes.Unauthorized));
            }

            if ((clientType == ClientType.IOS || clientType == ClientType.Android) && !fromBody)
            {
                return Unauthorized(ApiResponse<object>.Fail(
                    Messages.Unauthorized,
                    StatusCodes.Status401Unauthorized,
                    ErrorCodes.Unauthorized));
            }

            var fingerprint = ComputeFingerprint();

            var response = await _authService.RefreshTokenAsync(
                refreshToken, fingerprint, cancellationToken);

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
            if (data.ClientType == ClientType.IOS || data.ClientType == ClientType.Android)
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

        private string ComputeFingerprint()
            => FingerprintHelper.Compute(Request.Headers);

        /// <summary>
        /// Peek at the clientType claim from the JWT without full validation.
        /// Full cryptographic validation happens in the service layer.
        /// </summary>
        private static ClientType? PeekClientType(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
                return null;

            var jwt = handler.ReadJwtToken(token);
            var claim = jwt.Claims.FirstOrDefault(c => c.Type == "clientType")?.Value;

            return Enum.TryParse<ClientType>(claim, true, out var ct) ? ct : null;
        }

        private void SetRefreshTokenCookie(string token, DateTime expiresAtUtc)
        {
            Response.Cookies.Append(RefreshTokenCookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
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
                SameSite = SameSiteMode.None,
                Path = "/"
            });
        }
    }
}
