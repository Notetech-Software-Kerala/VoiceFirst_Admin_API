using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.API.Controllers
{
    [Route("api/program")]
    [ApiController]
    public class SysProgramController : ControllerBase
    {
        private readonly ISysProgramService _service;
        private static readonly int userId = 1; 

        public SysProgramController(ISysProgramService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> 
            Create([FromBody] SysProgramCreateDTO model, 
            CancellationToken cancellationToken)
        {
            if (model == null)
                return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired));
          

            var created = await _service.CreateAsync(model, userId, cancellationToken);
            return StatusCode(created.StatusCode, created);
        }
    }
}
