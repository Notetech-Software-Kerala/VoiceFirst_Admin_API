using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.API.Controllers;


[ApiController]
public class PostOfficeController : ControllerBase
{
    private readonly IPostOfficeService _service;
    private static readonly int userId = 1; // Placeholder

    public PostOfficeController(IPostOfficeService service)
    {
        _service = service;
    }

    [HttpPost]
    [Route("api/post-office")]
    public async Task<IActionResult> Create([FromBody] PostOfficeCreateDto model, CancellationToken cancellationToken)
    {
        if (model == null) return BadRequest(ApiResponse<object>.Fail(Messages.PayloadRequired));

        var created = await _service.CreateAsync(model, userId, cancellationToken);
        return StatusCode(created.StatusCode, created);
    }

    [HttpGet]
    [Route("api/post-office/{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _service.GetByIdAsync(id, cancellationToken);
        if (item == null) return NotFound(ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound));
        return Ok(ApiResponse<PostOfficeDetailDto>.Ok(item, Messages.PostOfficeRetrieveSucessfully));
    }
    [HttpGet]
    [Route("api/zipcodes/lookup")]
    public async Task<IActionResult> GetZipCodesByPostOfficeIdAndPlaceIdAsync([FromQuery] GetPostOfficeIdZipCodeLookUpDto getZipCodeLookUpDto, CancellationToken cancellationToken)
    {
        var item = await _service.GetZipCodesByPostOfficeIdAsync(getZipCodeLookUpDto.PostOfficeId, getZipCodeLookUpDto.PlaceId, cancellationToken);
        return StatusCode(item.StatusCode, item);
    }
    [HttpGet]
    [Route("api/zipcodes/lookup/post-office-ids")]
    public async Task<IActionResult> GetZipCodesByPostOfficeIdsAndPlaceIdAsync([FromQuery] GetPostOfficeIdsZipCodeLookUpDto getZipCodeLookUpDto, CancellationToken cancellationToken)
    {
        var item = await _service.GetZipCodesByPostOfficeIdsAsync(getZipCodeLookUpDto.PostOfficeIds, getZipCodeLookUpDto.PlaceId, cancellationToken);
        return StatusCode(item.StatusCode, item);
    }

    [HttpGet]
    [Route("api/post-office")]
    public async Task<IActionResult> GetAll([FromQuery] PostOfficeFilterDto filter, CancellationToken cancellationToken)
    {
        var items = await _service.GetAllAsync(filter, cancellationToken);
        return StatusCode(items.StatusCode, items);
    }

    [HttpGet]
    [Route("api/post-office/lookup")]
    public async Task<IActionResult> GetLookupAsync([FromQuery] PostOfficeLookUpFilterDto filter, CancellationToken cancellationToken)
    {
        var response = await _service.GetLookupAsync(filter,cancellationToken);
       return StatusCode(response.StatusCode, response); 
    }
    [HttpGet]
    [Route("api/post-office-details/lookup")]
    public async Task<IActionResult> GetPostOfficeDetailsByZipCodeAsync([FromQuery] string zipCode, CancellationToken cancellationToken)
    {
        var response = await _service.GetPostOfficeDetailsByZipCodeAsync(zipCode, cancellationToken);
       return StatusCode(response.StatusCode, response); 
    }

    [HttpPatch]
    [Route("api/post-office/{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PostOfficeUpdateDto model, CancellationToken cancellationToken)
    {
        var res = await _service.UpdateAsync(model, id, userId, cancellationToken);
        return StatusCode(res.StatusCode, res);
    }

    [HttpDelete]
    [Route("api/post-office/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var res = await _service.DeleteAsync(id, userId, cancellationToken);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch]
    [Route("api/post-office/recover/{id:int}")]
    public async Task<IActionResult> Restore(int id, CancellationToken cancellationToken)
    {
        var res = await _service.RestoreAsync(id, userId, cancellationToken);
        return StatusCode(res.StatusCode, res);
    }
   
}
