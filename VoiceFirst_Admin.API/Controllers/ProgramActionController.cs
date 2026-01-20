using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.API.Controllers
{
    [Route("api/program-action")]
    [ApiController]
    public class ProgramActionController : ControllerBase
    {
        private readonly IProgramActionService _service;
        private readonly static int userId= 1; // Placeholder for authenticated user ID
        public ProgramActionController(IProgramActionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProgramActionCreateDto model, CancellationToken cancellationToken)
        {
            if (model == null) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired));
     
            var created = await _service.CreateAsync(model, userId, cancellationToken);

            return StatusCode(created.StatusCode, created);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _service.GetByIdAsync(id, cancellationToken);
            if (item == null) return NotFound(ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound));
            return Ok(ApiResponse<ProgramActionDto>.Ok(item, Messages.ProgramActionCreatedRetrieveSucessfully));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CommonFilterDto filter, CancellationToken cancellationToken)
        {
            var items = await _service.GetAllAsync(filter, cancellationToken);
            return Ok(ApiResponse<object>.Ok(items, Messages.ProgramActionCreatedRetrieveSucessfully));
        }
        [HttpGet("lookup")]
        public async Task<IActionResult> GetLookupAsync( CancellationToken cancellationToken)
        {
            var items = await _service.GetLookupAsync( cancellationToken);
            return Ok(ApiResponse<object>.Ok(items, Messages.ProgramActionCreatedRetrieveSucessfully));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProgramActionUpdateDto model, CancellationToken cancellationToken)
        {
            var res = await _service.UpdateAsync( model,id, userId, cancellationToken);
            return StatusCode(res.StatusCode, res);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {

            var res = await _service.DeleteAsync(id, userId, cancellationToken);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("recover/{id:int}")]
        public async Task<IActionResult> Restore(int id, CancellationToken cancellationToken)
        {

            var res = await _service.RestoreAsync(id, userId, cancellationToken);
            return StatusCode(res.StatusCode, res);
        }


    }
}
