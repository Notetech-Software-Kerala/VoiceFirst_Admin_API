using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VoiceFirst_Admin.Business.Contracts.IServices;
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
        public async Task<ActionResult<ApiResponse<int>>>
            Create([FromBody] SysBusinessActivityCreateDTO dto,
            CancellationToken cancellationToken)
        {
            var id = await _service.CreateAsync
                (dto, userId,cancellationToken).ConfigureAwait(false);
            return Ok(ApiResponse<int>.Ok(id, "Business activity created"));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable
            <SysBusinessActivityUpdateDTO>>>> 
            GetAll([FromQuery] CommonFilterDto filter, CancellationToken cancellationToken)
        {
            var items = await _service.GetAllAsync(filter, cancellationToken).ConfigureAwait(false);
            return Ok(ApiResponse<IEnumerable<SysBusinessActivityUpdateDTO>>.Ok(items, "Business activities fetched"));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<SysBusinessActivityUpdateDTO?>>> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _service.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
            return Ok(ApiResponse<SysBusinessActivityUpdateDTO?>.Ok(item, "Business activity fetched"));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(int id, [FromBody] SysBusinessActivityUpdateDTO dto, CancellationToken cancellationToken)
        {
            var success = await _service.UpdateAsync(id, dto, cancellationToken).ConfigureAwait(false);
            return Ok(ApiResponse<bool>.Ok(success, success ? "Business activity updated" : "Business activity not found"));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken cancellationToken)
        {
            var success = await _service.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
            return Ok(ApiResponse<bool>.Ok(success, success ? "Business activity deleted" : "Business activity not found"));
        }
    }
}
