using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.Constants.Swagger;
using VoiceFirst_Admin.Utilities.DTOs.Features.ApplicationVersion;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.API.Controllers;

[Route("api/application-version")]
[ApiController]
public class PlatformVersionController : ControllerBase
{
    private readonly IApplicationVersionService _service;
    const int userId = 1;

    public PlatformVersionController(IApplicationVersionService service)
    {
        _service = service;
    }

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse<PlatformVersionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseDescription(StatusCodes.Status201Created, Description.APPLICATION_VERSION_CREATED, Messages.ApplicationVersionCreated)]
    [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
    [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.ApplicationNotFoundById)]
    [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.ApplicationVersionAlreadyExists)]
    [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] PlatformVersionCreateDto model,
        CancellationToken cancellationToken)
    {
        if (model == null)
        {
            return BadRequest(ApiResponse<object>.Fail(
                Messages.PayloadRequired,
                StatusCodes.Status400BadRequest,
                ErrorCodes.Payload));
        }

        var result = await _service.CreateAsync(model, userId, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
