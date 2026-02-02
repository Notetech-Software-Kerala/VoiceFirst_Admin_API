using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Plan;
using VoiceFirst_Admin.Utilities.DTOs.Features.Role;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.API.Controllers
{
    [Route("api/plan")]
    [ApiController]
    public class PlanController : ControllerBase
    {
        private readonly IPlanService _planService;
        const int userId = 1;
        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }


        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<PlanDetailDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<PlanDetailDto>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult>
          CreateAsync([FromBody] PlanCreateDto model,
          CancellationToken cancellationToken)
        {

            if (model == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    Messages.PayloadRequired,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.Payload
                ));
            }

            var result = await _planService.CreatePlanAsync(
                model, userId,
                cancellationToken);

            return StatusCode(result.StatusCode, result);
        }




        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PlanDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult>
           GetByIdAsync([FromRoute] int id,
           CancellationToken cancellationToken)
        {

            if (id <= 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    Messages.PayloadRequired,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.Payload
                ));
            }
            var result = await _planService.GetByIdAsync(id, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }


      


        [HttpGet("lookup")]
        public async Task<IActionResult> GetActivePlans(CancellationToken cancellationToken)
        {
            var result = await _planService.GetActiveAsync(cancellationToken);
            return StatusCode(result.StatusCode,result);
        }

        [HttpGet("program-details/{id:int}")]
        public async Task<IActionResult> GetProgramDetailsByPlanIdAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    Messages.PayloadRequired,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.Payload
                ));
            }
            var result = await _planService.GetProgramDetailsByPlanIdAsync
                (id, cancellationToken);
            return StatusCode(result.StatusCode, new { programDetails = result });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] PlanFilterDto filter, CancellationToken cancellationToken)
        {

            var response = await _planService.GetAllAsync(filter, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] PlanUpdateDto model, CancellationToken cancellationToken)
        {
            if (id <= 0 || model == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    Messages.PayloadRequired,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.Payload
                ));
            }
            const int userId = 1;
            var res = await _planService.UpdateAsync(id, model, userId, cancellationToken);
            return StatusCode(res.StatusCode, res);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0 )
            {
                return BadRequest(ApiResponse<object>.Fail(
                    Messages.PayloadRequired,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.Payload
                ));
            }
            const int userId = 1;
            var result = await _planService.DeleteAsync(id, userId, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("recover/{id:int}")]
        public async Task<IActionResult> RecoverAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    Messages.PayloadRequired,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.Payload
                ));
            }
            const int userId = 1;
            var result = await _planService.RecoverAsync(id, userId, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        //[HttpPost("link-role-plans")]
        //public async Task<IActionResult> LinkPlansToRole([FromBody] RolePlanLinkCreateDto model, CancellationToken cancellationToken)
        //{
        //    if (model == null)
        //        return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired));

        //    const int userId = 1;
        //    var result = await _planService.LinkPlansRoleAsync(model.RoleId, model.PlanIds, userId, cancellationToken);
        //    return StatusCode(result.StatusCode, result);
        //}
    }

}
