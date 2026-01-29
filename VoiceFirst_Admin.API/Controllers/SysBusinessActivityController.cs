using Azure;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
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

    /// <summary>
    /// Creates a new business activity.
    /// </summary>
    /// <remarks>
    /// Business rules:
    /// - Activity name must be unique .
    /// - Soft-deleted activities can be recovered instead of creating a new one.
    /// </remarks>
    ///
    /// <response code="201">
    /// Business activity created successfully.
    /// </response>
    ///
    /// <response code="400">
    /// Invalid request. Payload is null or validation failed.
    /// </response>
    ///
    /// <response code="401">
    /// Unauthorized. User is not authenticated.
    /// </response>
    ///
    /// <response code="409">
    /// Conflict. An activity with the same name already exists and is active.
    /// </response>
    ///
    /// <response code="422">
    /// Unprocessable entity. Activity already exists but was deleted.
    /// Returns the existing BusinessActivityId so it can be recovered.
    /// </response>
    ///
    /// <response code="500">
    /// Internal server error. An unexpected error occurred while processing the request.
    /// </response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SysBusinessActivityDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<SysBusinessActivityDTO>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] SysBusinessActivityCreateDTO model,
        CancellationToken cancellationToken)
    {
        // 1️⃣ Null payload check
        if (model == null)
        {
            return BadRequest(ApiResponse<object>.Fail(
                message: Messages.PayloadRequired,
                statusCode: StatusCodes.Status400BadRequest,
                error: ErrorCodes.Payload
            ));
        }
     
        // 3️⃣ Delegate business logic to service
        var response = await _service.CreateAsync(
            model,
            userId,
            cancellationToken);

        // 4️⃣ Return standardized response
        return StatusCode(response.StatusCode, response);
    }




    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SysBusinessActivityDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    
    public async Task<IActionResult> 
        GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if ( id <= 0)
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
    [ProducesResponseType(typeof(ApiResponse<PagedResultDto<SysBusinessActivityDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(
      [FromQuery] BusinessActivityFilterDTO filter,
      CancellationToken cancellationToken)
    {

        var response = await _service.GetAllAsync(filter, cancellationToken);

        return StatusCode(response.StatusCode, response);
    }



    [HttpGet("lookup")]
    [ProducesResponseType(typeof(ApiResponse<List<SysBusinessActivityActiveDTO?>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetActiveAsync(
     
      CancellationToken cancellationToken)
    {
        var response = await _service.GetActiveAsync(cancellationToken);
        return StatusCode(response.StatusCode, response);
    }




    /// <summary>
    /// Updates a business activity.
    /// </summary>
    /// <remarks>
    /// Business rules:
    /// - The activity name must be unique.
    /// - If the activity is soft-deleted, it may be restored instead of updated.
    /// </remarks>
    ///
    /// <response code="200">
    /// Business activity updated successfully and the updated data is returned.
    /// </response>
    ///
    /// <response code="204">
    /// No changes were detected. The business activity already contains the provided values.
    /// </response>
    /// 
    /// <response code="400">
    /// Invalid request. Payload is null or validation failed.
    /// </response>
    /// 
    /// <response code="404">
    /// Id nof found.
    /// </response>
    ///
    /// <response code="401">
    /// Unauthorized. User is not authenticated.
    /// </response>
    ///
    /// <response code="409">
    /// Conflict. An activity with the same name already exists and is active.
    /// </response>
    ///
    /// <response code="422">
    /// Unprocessable entity. Activity already exists but was deleted.
    /// Returns the existing BusinessActivityId so it can be recovered.
    /// </response>
    ///
    /// <response code="500">
    /// Internal server error. An unexpected error occurred while processing the request.
    /// </response>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SysBusinessActivityDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResponse<SysBusinessActivityDTO>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateAsync(
    int id,
    [FromBody] SysBusinessActivityUpdateDTO model,
    CancellationToken cancellationToken)
    {
        if (model == null || id <= 0)
        {
            return BadRequest(ApiResponse<object>.Fail(
                Messages.PayloadRequired,
                StatusCodes.Status400BadRequest,
                ErrorCodes.Payload
            ));
        }

        var response = await _service.UpdateAsync(
            model, id, userId, cancellationToken);
        return StatusCode(response.StatusCode,response);
    }




    [HttpPatch("recover/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SysBusinessActivityDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> RecoverAsync(
        int id,
        CancellationToken cancellationToken)
    {
        if ( id <= 0)
        {
            return BadRequest(ApiResponse<object>.Fail(
                Messages.PayloadRequired,
                StatusCodes.Status400BadRequest,
                ErrorCodes.Payload
            ));
        }
        var recoveredDto = await _service.RecoverBusinessActivityAsync
            (id, userId, cancellationToken);
        return StatusCode(recoveredDto.StatusCode, recoveredDto);
    }



    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult>
        DeleteAsync(int id, CancellationToken cancellationToken)
    {
        if ( id <= 0)
        {
            return BadRequest(ApiResponse<object>.Fail(
                Messages.PayloadRequired,
                StatusCodes.Status400BadRequest,
                error: ErrorCodes.Payload
            ));
        }
        var recoveredDto =  await _service.DeleteAsync(id, userId, cancellationToken);
        return StatusCode(recoveredDto.StatusCode, recoveredDto);
    }


}
