using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Features.Users;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

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
        //[SwaggerResponseDescription(201, Messages.EmployeeCreated)]
        //[SwaggerResponseDescription(400, Messages.PayloadRequired)]
        //[SwaggerResponseDescription(401, Messages.Unauthorized)]
        //[SwaggerResponseDescription(404, Messages.CountryNotFound)]
        //[SwaggerResponseDescription(409, Messages.EmployeeEmailAlreadyExists)]
        //[SwaggerResponseDescription(409, Messages.EmployeeMobileNoAlreadyExists)]
        //[SwaggerResponseDescription(422, Messages.EmployeeEmailAlreadyExistsRecoverable)]
        //[SwaggerResponseDescription(422, Messages.EmployeeMobileNoAlreadyExistsRecoverable)]
        //[SwaggerResponseDescription(500, Messages.InternalServerError)]
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
        //[SwaggerResponseDescription(200, Messages.EmployeeRetrieved)]
        //[SwaggerResponseDescription(400, Messages.PayloadRequired)]
        //[SwaggerResponseDescription(401, Messages.Unauthorized)]
        //[SwaggerResponseDescription(404, Messages.EmployeeNotFoundById)]
        //[SwaggerResponseDescription(500, Messages.InternalServerError)]
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



        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<EmployeeDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        //[SwaggerResponseDescription(200, Messages.EmployeeDeleted)]
        //[SwaggerResponseDescription(400, Messages.PayloadRequired)]
        //[SwaggerResponseDescription(401, Messages.Unauthorized)]
        //[SwaggerResponseDescription(404, Messages.EmployeeNotFoundById)]
        //[SwaggerResponseDescription(409, Messages.EmployeeAlreadyDeleted)]
        //[SwaggerResponseDescription(500, Messages.InternalServerError)]
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



        [HttpPatch("recover/{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<EmployeeDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        //[SwaggerResponseDescription(200, Messages.EmployeeRecovered)]
        //[SwaggerResponseDescription(400, Messages.PayloadRequired)]
        //[SwaggerResponseDescription(401, Messages.Unauthorized)]
        //[SwaggerResponseDescription(404, Messages.EmployeeNotFoundById)]
        //[SwaggerResponseDescription(409, Messages.EmployeeAlreadyRecovered)]
        //[SwaggerResponseDescription(500, Messages.InternalServerError)]
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
        //[SwaggerResponseDescription(200, Messages.EmployeeRetrieved)]
        //[SwaggerResponseDescription(401, Messages.Unauthorized)]
        //[SwaggerResponseDescription(500, Messages.InternalServerError)]
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
        //[SwaggerResponseDescription(200, Messages.EmployeeUpdated)]
        //[SwaggerResponseDescription(204, Messages.Updated)]
        //[SwaggerResponseDescription(400, Messages.PayloadRequired)]
        //[SwaggerResponseDescription(404, Messages.EmployeeNotFoundById)]
        //[SwaggerResponseDescription(409, Messages.EmployeeEmailAlreadyExists)]
        //[SwaggerResponseDescription(409, Messages.EmployeeMobileNoAlreadyExists)]
        //[SwaggerResponseDescription(422, Messages.EmployeeEmailAlreadyExistsRecoverable)]
        //[SwaggerResponseDescription(422, Messages.EmployeeMobileNoAlreadyExistsRecoverable)]
        //[SwaggerResponseDescription(401, Messages.Unauthorized)]
        //[SwaggerResponseDescription(500, Messages.InternalServerError)]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] EmployeeUpdateDto model, CancellationToken cancellationToken)
        {
            if (model == null)
                return BadRequest(ApiResponse<object>.Fail(
                   Messages.PayloadRequired,
                   StatusCodes.Status400BadRequest,
                   ErrorCodes.Payload
                   ));

            var result = await _service.UpdateAsync
                (id, model, userId, cancellationToken);

            return StatusCode(result.StatusCode, result);
        }

    }
}
