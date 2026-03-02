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
    public async Task<IActionResult> GetActive([FromQuery] BasicFilterDto filter,CancellationToken cancellationToken)
    {
        var items = await _service.GetActiveAsync(filter, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.CountryRetrieveSucessfully));
    }


    [HttpGet]
    [Route("api/dialCode/lookup")]
    public async Task<IActionResult> GetDialCodesLookUpAsync([FromQuery] BasicFilterDto filter, CancellationToken cancellationToken)
    {
        var response = await _service.GetDialCodesLookUpAsync(filter, cancellationToken);

        return Ok(ApiResponse<object>.Ok(response, Messages.DialCodesRetrieved));
    }

    [HttpGet]
    [Route("api/division/one")]
    public async Task<IActionResult> GetDivisionOneAll([FromQuery] DivisionOneFilterDto filter, CancellationToken cancellationToken)
    {
        var items = await _service.GetAllDivisionOneAsync(filter, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.DivisionOneRetrieveSucessfully));
    }
    [HttpGet]
    [Route("api/division/one/lookup")]
    public async Task<IActionResult> GetDivisionOneActiveByCountry([FromQuery] DivisionOneLookUpFilterDto filter, CancellationToken cancellationToken)
    {
        var items = await _service.GetDivisionOneActiveByCountryIdAsync(filter, cancellationToken);
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
    [Route("api/division/two/lookup")]
    public async Task<IActionResult> GetDivisionTwoActiveByDivisionOne([FromQuery] DivisionTwoLookUpFilterDto filter, CancellationToken cancellationToken)
    {
        var items = await _service.GetDivisionTwoActiveByDivisionOneIdAsync(filter, cancellationToken);
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
    [Route("api/division/three/lookup")]
    public async Task<IActionResult> GetDivisionThreeActiveByDivisionTwo([FromQuery] DivisionThreeLookUpFilterDto filter, CancellationToken cancellationToken)
    {
        var items = await _service.GetDivisionThreeActiveByDivisionTwoIdAsync(filter, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items, Messages.DivisionThreeRetrieveSucessfully));
    }
}
