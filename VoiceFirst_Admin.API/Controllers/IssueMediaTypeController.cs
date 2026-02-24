using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.Constants.Swagger;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueMediaType;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

[ApiController]
[Route("api/issue-media-type")]
public class IssueMediaTypeController : ControllerBase
{
    private readonly ISysIssueMediaTypeService _svc;
    public IssueMediaTypeController(ISysIssueMediaTypeService svc) => _svc = svc;

    [HttpPost] public async Task<IActionResult> CreateAsync([FromBody] SysIssueMediaTypeCreateDTO m, CancellationToken ct) { var r = await _svc.CreateAsync(m, 1, ct); return StatusCode(r.StatusCode, r); }
    [HttpGet("{id:int}")] public async Task<IActionResult> GetByIdAsync(int id, CancellationToken ct) { if (id <= 0) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired, StatusCodes.Status400BadRequest, ErrorCodes.Payload)); var r = await _svc.GetByIdAsync(id, ct); return StatusCode(r.StatusCode, r); }
    [HttpGet] public async Task<IActionResult> GetAllAsync([FromQuery] IssueMediaTypeFilterDTO f, CancellationToken ct) { var r = await _svc.GetAllAsync(f, ct); return StatusCode(r.StatusCode, r); }
    [HttpGet("lookup")] public async Task<IActionResult> GetActiveAsync(CancellationToken ct) { var r = await _svc.GetActiveAsync(ct); return StatusCode(r.StatusCode, r); }
    [HttpPatch("{id:int}")] public async Task<IActionResult> UpdateAsync(int id, [FromBody] SysIssueMediaTypeUpdateDTO m, CancellationToken ct) { if (id <= 0) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired, StatusCodes.Status400BadRequest, ErrorCodes.Payload)); var r = await _svc.UpdateAsync(m, id, 1, ct); return StatusCode(r.StatusCode, r); }
    [HttpPatch("recover/{id:int}")] public async Task<IActionResult> RecoverAsync(int id, CancellationToken ct) { if (id <= 0) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired, StatusCodes.Status400BadRequest, ErrorCodes.Payload)); var r = await _svc.RecoverAsync(id, 1, ct); return StatusCode(r.StatusCode, r); }
    [HttpDelete("{id:int}")] public async Task<IActionResult> DeleteAsync(int id, CancellationToken ct) { if (id <= 0) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired, StatusCodes.Status400BadRequest, error: ErrorCodes.Payload)); var r = await _svc.DeleteAsync(id, 1, ct); return StatusCode(r.StatusCode, r); }
}
