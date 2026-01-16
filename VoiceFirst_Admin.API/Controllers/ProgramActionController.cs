using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;
using VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction;
using VoiceFirst_Admin.Utilities.Constants;

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

            // uniqueness check
            if (await _service.ExistsByNameAsync(model.ProgramActionName, null, cancellationToken))
                return Conflict(ApiResponse<object>.Fail(Messages.ProgramActionNameAlreadyExists, statusCode: StatusCodes.Status409Conflict));

            var created = await _service.CreateAsync(model, userId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.SysProgramActionId }, ApiResponse<SysProgramActions>.Ok(created, Messages.Created));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _service.GetByIdAsync(id, cancellationToken);
            if (item == null) return NotFound(ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound));
            return Ok(ApiResponse<ProgramActionDto>.Ok(item, Messages.Success));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CommonFilterDto filter, CancellationToken cancellationToken)
        {
            var items = await _service.GetAllAsync(filter, cancellationToken);
            return Ok(ApiResponse<object>.Ok(items, Messages.Success));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProgramActionUpdateDto model, CancellationToken cancellationToken)
        {
            if (model == null || model.ActionId != id) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired));

            if (await _service.ExistsByNameAsync(model.ActionName ?? string.Empty, id, cancellationToken))
                return Conflict(ApiResponse<object>.Fail(Messages.ProgramActionNameAlreadyExists, StatusCodes.Status409Conflict));

            var ok = await _service.UpdateAsync(model, userId, cancellationToken);
            if (!ok) return NotFound(ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound));
            return Ok(ApiResponse<object>.Ok(null!, Messages.Updated, StatusCodes.Status204NoContent));
        }


    }
}
