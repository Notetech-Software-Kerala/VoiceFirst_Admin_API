using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.API.Security;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.Constants.Swagger;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueStatus;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

[ApiController]
[Route("api/issue-status")]
public class IssueStatusController : ControllerBase
{
    private readonly ISysIssueStatusService _service;
    private readonly IUserContext _userContext;

    public IssueStatusController(ISysIssueStatusService service, IUserContext userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SysIssueStatusDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<SysIssueStatusDTO>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseDescription(StatusCodes.Status201Created, Description.ISSUE_STATUS_CREATED, Messages.IssueStatusCreated)]
    [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.IssueStatusAlreadyExists)]
    [SwaggerResponseDescription(StatusCodes.Status422UnprocessableEntity, Description.UNPROCESSABLE_422, Messages.IssueStatusAlreadyExistsRecoverable)]
    [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
    [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.SomethingWentWrong)]
    public async Task<IActionResult> CreateAsync([FromBody] SysIssueStatusCreateDTO model, CancellationToken cancellationToken)
    {
        var response = await _service.CreateAsync(model, 1, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SysIssueStatusDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ISSUE_STATUS_RETRIEVED, Messages.IssueStatusRetrieved)]
    [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.IssueStatusNotFoundById)]
    public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (id <= 0) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired, StatusCodes.Status400BadRequest, ErrorCodes.Payload));
        var response = await _service.GetByIdAsync(id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResultDto<SysIssueStatusDTO>>), StatusCodes.Status200OK)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ISSUE_STATUSES_RETRIEVED, Messages.IssueStatusesRetrieved)]
    public async Task<IActionResult> GetAllAsync([FromQuery] IssueStatusFilterDTO filter, CancellationToken cancellationToken)
    {
        var response = await _service.GetAllAsync(filter, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("lookup")]
    [ProducesResponseType(typeof(ApiResponse<List<SysIssueStatusActiveDTO?>>), StatusCodes.Status200OK)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ISSUE_STATUSES_RETRIEVED, Messages.IssueStatusesRetrieved)]
    public async Task<IActionResult> GetActiveAsync(CancellationToken cancellationToken)
    {
        var response = await _service.GetActiveAsync(cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SysIssueStatusDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<SysIssueStatusDTO>), StatusCodes.Status422UnprocessableEntity)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ISSUE_STATUS_UPDATED, Messages.IssueStatusUpdated)]
    [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.IssueStatusNotFoundById)]
    [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.IssueStatusAlreadyExists)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] SysIssueStatusUpdateDTO model, CancellationToken cancellationToken)
    {
        if (id <= 0) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired, StatusCodes.Status400BadRequest, ErrorCodes.Payload));
        var response = await _service.UpdateAsync(model, id, 1, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPatch("recover/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SysIssueStatusDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ISSUE_STATUS_RECOVERED, Messages.IssueStatusRecovered)]
    [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.IssueStatusNotFoundById)]
    [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.ISSUE_STATUS_ALREADY_RECOVERED_409, Messages.IssueStatusAlreadyRecovered)]
    public async Task<IActionResult> RecoverAsync(int id, CancellationToken cancellationToken)
    {
        if (id <= 0) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired, StatusCodes.Status400BadRequest, ErrorCodes.Payload));
        var response = await _service.RecoverIssueStatusAsync(id, 1, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ISSUE_STATUS_DELETED, Messages.IssueStatusDeleted)]
    [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.IssueStatusNotFoundById)]
    [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.ISSUE_STATUS_ALREADY_DELETED_409, Messages.IssueStatusAlreadyDeleted)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (id <= 0) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired, StatusCodes.Status400BadRequest, error: ErrorCodes.Payload));
        var response = await _service.DeleteAsync(id, 1, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
