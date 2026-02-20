using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.API.Security;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Users;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Constants.Swagger;

namespace VoiceFirst_Admin.API.Controllers
{
    [Route("api/employee")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        const int userId = 1;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<EmployeeDetailDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<EmployeeDetailDto>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseDescription(StatusCodes.Status201Created, Description.USER_CREATED, Messages.EmployeeCreated)]
        [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.DialCodeNotFound)]
        [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.EmployeeEmailAlreadyExists)]
        [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.EmployeeMobileNoAlreadyExists)]
        [SwaggerResponseDescription(StatusCodes.Status422UnprocessableEntity, Description.UNPROCESSABLE_422, Messages.EmployeeEmailAlreadyExistsRecoverable)]
        [SwaggerResponseDescription(StatusCodes.Status422UnprocessableEntity, Description.UNPROCESSABLE_422, Messages.EmployeeMobileNoAlreadyExistsRecoverable)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
        public async Task<IActionResult> Create(
         [FromBody] EmployeeCreateDto model,
         CancellationToken cancellationToken)
        {
            if (model == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    Messages.PayloadRequired,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.Payload
                ));
            }

            var apiResponse = await _service.CreateAsync(
                model,
                1, // ApplicationId
                userId,
                cancellationToken
            );

            return StatusCode(apiResponse.StatusCode, apiResponse);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<EmployeeDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseDescription(StatusCodes.Status200OK, Description.USER_RETRIEVED, Messages.EmployeeRetrieved)]
        [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.EmployeeNotFoundById)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
        public async Task<IActionResult>
            GetByIdAsync([FromRoute] int id,
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
            var result = await _service.GetByIdAsync(id, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }



        [AuthorizeAdmin]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<EmployeeDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseDescription(StatusCodes.Status200OK, Description.USER_DELETED, Messages.EmployeeDeleted)]
        [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.EmployeeNotFoundById)]
        [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.USER_ALREADY_DELETED_409, Messages.EmployeeAlreadyDeleted)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
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



        [AuthorizeAdmin]
        [HttpPatch("recover/{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<EmployeeDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseDescription(StatusCodes.Status200OK, Description.USER_RECOVERED, Messages.EmployeeRecovered)]
        [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.EmployeeNotFoundById)]
        [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.USER_ALREADY_RECOVERED_409, Messages.EmployeeAlreadyRecovered)]
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



        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<EmployeeDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseDescription(StatusCodes.Status200OK, Description.USERS_RETRIEVED, Messages.EmployeeRetrieved)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
        public async Task<IActionResult> GetAllAsync(
          [FromQuery] EmployeeFilterDto filter,
          CancellationToken cancellationToken)
        {
            var response = await _service.GetAllAsync(filter,userId, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }




        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<EmployeeDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<EmployeeDetailDto>), StatusCodes.Status422UnprocessableEntity)]
        [SwaggerResponseDescription(StatusCodes.Status200OK, Description.USER_UPDATED, Messages.EmployeeUpdated)]
        [SwaggerResponseDescription(StatusCodes.Status204NoContent, Description.USER_UPDATED, Messages.Updated)]
        [SwaggerResponseDescription(StatusCodes.Status400BadRequest, Description.BADREQUEST_400, Messages.PayloadRequired)]
        [SwaggerResponseDescription(StatusCodes.Status404NotFound, Description.NOTFOUND_404, Messages.EmployeeNotFoundById)]
        [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.EmployeeEmailAlreadyExists)]
        [SwaggerResponseDescription(StatusCodes.Status409Conflict, Description.CONFLICT_409, Messages.EmployeeMobileNoAlreadyExists)]
        [SwaggerResponseDescription(StatusCodes.Status422UnprocessableEntity, Description.UNPROCESSABLE_422, Messages.EmployeeEmailAlreadyExistsRecoverable)]
        [SwaggerResponseDescription(StatusCodes.Status422UnprocessableEntity, Description.UNPROCESSABLE_422, Messages.EmployeeMobileNoAlreadyExistsRecoverable)]
        [SwaggerResponseDescription(StatusCodes.Status401Unauthorized, Description.UNAUTHORIZED_401, Messages.Unauthorized)]
        [SwaggerResponseDescription(StatusCodes.Status500InternalServerError, Description.SERVERERROR_500, Messages.InternalServerError)]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] EmployeeUpdateDto model, CancellationToken cancellationToken)
        {
            if (model == null)
                return BadRequest(ApiResponse<object>.Fail(
                   Messages.PayloadRequired,
                   StatusCodes.Status400BadRequest,
                   ErrorCodes.Payload
                   ));

            var result = await _service.UpdateAsync
                (id,1, model, userId, cancellationToken);

            return StatusCode(result.StatusCode, result);
        }

    }
}
