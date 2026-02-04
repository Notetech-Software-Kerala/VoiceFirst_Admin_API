using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.API.Controllers
{
    [Route("api/program")]
    [ApiController]
    public class SysProgramController : ControllerBase
    {
        private readonly ISysProgramService _service;
        private static readonly int userId = 1; 

        public SysProgramController(ISysProgramService service)
        {
            _service = service;
        }

        /// <summary>
        /// Creates a new system program.
        /// </summary>
        /// <remarks>
        /// Business rules:
        /// - Program name must be unique per application.
        /// - Program route must be unique per application.
        /// - Soft-deleted programs can be restored instead of creating a new one.
        /// </remarks>
        ///
        /// <response code="201">
        /// Program created successfully.
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
        /// Conflict. A program with the same name or route already exists and is active.
        /// </response>
        ///
        /// <response code="404">
        /// Platform or actions not found .
        /// </response>
        ///
        /// <response code="422">
        /// Unprocessable entity. Program already exists but was deleted.
        /// </response>
        ///
        /// <response code="500">
        /// Internal server error. An unexpected error occurred while processing the request.
        /// </response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<SysProgramDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<SysProgramDto>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(
            [FromBody] SysProgramCreateDTO model,
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
        [ProducesResponseType(typeof(ApiResponse<SysProgramDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(typeof(ApiResponse<SysBusinessActivityDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
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
            if (id <= 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    Messages.PayloadRequired,
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.Payload
                ));
            }
            var recoveredDto = await _service.RecoverProgramAsync
                (id, userId, cancellationToken);
            return StatusCode(recoveredDto.StatusCode, recoveredDto);
          
        }



        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<SysProgramDto>>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(
          [FromQuery] SysProgramFilterDTO filter,
          CancellationToken cancellationToken)
        {
            var response = await _service.GetAllAsync(filter, cancellationToken);
            return StatusCode(response.StatusCode, response);            
        }


       
        [HttpGet("lookup")]
        [ProducesResponseType(typeof(ApiResponse<List<SysProgramLookUp?>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProgramLookupAsync(CancellationToken cancellationToken)
        {
            var result = await _service.GetProgramLookupAsync(cancellationToken);
            return StatusCode(result.StatusCode, result);
        }




        [HttpGet("active-by-application/{applicationId:int}")]
        [ProducesResponseType(typeof(ApiResponse<SysProgramByApplicationIdDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllActiveByApplicationIdAsync(int applicationId, CancellationToken cancellationToken)
        {
            var result = await _service.GetAllActiveByApplicationIdAsync(applicationId, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("for-plan")]
        [ProducesResponseType(typeof(ApiResponse<SysProgramByApplicationIdDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllActiveForPlanAsync(CancellationToken cancellationToken)
        {
            var result = await _service.GetAllActiveForPlanAsync(cancellationToken);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet("action-lookup/{programId:int}")]
        [ProducesResponseType(typeof(ApiResponse<List<SysProgramActionLinkLookUp>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActionLookupByProgramIdAsync(int programId, CancellationToken cancellationToken)
        {
            var result = await _service.GetActionLookupByProgramIdAsync(programId, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }



        /// <summary>
        /// Updates  programs.
        /// </summary>
        /// <remarks>
        /// Business rules:
        /// - The program name ,route,label,actions must be unique.
        /// - If the program is soft-deleted, it may be restored instead of updated.
        /// </remarks>
        ///
        /// <response code="200">
        /// program updated successfully and the updated data is returned.
        /// </response>
        ///
        /// <response code="204">
        /// No changes were detected. The program already contains the provided values.
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
        /// Conflict. An program with the same name,route,label already exists and is active.
        /// </response>
        ///
        /// <response code="422">
        /// Unprocessable entity. program already exists but was deleted.
        /// Returns the existing programId so it can be recovered.
        /// </response>
        ///
        /// <response code="500">
        /// Internal server error. An unexpected error occurred while processing the request.
        /// </response>


        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<SysProgramDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<SysProgramDto>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody]SysProgramUpdateDTO model, CancellationToken cancellationToken)
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







        //[HttpGet("lookup")]
        //public async Task<IActionResult> GetActiveAsync(

        //  CancellationToken cancellationToken)
        //{
        //    var result = await _service.GetActiveAsync(cancellationToken);
        //    return StatusCode(result.StatusCode, result);
        //}









    }
}
