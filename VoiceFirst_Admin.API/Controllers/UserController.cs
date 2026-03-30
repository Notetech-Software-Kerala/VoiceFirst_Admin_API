using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.User;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userProfileService;

        public UserController(IUserService userProfileService)
        {
            _userProfileService = userProfileService;
        }


        [HttpGet]
        [Route("me")]
        [Authorize]
        public async Task<IActionResult> GetProfileAsync(CancellationToken cancellationToken)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;


                if (string.IsNullOrWhiteSpace(userIdClaim)
                   )
                {
                    return Unauthorized(ApiResponse<object>.Fail(
                        Messages.Unauthorized,
                        StatusCodes.Status401Unauthorized,
                        ErrorCodes.Unauthorized));
                }

                var userId = int.Parse(userIdClaim);
                var profileResponse = await _userProfileService.GetProfileAsync(userId, cancellationToken);
                return Ok(profileResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }



        [HttpPatch]
        [Route("me")]
        [Authorize]
        public async Task<IActionResult> UpdateProfileAsync([FromBody] UserProfileUpdateDto userProfileUpdateDto, CancellationToken cancellationToken)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;


                if (string.IsNullOrWhiteSpace(userIdClaim)
                   )
                {
                    return Unauthorized(ApiResponse<object>.Fail(
                        Messages.Unauthorized,
                        StatusCodes.Status401Unauthorized,
                        ErrorCodes.Unauthorized));
                }

                var userId = int.Parse(userIdClaim);
                var profileResponse = await _userProfileService.UpdateProfileAsync(userId, userProfileUpdateDto, cancellationToken);
                return Ok(profileResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }
    }
}
