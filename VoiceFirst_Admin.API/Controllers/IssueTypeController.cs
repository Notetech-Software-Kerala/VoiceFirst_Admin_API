using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.API.Security;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.Constants.Swagger;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

[ApiController]
[Route("api/issue-type")]
public class IssueTypeController : ControllerBase
{
    private readonly ISysIssueTypeService _service;
    private readonly IUserContext _userContext;

    public IssueTypeController(ISysIssueTypeService service, IUserContext userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    /// <summary>
    /// Creates a new issue type.
    /// </summary>
    //[Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SysIssueTypeDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<SysIssueTypeDTO>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseDescription(StatusCodes.Status201Created, Description.ISSUE_TYPE_CREATED, Messages.IssueTypeCreated)]
    [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.IssueTypeAlreadyExists)]
    [SwaggerResponseDescription(StatusCodes.Status422UnprocessableEntity, Description.UNPROCESSABLE_422, Messages.IssueTypeAlreadyExistsRecoverable)]
    [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
    [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
    [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.SomethingWentWrong)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] SysIssueTypeCreateDTO model,
        CancellationToken cancellationToken)
    {
        var response = await _service.CreateAsync(
            model,
            1,
            cancellationToken);

        return StatusCode(response.StatusCode, response);
    }




    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SysIssueTypeDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ISSUE_TYPE_RETRIEVED, Messages.IssueTypeRetrieved)]
    [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.IssueTypeNotFoundById)]
    [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
    [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
    [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
    public async Task<IActionResult>
        GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest(ApiResponse<object>.Fail(
                Messages.PayloadRequired,
                StatusCodes.Status400BadRequest,
                ErrorCodes.Payload
            ));
        }
        var response = await _service.GetByIdAsync(id, cancellationToken);

        return StatusCode(response.StatusCode, response);
    }



    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResultDto<SysIssueTypeDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ISSUE_TYPES_RETRIEVED, Messages.IssueTypesRetrieved)]
    [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
    [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
    public async Task<IActionResult> GetAllAsync(
      [FromQuery] IssueTypeFilterDTO filter,
      CancellationToken cancellationToken)
    {
        var response = await _service.GetAllAsync(filter, cancellationToken);

        return StatusCode(response.StatusCode, response);
    }



    [HttpGet("lookup")]
    [ProducesResponseType(typeof(ApiResponse<List<SysIssueTypeActiveDTO?>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ISSUE_TYPES_RETRIEVED, Messages.IssueTypesRetrieved)]
    [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
    [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
    public async Task<IActionResult> GetActiveAsync(
      CancellationToken cancellationToken)
    {
        var response = await _service.GetActiveAsync(cancellationToken);
        return StatusCode(response.StatusCode, response);
    }




    /// <summary>
    /// Updates an issue type.
    /// </summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SysIssueTypeDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResponse<SysIssueTypeDTO>), StatusCodes.Status422UnprocessableEntity)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ISSUE_TYPE_UPDATED, Messages.IssueTypeUpdated)]
    [SwaggerResponseDescription(StatusCodes.Status204NoContent, Description.ISSUE_TYPE_UPDATED, Messages.IssueTypeUpdated)]
    [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
    [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.IssueTypeNotFoundById)]
    [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.IssueTypeAlreadyExists)]
    [SwaggerResponseDescription(StatusCodes.Status422UnprocessableEntity, Description.UNPROCESSABLE_422, Messages.IssueTypeAlreadyExistsRecoverable)]
    [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
    [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
    public async Task<IActionResult> UpdateAsync(
    int id,
    [FromBody] SysIssueTypeUpdateDTO model,
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

        var response = await _service.UpdateAsync(
            model, id, 1, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }



    [HttpPatch("recover/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SysIssueTypeDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ISSUE_TYPE_RECOVERED, Messages.IssueTypeRecovered)]
    [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.IssueTypeNotFoundById)]
    [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
    [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.ISSUE_TYPE_ALREADY_RECOVERED_409, Messages.IssueTypeAlreadyRecovered)]
    [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
    [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
    public async Task<IActionResult> RecoverAsync(
        int id,
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
        var recoveredDto = await _service.RecoverIssueTypeAsync
           (id, 1, cancellationToken);
        return StatusCode(recoveredDto.StatusCode, recoveredDto);
    }


    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ISSUE_TYPE_DELETED, Messages.IssueTypeDeleted)]
    [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.IssueTypeNotFoundById)]
    [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
    [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.ISSUE_TYPE_ALREADY_DELETED_409, Messages.IssueTypeAlreadyDeleted)]
    [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
    [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
    public async Task<IActionResult>
        DeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest(ApiResponse<object>.Fail(
                Messages.PayloadRequired,
                StatusCodes.Status400BadRequest,
                error: ErrorCodes.Payload
            ));
        }
        var deleteDto = await _service.DeleteAsync(id, 1, cancellationToken);
        return StatusCode(deleteDto.StatusCode, deleteDto);
    }
}
