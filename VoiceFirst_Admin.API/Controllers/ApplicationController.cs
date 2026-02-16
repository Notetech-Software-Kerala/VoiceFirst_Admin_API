using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.DTOs.Features.Application;

namespace VoiceFirst_Admin.API.Controllers
{
    [Route("api/platform")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _service;
        public ApplicationController(IApplicationService service)
        {
            _service = service;
        }
        
        [HttpGet("lookup")]
        [ProducesResponseType(typeof(ApiResponse<List<ApplicationActiveDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActiveAsync(
            CancellationToken cancellationToken)
        {
            var result = await _service.GetActiveAsync(cancellationToken);
            return StatusCode(result.StatusCode, result);
        }
    }
}
