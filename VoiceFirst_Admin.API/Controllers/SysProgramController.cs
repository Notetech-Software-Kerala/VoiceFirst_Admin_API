using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
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

        [HttpPost]
        public async Task<IActionResult> 
            Create([FromBody] SysProgramCreateDTO model, 
            CancellationToken cancellationToken)
        {
            if (model == null)
                return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired));
          

            var result = await _service.CreateAsync(model, userId, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _service.GetByIdAsync(id, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {

            var result = await _service.DeleteAsync(id,userId, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPatch("recover/{id:int}")]
        public async Task<IActionResult> RecoverAsync(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _service.RecoverProgramAsync(id, userId, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }


        //[HttpGet("lookup")]
        //public async Task<IActionResult> GetActiveAsync(

        //  CancellationToken cancellationToken)
        //{
        //    var result = await _service.GetActiveAsync(cancellationToken);
        //    return StatusCode(result.StatusCode, result);
        //}

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramFilterDTO filter,
            CancellationToken cancellationToken)
        {
            var items = await _service.GetAllAsync(filter, cancellationToken);
            return Ok(ApiResponse<object>.Ok(items, Messages.ProgramRetrieved));
        }

        [HttpGet("active-by-application/{applicationId:int}")]
        public async Task<IActionResult> GetAllActiveByApplicationIdAsync(int applicationId, CancellationToken cancellationToken)
        {
            var result = await _service.GetAllActiveByApplicationIdAsync(applicationId, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramUpdateDTO model, CancellationToken cancellationToken)
        {
            if (model == null)
                return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired));

            var result = await _service.UpdateAsync(id, model, userId, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> GetProgramLookupAsync(CancellationToken cancellationToken)
        {
            var result = await _service.GetProgramLookupAsync(cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{programId:int}/action-lookup")]
        public async Task<IActionResult> GetActionLookupByProgramIdAsync(int programId, CancellationToken cancellationToken)
        {
            var result = await _service.GetActionLookupByProgramIdAsync(programId, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }
    }
}
