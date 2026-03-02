using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Application;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.API.Controllers
{
    [Route("api/platform")]
    [ApiController]
    //[Authorize]
    public sealed class PlatformController : ControllerBase
    {
        private readonly IPlatformService _platformService;

        public PlatformController(IPlatformService platformService)
        {
            _platformService = platformService
                ?? throw new ArgumentNullException(nameof(platformService));
        }

        /// <summary>
        /// Retrieves all active platforms.
        /// </summary>
        [HttpGet("lookup")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PlatformLookupDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActivePlatformsAsync(
            CancellationToken cancellationToken)
        {
            var response = await _platformService
                .GetActivePlatformsAsync(cancellationToken);

            return StatusCode(response.StatusCode, response);
        }
    }
}
