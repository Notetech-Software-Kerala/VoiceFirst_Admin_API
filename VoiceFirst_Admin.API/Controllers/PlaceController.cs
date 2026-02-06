using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Place;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaceController : ControllerBase
    {
        private readonly IPlaceService _service;
        const int userId = 1;

        public PlaceController(IPlaceService service)
        {
            _service = service;
        }

        /// <summary>
        /// Creates a new place.
        /// </summary>
        /// <remarks>
        /// Business rules:
        /// - place name must be unique .
        /// - Soft-deleted place can be recovered instead of creating a new one.
        /// </remarks>
        ///
        /// <response code="201">
        /// Place created successfully.
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
        /// Conflict. An place with the same name already exists and is active.
        /// </response>
        ///
        /// <response code="422">
        /// Unprocessable entity. place already exists but was deleted.
        /// Returns the existing placeId so it can be recovered.
        /// </response>
        ///
        /// <response code="500">
        /// Internal server error. An unexpected error occurred while processing the request.
        /// </response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PlaceDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<PlaceDTO>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseDescription(201, Messages.PlaceCreated)]
        [SwaggerResponseDescription(409, Messages.PlaceAlreadyExists)]
        [SwaggerResponseDescription(422, Messages.PlaceAlreadyExistsRecoverable)]
        [SwaggerResponseDescription(400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(401, Messages.Unauthorized)]
        [SwaggerResponseDescription(500, Messages.InternalServerError)]
        public async Task<IActionResult> CreateAsync(
            [FromBody] PlaceCreateDTO model,
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
        [ProducesResponseType(typeof(ApiResponse<PlaceDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseDescription(200, Messages.PlaceRetrieved)]
        [SwaggerResponseDescription(404, Messages.PlaceNotFoundById)]
        [SwaggerResponseDescription(400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(401, Messages.Unauthorized)]
        [SwaggerResponseDescription(500, Messages.InternalServerError)]

        public async Task<IActionResult>
            GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
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
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<PlaceDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [SwaggerResponseDescription(200, Messages.PlacesRetrieved)]
        [SwaggerResponseDescription(401, Messages.Unauthorized)]
        [SwaggerResponseDescription(500, Messages.InternalServerError)]
        public async Task<IActionResult> GetAllAsync(
          [FromQuery] PlaceFilterDTO filter,
          CancellationToken cancellationToken)
        {

            var response = await _service.GetAllAsync(filter, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }



        [HttpGet("lookup")]
        [ProducesResponseType(typeof(ApiResponse<List<PlaceLookUpDTO?>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseDescription(200, Messages.PlacesRetrieved)]
        [SwaggerResponseDescription(401, Messages.Unauthorized)]
        [SwaggerResponseDescription(500, Messages.InternalServerError)]
        public async Task<IActionResult> GetLookUpAsync(

          CancellationToken cancellationToken)
        {
            var response = await _service.GetLookUpAsync(cancellationToken);
            return StatusCode(response.StatusCode, response);
        }




        /// <summary>
        /// Updates a place.
        /// </summary>
        /// <remarks>
        /// Business rules:
        /// - The place name must be unique.
        /// - If the activity is soft-deleted, it may be restored instead of updated.
        /// </remarks>
        ///
        /// <response code="200">
        /// place updated successfully and the updated data is returned.
        /// </response>
        ///
        /// <response code="204">
        /// No changes were detected. The place already contains the provided values.
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
        /// Conflict. An place with the same name already exists and is active.
        /// </response>
        ///
        /// <response code="422">
        /// Unprocessable entity. place already exists but was deleted.
        /// Returns the existing PlaceId so it can be recovered.
        /// </response>
        ///
        /// <response code="500">
        /// Internal server error. An unexpected error occurred while processing the request.
        /// </response>
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<PlaceDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<PlaceDTO>), StatusCodes.Status422UnprocessableEntity)]
        [SwaggerResponseDescription(200, Messages.PlaceUpdated)]
        [SwaggerResponseDescription(204, Messages.Updated)]
        [SwaggerResponseDescription(400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(404, Messages.PlaceNotFoundById)]
        [SwaggerResponseDescription(409, Messages.PlaceAlreadyExists)]
        [SwaggerResponseDescription(422, Messages.PlaceAlreadyExistsRecoverable)]
        [SwaggerResponseDescription(401, Messages.Unauthorized)]
        [SwaggerResponseDescription(500, Messages.InternalServerError)]
        public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] PlaceUpdateDTO model,
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
            return StatusCode(response.StatusCode, response);
        }




        [HttpPatch("recover/{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<PlaceDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseDescription(200, Messages.PlaceRecovered)]
        [SwaggerResponseDescription(404, Messages.PlaceNotFoundById)]
        [SwaggerResponseDescription(400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(409, Messages.PlaceAlreadyRecovered)]
        [SwaggerResponseDescription(401, Messages.Unauthorized)]
        [SwaggerResponseDescription(500, Messages.InternalServerError)]

        public async Task<IActionResult> RecoverAsync(
            int id,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    Messages.PayloadRequired,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.Payload
                ));
            }
            var recoveredDto = await _service.RecoverAsync
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
        [SwaggerResponseDescription(200, Messages.PlaceDeleted)]
        [SwaggerResponseDescription(404, Messages.PlaceNotFoundById)]
        [SwaggerResponseDescription(400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(409, Messages.PlaceAlreadyDeleted)]
        [SwaggerResponseDescription(401, Messages.Unauthorized)]
        [SwaggerResponseDescription(500, Messages.InternalServerError)]
        public async Task<IActionResult>
            DeleteAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    Messages.PayloadRequired,
                    StatusCodes.Status400BadRequest,
                    error: ErrorCodes.Payload
                ));
            }
            var recoveredDto = await _service.DeleteAsync(id, userId, cancellationToken);
            return StatusCode(recoveredDto.StatusCode, recoveredDto);
        }

    }
}
