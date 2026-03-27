using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.API.Controllers;

[Consumes("application/json")]
[Produces("application/json")]
[Route("api/user-custom-field")]
[ApiController]
public class CustomFieldController : ControllerBase
{
    private readonly ISysUserCustomFieldService _service;
    private readonly static int userId = 1; // placeholder

    public CustomFieldController(ISysUserCustomFieldService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CustomFieldCreateDto dto, CancellationToken cancellationToken)
    {
        if (dto == null) return BadRequest(ApiResponse<object>.Fail(Messages.BadRequest));
        var res = await _service.CreateAsync(dto, userId, cancellationToken);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var item = await _service.GetByIdAsync(id, cancellationToken);
        if (item == null) return NotFound(ApiResponse<object>.Fail(Messages.CustomFieldsNotFound));
        return Ok(ApiResponse<CustomFieldDetailDto>.Ok(item, Messages.CustomFieldRetrieved));
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CustomFieldUpdateDto dto, CancellationToken cancellationToken)
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

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CustomFieldFilterDto filter, CancellationToken cancellationToken)
    {
        var items = await _service.GetAllAsync(filter, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.CustomFieldRetrieved));
    }
    [HttpGet("lookup")]
    public async Task<IActionResult> GetLookUp([FromQuery] BasicFilterDto filter, CancellationToken cancellationToken)
    {
        var items = await _service.GetLookUpAsync(filter, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.CustomFieldRetrieved));
    }
    [HttpGet("lookup/datatype")]
    public async Task<IActionResult> GetDataTypeLookUp( CancellationToken cancellationToken)
    {
        var items = await _service.GetDataTypeLookUpAsync( cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.CustomFieldRetrieved));
    }
    [HttpGet("lookup/validation-rule")]
    public async Task<IActionResult> GetRuleLookUp([FromQuery] BasicFilterDto filter, CancellationToken cancellationToken)
    {
        var items = await _service.GetRuleLookUpAsync(filter, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.CustomFieldRetrieved));
    }
}
