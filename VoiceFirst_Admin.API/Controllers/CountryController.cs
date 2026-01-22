using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Business.Services;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Country;
using VoiceFirst_Admin.Utilities.DTOs.Features.Division;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.API.Controllers;


[ApiController]
public class CountryController : ControllerBase
{
    private readonly ICountryService _service;

    public CountryController(ICountryService service)
    {
        _service = service;
    }

    // 1) Get country list with filter/paging
    [HttpGet]
    [Route("api/country")]
    public async Task<IActionResult> GetAll([FromQuery] CountryFilterDto filter, CancellationToken cancellationToken)
    {
        var items = await _service.GetAllAsync(filter, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.CountryRetrieveSucessfully));
    }

    // 2) Get active countries only (no paging)
    [HttpGet]
    [Route("api/country/lookup")]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
    {
        var items = await _service.GetActiveAsync(cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.CountryRetrieveSucessfully));
    }

    [HttpGet]
    [Route("api/division/one")]
    public async Task<IActionResult> GetDivisionOneAll([FromQuery] DivisionOneFilterDto filter, CancellationToken cancellationToken)
    {
        var items = await _service.GetAllDivisionOneAsync(filter, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.DivisionOneRetrieveSucessfully));
    }
    [HttpGet]
    [Route("api/division/one/lookup/{countryId:int}")]
    public async Task<IActionResult> GetDivisionOneActiveByCountry(int countryId, CancellationToken cancellationToken)
    {
        var items = await _service.GetDivisionOneActiveByCountryIdAsync(countryId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.DivisionOneRetrieveSucessfully));
    }

    // Division Two

    [HttpGet]
    [Route("api/division/two")]
    public async Task<IActionResult> GetDivisionTwoAll([FromQuery] DivisionTwoFilterDto filter, CancellationToken cancellationToken)
    {
        var items = await _service.GetAllDivisionTwoAsync(filter, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.DivisionTwoRetrieveSucessfully));
    }
    [HttpGet]
    [Route("api/division/two/lookup/{divisionOneId:int}")]
    public async Task<IActionResult> GetDivisionTwoActiveByDivisionOne(int divisionOneId, CancellationToken cancellationToken)
    {
        var items = await _service.GetDivisionTwoActiveByDivisionOneIdAsync(divisionOneId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.DivisionTwoRetrieveSucessfully));
    }

    // Division Three

    [HttpGet]
    [Route("api/division/three")]
    public async Task<IActionResult> GetDivisionThreeAll([FromQuery] DivisionThreeFilterDto filter, CancellationToken cancellationToken)
    {
        var items = await _service.GetAllDivisionThreeAsync(filter, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.DivisionThreeRetrieveSucessfully));
    }
    [HttpGet]
    [Route("api/division/three/lookup/{divisionTwoId:int}")]
    public async Task<IActionResult> GetDivisionThreeActiveByDivisionTwo(int divisionTwoId, CancellationToken cancellationToken)
    {
        var items = await _service.GetDivisionThreeActiveByDivisionTwoIdAsync(divisionTwoId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.DivisionThreeRetrieveSucessfully));
    }
}
