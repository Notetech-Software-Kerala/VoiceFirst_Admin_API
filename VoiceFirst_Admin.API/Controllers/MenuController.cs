using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.DTOs.Features.Menu;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Constants;

namespace VoiceFirst_Admin.API.Controllers;

[Route("api/menu")]
[ApiController]
public class MenuController : ControllerBase
{
    private readonly IMenuService _service;
    private const int userId = 1;
    public MenuController(IMenuService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MenuCreateDto model, CancellationToken cancellationToken)
    {
        if (model == null) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired));
        var res = await _service.CreateAsync(model, userId, cancellationToken);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var res = await _service.GetByIdAsync(id, cancellationToken);
        if (res == null) return NotFound(ApiResponse<object>.Fail(Messages.NotFound));
        return Ok(ApiResponse<MenuMasterDto>.Ok(res, Messages.Success));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var res = await _service.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<MenuMasterDto>>.Ok(res, Messages.Success));
    }

    [HttpGet("detail/{id:int}")]
    public async Task<IActionResult> GetDetailById(int id, CancellationToken cancellationToken)
    {
        var res = await _service.GetDetailByIdAsync(id, cancellationToken);
        if (res == null) return NotFound(ApiResponse<object>.Fail(Messages.NotFound));
        return Ok(ApiResponse<MenuDetailDto>.Ok(res, Messages.Success));
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] MenuUpdateDto model, CancellationToken cancellationToken)
    {
        if (model == null) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired));
        var res = await _service.UpdateAsync(id, model, userId, cancellationToken);
        return StatusCode(res.StatusCode, res);
    }
}
