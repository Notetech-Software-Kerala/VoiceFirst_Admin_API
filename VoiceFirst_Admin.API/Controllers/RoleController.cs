using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.Constants.Swagger;
using VoiceFirst_Admin.Utilities.DTOs.Features.Role;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.API.Controllers;

[Consumes("application/json")]
[Produces("application/json")]

[Route("api/role")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly IRoleService _service;
    private readonly static int userId = 1; // placeholder
    public RoleController(IRoleService service)
    {
        _service = service;
    }

    [HttpPost]

    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]

    [SwaggerResponseDescription(StatusCodes.Status201Created, Description.ROLE_CREATED, Messages.RoleCreated, DataExamples.ROLCREATEDATA)]
    [SwaggerResponseDescription(StatusCodes.Status403Forbidden, Description.SYSTEM_ROLE_403, Messages.RoleNameDefault)]
    [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.ROLE_FAILD, Messages.RoleFailed)]
    [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.RoleNameAlreadyExists)]
    [SwaggerResponseDescription(StatusCodes.Status422UnprocessableEntity, Description.CONFLICT_WITH_DELETED_422, Messages.RoleNameExistsInTrash)]
    [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Messages.SomethingWentWrong, Messages.SomethingWentWrong)]
    public async Task<IActionResult> Create([FromBody] RoleCreateDto model, CancellationToken cancellationToken)
    {
        if (model == null) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired));
        var created = await _service.CreateAsync(model, userId, cancellationToken);
        return StatusCode(created.StatusCode, created);
    }
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.PROGRAM_RETRIEVED, Messages.RoleRetrieveSucessfully, DataExamples.ROLEBYIDDATA)]
    [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.NotFound)]
    [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.SomethingWentWrong)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _service.GetByIdAsync(id, cancellationToken);
        if (item == null) return NotFound(ApiResponse<object>.Fail(Messages.NotFound));
        return Ok(ApiResponse<RoleDetailDto>.Ok(item, Messages.RoleRetrieveSucessfully));
    }
    //[HttpGet("plan-role-link")]
    //[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    //[ProducesResponseType(typeof(ApiResponse<PlanRoleActionLinkDetailsDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    //[SwaggerResponseDescription(StatusCodes.Status200OK, Description.PROGRAM_RETRIEVED, Messages.RoleRetrieveSucessfully, DataExamples.ROLGETALLDATA)]
    //[SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.NotFound)]
    //[SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.SomethingWentWrong)]
    //public async Task<IActionResult> GetByPlanIdAsync([FromQuery] PlanRoleDto planRoleDto, CancellationToken cancellationToken)
    //{
    //    var item = await _service.GetByPlanIdAsync(planRoleDto, cancellationToken);
    //    if (item == null) return NotFound(ApiResponse<object>.Fail(Messages.NotFound));
    //    return Ok(ApiResponse<PlanRoleActionLinkDetailsDto>.Ok(item, Messages.RoleRetrieveSucessfully));
    //}

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ACTIVITIES_RETRIEVED, Messages.RoleRetrieveSucessfully, DataExamples.ROLGETALLDATA)]
    [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.SomethingWentWrong)]
    public async Task<IActionResult> GetAll([FromQuery] RoleFilterDto filter, CancellationToken cancellationToken)
    {
        var items = await _service.GetAllAsync(filter, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.RoleRetrieveSucessfully));
    }


    [HttpGet("lookup")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.USERS_RETRIEVED, Messages.RoleRetrieveSucessfully, DataExamples.ROLLOOKUPATA)]
    [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.SomethingWentWrong)]
    public async Task<IActionResult> GetLookupAsync(CancellationToken cancellationToken)
    {
        var items = await _service.GetLookUpAllAsync(1,cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.RoleRetrieveSucessfully));
    }


    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ROLE_UPDATED, Messages.RoleCreated, DataExamples.ROLUPDATEDATA)]
    [SwaggerResponseDescription(StatusCodes.Status403Forbidden, Description.SYSTEM_ROLE_403, Messages.RoleNameDefault)]
    [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.ROLE_FAILD, Messages.RoleFailed)]
    [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.RoleNameAlreadyExists)]
    [SwaggerResponseDescription(StatusCodes.Status422UnprocessableEntity, Description.CONFLICT_WITH_DELETED_422, Messages.RoleNameExistsInTrash)]
    [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Messages.SomethingWentWrong, Messages.SomethingWentWrong)]
    public async Task<IActionResult> Update(int id, [FromBody] RoleUpdateDto model, CancellationToken cancellationToken)
    {
        var res = await _service.UpdateAsync(model, id, userId, cancellationToken);
        return StatusCode(res.StatusCode, res);
    }


    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ACTIVITY_DELETED, Messages.RoleDeleteSucessfully)]
    [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.NotFound)]
    [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.SomethingWentWrong)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var res = await _service.DeleteAsync(id, userId, cancellationToken);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("recover/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ACTIVITY_RECOVERED, Messages.RoleRestoreSucessfully)]
    [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.NotFound)]
    [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.SomethingWentWrong)]
    public async Task<IActionResult> Restore(int id, CancellationToken cancellationToken)
    {
        var res = await _service.RestoreAsync(id, userId, cancellationToken);
        return StatusCode(res.StatusCode, res);
    }
}
