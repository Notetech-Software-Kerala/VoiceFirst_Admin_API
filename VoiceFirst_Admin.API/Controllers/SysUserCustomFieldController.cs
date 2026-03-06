using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.API.Controllers;

[Consumes("application/json")]
[Produces("application/json")]
[Route("api/sys-user-custom-field")]
[ApiController]
public class SysUserCustomFieldController : ControllerBase
{
    private readonly ISysUserCustomFieldService _service;
    private readonly static int userId = 1; // placeholder

    public SysUserCustomFieldController(ISysUserCustomFieldService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserCustomFieldCreateDto dto, CancellationToken cancellationToken)
    {
        if (dto == null) return BadRequest(ApiResponse<object>.Fail("Payload required"));
        var res = await _service.CreateAsync(dto, userId, cancellationToken);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var item = await _service.GetByIdAsync(id, cancellationToken);
        if (item == null) return NotFound(ApiResponse<object>.Fail("Not found"));
        return Ok(ApiResponse<SysUserCustomFieldDetailDto>.Ok(item, "Retrieved"));
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] SysUserCustomFieldUpdateDto dto, CancellationToken cancellationToken)
    {
        var res = await _service.UpdateAsync(dto, id, userId, cancellationToken);
        return StatusCode(res.StatusCode, res);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var res = await _service.DeleteAsync(id, userId, cancellationToken);
        return StatusCode(res.StatusCode, res);
    }
}
