using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

[ApiController]
[Route("api/business-activity")]
public class SysBusinessActivityController : ControllerBase
{
    private readonly ISysBusinessActivityService _service;
    const int userId = 1;

    public SysBusinessActivityController(ISysBusinessActivityService service)
    {
        _service = service;
    }


    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromBody] SysBusinessActivityCreateDTO model,
        CancellationToken cancellationToken)
    {
        if (model == null)
            return BadRequest(ApiResponse<SysBusinessActivityDTO?>.
                Fail(Messages.PayloadRequired,
               StatusCodes.Status400BadRequest,
               ErrorCodes.Payload));
            
        var apiResponse = await _service.
            CreateAsync(model, userId, cancellationToken);
      
        return StatusCode(apiResponse.StatusCode,
            apiResponse);

    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> 
        GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var item = await _service.GetByIdAsync(id, cancellationToken);

        return Ok(ApiResponse<SysBusinessActivityDTO?>.Ok(
            item,
            Messages.BusinessActivityRetrieved));
    }



    [HttpGet("lookup")]
    public async Task<IActionResult> GetActiveAsync(
     
      CancellationToken cancellationToken)
    {
        var items = await _service.GetActiveAsync(cancellationToken);
        return Ok(ApiResponse<object>.Ok(items,
            Messages.BusinessActivityRetrieved));
    }


    [HttpGet]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] BusinessActivityFilterDTO filter,
        CancellationToken cancellationToken)
    {
        
        var items = await _service.GetAllAsync(filter, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, 
            Messages.BusinessActivityRetrieved));
    }

  
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] SysBusinessActivityUpdateDTO model,
        CancellationToken cancellationToken)
    {
        if (model == null || id == 0)
            return Ok(ApiResponse<object>.Fail(
                Messages.PayloadRequired,
                Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest));

        var updatedDto = await _service.UpdateAsync(model, id, userId, cancellationToken);

        return Ok(ApiResponse<object>.Ok(updatedDto, 
            Messages.BusinessActivityUpdated));
    }

    [HttpPatch("recover/{id:int}")]
    public async Task<IActionResult> RecoverAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var recoveredDto = await _service.RecoverBusinessActivityAsync(id, userId, cancellationToken);
        return StatusCode(recoveredDto.StatusCode, recoveredDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult>
        DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, userId, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(true,
            Messages.BusinessActivityDeleted));
    }
}
