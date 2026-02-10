using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.DTOs.Features.Menu;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

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

    [HttpGet("master")]
    public async Task<IActionResult> GetAllMasters([FromQuery] MenuFilterDto filter, CancellationToken cancellationToken)
    {
        var paged = await _service.GetAllMenuMastersAsync(filter, cancellationToken);
        return Ok(ApiResponse<PagedResultDto<MenuMasterDto>>.Ok(paged, Messages.MenuRetrieveSucessfully));
    }
    [HttpGet("master/{id:int}")]
    public async Task<IActionResult> GetAllMenuMastersByIdAsync(int id, CancellationToken cancellationToken)
    {
        var menu = await _service.GetAllMenuMastersByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<MenuMasterDetailDto>.Ok(menu, Messages.MenuRetrieveSucessfully));
    }
    [HttpGet("web")]
    public async Task<IActionResult> GetAllWeb(CancellationToken cancellationToken)
    {
        var data = await _service.GetAllWebMenusAsync(cancellationToken);
        return Ok(ApiResponse<List<WebMenuDto>>.Ok(data, Messages.WebMenuRetrieveSucessfully));
    }
    [HttpGet("app")]
    public async Task<IActionResult> GetAllApp(CancellationToken cancellationToken)
    {
        var data = await _service.GetAllAppMenusAsync(cancellationToken);
        return Ok(ApiResponse<List<AppMenuDto>>.Ok(data, Messages.AppMenuRetrieveSucessfully));
    }
    [HttpPatch("master/{id:int}")]
    public async Task<IActionResult> UpdateMaster(int id, [FromBody] MenuMasterUpdateDto model, CancellationToken cancellationToken)
    {
        if (model == null) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired));
        var res = await _service.UpdateMenuMasterAsync(id, model, userId, cancellationToken);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("web/bulk")]
    public async Task<IActionResult> BulkUpdateWeb([FromBody] WebMenuBulkUpdateDto model, CancellationToken cancellationToken)
    {
        if (model == null) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired));
        var res = await _service.BulkUpdateWebMenusAsync(model, userId, cancellationToken);
        if (res.StatusCode != StatusCodes.Status200OK)
            return StatusCode(res.StatusCode, res);
        else
        {
            var data = await _service.GetAllWebMenusAsync(cancellationToken);
            return Ok(ApiResponse<List<WebMenuDto>>.Ok(data, Messages.WebMenuUpdatedSucessfully));
        }
    }
    [HttpPatch("app/bulk")]
    public async Task<IActionResult> BulkUpdateApp([FromBody] AppMenuBulkUpdateDto model, CancellationToken cancellationToken)
    {
        if (model == null) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired));
        var res = await _service.BulkUpdateAppMenusAsync(model, userId, cancellationToken);
        if(res.StatusCode!= StatusCodes.Status200OK)
            return StatusCode(res.StatusCode, res);
        else
        {
            var data = await _service.GetAllAppMenusAsync(cancellationToken);
            return Ok(ApiResponse<List<AppMenuDto>>.Ok(data, Messages.WebMenuUpdatedSucessfully));
        }
    }

    
}
