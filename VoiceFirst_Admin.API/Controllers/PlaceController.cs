using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.API.Security;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Place;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Constants.Swagger;

namespace VoiceFirst_Admin.API.Controllers
{
    [Route("api/place")]
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
        [SwaggerResponseDescription(StatusCodes.Status201Created, Description.PLACE_CREATED, Messages.PlaceCreated)]
        [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.PlaceAlreadyExists)]
        [SwaggerResponseDescription(StatusCodes.Status422UnprocessableEntity, Description.UNPROCESSABLE_422, Messages.PlaceAlreadyExistsRecoverable)]
        [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
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
        [SwaggerResponseDescription(StatusCodes.Status200OK, Description.PLACE_RETRIEVED, Messages.PlaceRetrieved)]
        [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.PlaceNotFoundById)]
        [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]

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
        [SwaggerResponseDescription(StatusCodes.Status200OK, Description.PLACES_RETRIEVED, Messages.PlacesRetrieved)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
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
        [SwaggerResponseDescription(StatusCodes.Status200OK, Description.PLACES_RETRIEVED, Messages.PlacesRetrieved)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
        public async Task<IActionResult> GetLookUpAsync(
          [FromQuery] int zipCodeId,
          CancellationToken cancellationToken)
        {
            var response = await _service.GetLookUpAsync(zipCodeId, cancellationToken);
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
        [SwaggerResponseDescription(StatusCodes.Status200OK, Description.PLACE_UPDATED, Messages.PlaceUpdated)]
        [SwaggerResponseDescription(StatusCodes.Status204NoContent, Description.PLACE_UPDATED, Messages.Updated)]
        [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.PlaceNotFoundById)]
        [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.PlaceAlreadyExists)]
        [SwaggerResponseDescription(StatusCodes.Status422UnprocessableEntity, Description.UNPROCESSABLE_422, Messages.PlaceAlreadyExistsRecoverable)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
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




        [AuthorizeAdmin]
        [HttpPatch("recover/{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<PlaceDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseDescription(StatusCodes.Status200OK, Description.PLACE_RECOVERED, Messages.PlaceRecovered)]
        [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.PlaceNotFoundById)]
        [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.PlaceAlreadyRecovered)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
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



        [AuthorizeAdmin]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseDescription(StatusCodes.Status200OK, Description.PLACE_DELETED, Messages.PlaceDeleted)]
        [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.PlaceNotFoundById)]
        [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.PlaceAlreadyDeleted)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
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
