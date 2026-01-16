using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SysBusinessActivityController : ControllerBase
    {
        private readonly ISysBusinessActivityService _service;
        const int userId = 1;
        public SysBusinessActivityController(ISysBusinessActivityService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SysBusinessActivityCreateDTO model, CancellationToken cancellationToken)
        {
            if (model == null) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired));

            // uniqueness check
            if (await _service.ExistsByNameAsync(model.Name, null, cancellationToken))
                return Conflict(ApiResponse<object>.Fail(Messages.SysBusinessActivityAlreadyExists, statusCode: 409));

            var created = await _service.CreateAsync(model, userId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.SysBusinessActivityId }, ApiResponse<SysBusinessActivityListItemDTO>.Ok(new SysBusinessActivityListItemDTO { }, Messages.SysBusinessActivityCreated));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _service.GetByIdAsync(id, cancellationToken);
            if (item == null) return NotFound(ApiResponse<object>.Fail(Messages.NotFound, 404));
            return Ok(ApiResponse<SysBusinessActivityDetailsDTO>.Ok(new SysBusinessActivityDetailsDTO {  }, Messages.Success));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CommonFilterDto filter, CancellationToken cancellationToken)
        {
            var items = await _service.GetAllAsync(filter, cancellationToken);
            return Ok(ApiResponse<object>.Ok(items, Messages.Success));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SysBusinessActivityUpdateDTO model, CancellationToken cancellationToken)
        {
            if (model == null || id == 0) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired));

            if (await _service.ExistsByNameAsync(model.Name ?? string.Empty, id, cancellationToken))
                return Conflict(ApiResponse<object>.Fail(Messages.SysBusinessActivityAlreadyExists, 409));

            var ok = await _service.UpdateAsync(model, id,userId, cancellationToken);
            if (!ok) return NotFound(ApiResponse<object>.Fail(Messages.NotFound, 404));
            return Ok(ApiResponse<object>.Ok(null!, Messages.Updated, StatusCodes.Status204NoContent));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken cancellationToken)
        {
            var success = await _service.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
            return Ok(ApiResponse<bool>.Ok(success, success ? "Business activity deleted" : "Business activity not found"));
        }
    }
}
