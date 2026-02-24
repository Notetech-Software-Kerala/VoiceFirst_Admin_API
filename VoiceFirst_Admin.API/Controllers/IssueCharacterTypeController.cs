using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.API.Security;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.Constants.Swagger;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueCharacterType;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

[ApiController]
[Route("api/issue-character-type")]
public class IssueCharacterTypeController : ControllerBase
{
    private readonly ISysIssueCharacterTypeService _service;
    public IssueCharacterTypeController(ISysIssueCharacterTypeService service) => _service = service;

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SysIssueCharacterTypeDTO>), StatusCodes.Status201Created)]
    [SwaggerResponseDescription(StatusCodes.Status201Created, Description.ISSUE_CHARACTER_TYPE_CREATED, Messages.IssueCharacterTypeCreated)]
    public async Task<IActionResult> CreateAsync([FromBody] SysIssueCharacterTypeCreateDTO model, CancellationToken ct) { var r = await _service.CreateAsync(model, 1, ct); return StatusCode(r.StatusCode, r); }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SysIssueCharacterTypeDTO>), StatusCodes.Status200OK)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ISSUE_CHARACTER_TYPE_RETRIEVED, Messages.IssueCharacterTypeRetrieved)]
    public async Task<IActionResult> GetByIdAsync(int id, CancellationToken ct) { if (id <= 0) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired, StatusCodes.Status400BadRequest, ErrorCodes.Payload)); var r = await _service.GetByIdAsync(id, ct); return StatusCode(r.StatusCode, r); }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResultDto<SysIssueCharacterTypeDTO>>), StatusCodes.Status200OK)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ISSUE_CHARACTER_TYPES_RETRIEVED, Messages.IssueCharacterTypesRetrieved)]
    public async Task<IActionResult> GetAllAsync([FromQuery] IssueCharacterTypeFilterDTO filter, CancellationToken ct) { var r = await _service.GetAllAsync(filter, ct); return StatusCode(r.StatusCode, r); }

    [HttpGet("lookup")]
    [ProducesResponseType(typeof(ApiResponse<List<SysIssueCharacterTypeActiveDTO?>>), StatusCodes.Status200OK)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ISSUE_CHARACTER_TYPES_RETRIEVED, Messages.IssueCharacterTypesRetrieved)]
    public async Task<IActionResult> GetActiveAsync(CancellationToken ct) { var r = await _service.GetActiveAsync(ct); return StatusCode(r.StatusCode, r); }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SysIssueCharacterTypeDTO>), StatusCodes.Status200OK)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ISSUE_CHARACTER_TYPE_UPDATED, Messages.IssueCharacterTypeUpdated)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] SysIssueCharacterTypeUpdateDTO model, CancellationToken ct) { if (id <= 0) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired, StatusCodes.Status400BadRequest, ErrorCodes.Payload)); var r = await _service.UpdateAsync(model, id, 1, ct); return StatusCode(r.StatusCode, r); }

    [HttpPatch("recover/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SysIssueCharacterTypeDTO>), StatusCodes.Status200OK)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ISSUE_CHARACTER_TYPE_RECOVERED, Messages.IssueCharacterTypeRecovered)]
    public async Task<IActionResult> RecoverAsync(int id, CancellationToken ct) { if (id <= 0) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired, StatusCodes.Status400BadRequest, ErrorCodes.Payload)); var r = await _service.RecoverAsync(id, 1, ct); return StatusCode(r.StatusCode, r); }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    [SwaggerResponseDescription(StatusCodes.Status200OK, Description.ISSUE_CHARACTER_TYPE_DELETED, Messages.IssueCharacterTypeDeleted)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken ct) { if (id <= 0) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired, StatusCodes.Status400BadRequest, error: ErrorCodes.Payload)); var r = await _service.DeleteAsync(id, 1, ct); return StatusCode(r.StatusCode, r); }
}
