using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Role;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.API.Controllers;

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
    public async Task<IActionResult> Create([FromBody] RoleCreateDto model, CancellationToken cancellationToken)
    {
        if (model == null) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired));
        var created = await _service.CreateAsync(model, userId, cancellationToken);
        return StatusCode(created.StatusCode, created);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _service.GetByIdAsync(id, cancellationToken);
        if (item == null) return NotFound(ApiResponse<object>.Fail(Messages.NotFound));
        return Ok(ApiResponse<RoleDetailDto>.Ok(item, Messages.RoleRetrieveSucessfully));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CommonFilterDto filter, CancellationToken cancellationToken)
    {
        var items = await _service.GetAllAsync(filter, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.RoleRetrieveSucessfully));
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] RoleUpdateDto model, CancellationToken cancellationToken)
    {
        var res = await _service.UpdateAsync(model, id, userId, cancellationToken);
        return StatusCode(res.StatusCode, res);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var res = await _service.DeleteAsync(id, userId, cancellationToken);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("recover/{id:int}")]
    public async Task<IActionResult> Restore(int id, CancellationToken cancellationToken)
    {
        var res = await _service.RestoreAsync(id, userId, cancellationToken);
        return StatusCode(res.StatusCode, res);
    }
}
