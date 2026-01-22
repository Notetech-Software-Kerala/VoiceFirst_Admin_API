using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.API.Controllers
{
    [Route("api/plan")]
    [ApiController]
    public class PlanController : ControllerBase
    {
        private readonly IPlanService _planService;
        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanCreateDto model, CancellationToken cancellationToken)
        {
            if (model == null)
                return BadRequest(VoiceFirst_Admin.Utilities.Models.Common.ApiResponse<object>.Fail(VoiceFirst_Admin.Utilities.Constants.Messages.PayloadRequired));

            const int userId = 1;
            var result = await _planService.CreateAsync(model, userId, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> GetActivePlans(CancellationToken cancellationToken)
        {
            var result = await _planService.GetActiveAsync(cancellationToken);
            return StatusCode(result.StatusCode,result);
        }

        [HttpGet("{planId:int}/program-details")]
        public async Task<IActionResult> GetProgramDetailsByPlanIdAsync(int planId, CancellationToken cancellationToken)
        {
            var result = await _planService.GetProgramDetailsByPlanIdAsync(planId, cancellationToken);
            return StatusCode(result.StatusCode, new { programDetails = result });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanFilterDto filter, CancellationToken cancellationToken)
        {
            var result = await _planService.GetAllAsync(filter, cancellationToken);
            return Ok(ApiResponse<object>.Ok(result, VoiceFirst_Admin.Utilities.Constants.Messages.PlanRetrieved));
        }

        [HttpPatch("{planId:int}")]
        public async Task<IActionResult> UpdateAsync(int planId, [FromBody] VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanUpdateDto model, CancellationToken cancellationToken)
        {
            if (model == null)
                return BadRequest(VoiceFirst_Admin.Utilities.Models.Common.ApiResponse<object>.Fail(VoiceFirst_Admin.Utilities.Constants.Messages.PayloadRequired));

            const int userId = 1;
            var res = await _planService.UpdateAsync(planId, model, userId, cancellationToken);
            return StatusCode(res.StatusCode, res);
        }
    }

}
