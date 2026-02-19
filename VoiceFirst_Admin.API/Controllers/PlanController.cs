using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using VoiceFirst_Admin.API.Security;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Plan;
using VoiceFirst_Admin.Utilities.DTOs.Features.PlanProgramActoinLink;
using VoiceFirst_Admin.Utilities.DTOs.Features.Role;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Constants.Swagger;

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
        [SwaggerResponseDescription(StatusCodes.Status201Created, Description.PLAN_CREATED, Messages.PlanCreated)]
        [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.PlanAlreadyExists)]
        [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.ProgramNotFoundById)]
        [SwaggerResponseDescription(StatusCodes.Status422UnprocessableEntity, Description.UNPROCESSABLE_422, Messages.PlanAlreadyExistsRecoverable)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
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
        [SwaggerResponseDescription(StatusCodes.Status200OK, Description.PLAN_RETRIEVED, Messages.PlanRetrieved)]
        [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.PlanNotFoundById)]
        [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
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
        [ProducesResponseType(typeof(ApiResponse<List<PlanActiveDto?>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseDescription(StatusCodes.Status200OK, Description.PLANS_RETRIEVED, Messages.PlansRetrieved)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
        public async Task<IActionResult> GetActivePlans(CancellationToken cancellationToken)
        {
            var result = await _planService.GetActiveAsync(cancellationToken);
            return StatusCode(result.StatusCode,result);
        }

        [HttpGet("program-details/{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProgramPlanDetailDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseDescription(StatusCodes.Status200OK, Description.PLAN_RETRIEVED, Messages.PlanRetrieved)]
        [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.PlanNotFoundById)]
        [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
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
            return StatusCode(result.StatusCode, result);
        }




        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<PlanDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseDescription(StatusCodes.Status200OK, Description.PLANS_RETRIEVED, Messages.PlansRetrieved)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
        public async Task<IActionResult> GetAllAsync
            ([FromQuery] PlanFilterDto filter, 
            CancellationToken cancellationToken)
        {

            var response = await _planService.GetAllAsync(filter, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }




        [AuthorizeAdmin]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<PlanDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseDescription(StatusCodes.Status200OK, Description.PLAN_DELETED, Messages.PlanDeleted)]
        [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.PlanNotFoundById)]
        [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.PlanAlreadyDeleted)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
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
            var result = await _planService.DeleteAsync(id, userId, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }


        [AuthorizeAdmin]
        [HttpPatch("recover/{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<PlanDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseDescription(StatusCodes.Status200OK, Description.PLAN_RECOVERED, Messages.PlanRecovered)]
        [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.PlanNotFoundById)]
        [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.PlanAlreadyRecovered)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
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





        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<PlanDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<PlanDetailDto>), StatusCodes.Status422UnprocessableEntity)]
        [SwaggerResponseDescription(StatusCodes.Status200OK, Description.PLAN_UPDATED, Messages.PlanUpdated)]
        [SwaggerResponseDescription(StatusCodes.Status204NoContent, Description.PLAN_UPDATED, Messages.Updated)]
        [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.PlanNotFoundById)]
        [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.PlanAlreadyExists)]
        [SwaggerResponseDescription(StatusCodes.Status422UnprocessableEntity, Description.UNPROCESSABLE_422, Messages.PlanAlreadyExistsRecoverable)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
        public async Task<IActionResult> UpdateAsync
            (int id, 
            [FromBody] PlanUpdateDto model, 
            CancellationToken cancellationToken)
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
