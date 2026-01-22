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

[Route("api/country")]
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
    public async Task<IActionResult> GetAll([FromQuery] CountryFilterDto filter, CancellationToken cancellationToken)
    {
        var items = await _service.GetAllAsync(filter, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.ProgramActionCreatedRetrieveSucessfully));
    }

    // 2) Get active countries only (no paging)
    [HttpGet("lookup")]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
    {
        var items = await _service.GetActiveAsync(cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.ProgramActionCreatedRetrieveSucessfully));
    }

    [HttpGet("one")]
    public async Task<IActionResult> GetDivisionOneAll([FromQuery] DivisionOneFilterDto filter, CancellationToken cancellationToken)
    {
        var items = await _service.GetAllDivisionOneAsync(filter, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.ProgramActionCreatedRetrieveSucessfully));
    }

    [HttpGet("one/lookup/{countryId:int}")]
    public async Task<IActionResult> GetDivisionOneActiveByCountry(int countryId, CancellationToken cancellationToken)
    {
        var items = await _service.GetDivisionOneActiveByCountryIdAsync(countryId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.ProgramActionCreatedRetrieveSucessfully));
    }

    // Division Two
    [HttpGet("two")]
    public async Task<IActionResult> GetDivisionTwoAll([FromQuery] DivisionTwoFilterDto filter, CancellationToken cancellationToken)
    {
        var items = await _service.GetAllDivisionTwoAsync(filter, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.ProgramActionCreatedRetrieveSucessfully));
    }

    [HttpGet("two/lookup/{divisionOneId:int}")]
    public async Task<IActionResult> GetDivisionTwoActiveByDivisionOne(int divisionOneId, CancellationToken cancellationToken)
    {
        var items = await _service.GetDivisionTwoActiveByDivisionOneIdAsync(divisionOneId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.ProgramActionCreatedRetrieveSucessfully));
    }

    // Division Three
    [HttpGet("three")]
    public async Task<IActionResult> GetDivisionThreeAll([FromQuery] DivisionThreeFilterDto filter, CancellationToken cancellationToken)
    {
        var items = await _service.GetAllDivisionThreeAsync(filter, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.ProgramActionCreatedRetrieveSucessfully));
    }

    [HttpGet("three/lookup/{divisionTwoId:int}")]
    public async Task<IActionResult> GetDivisionThreeActiveByDivisionTwo(int divisionTwoId, CancellationToken cancellationToken)
    {
        var items = await _service.GetDivisionThreeActiveByDivisionTwoIdAsync(divisionTwoId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.ProgramActionCreatedRetrieveSucessfully));
    }
}
