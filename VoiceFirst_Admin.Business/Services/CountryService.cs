using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Country;
using VoiceFirst_Admin.Utilities.DTOs.Features.Division;
using VoiceFirst_Admin.Utilities.DTOs.Features.Place;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoiceFirst_Admin.Business.Services;

public class CountryService : ICountryService
{
    private readonly ICountryRepo _repo;
    private readonly IMapper _mapper;

    public CountryService(ICountryRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<CountryDto>> GetAllAsync(CountryFilterDto filter, CancellationToken cancellationToken = default)
    {
        var pagedEntities = await _repo.GetAllAsync(filter, cancellationToken);
        var list = _mapper.Map<IEnumerable<CountryDto>>(pagedEntities.Items);

        return new PagedResultDto<CountryDto>
        {
            Items = list,
            TotalCount = pagedEntities.TotalCount,
            PageNumber = pagedEntities.PageNumber,
            PageSize = pagedEntities.PageSize
        };
    }

    public async Task<PagedResultDto<CountryLookUpDto>> GetActiveAsync(BasicFilterDto filter, CancellationToken cancellationToken = default)
    {
        var entities = await _repo.GetActiveAsync(filter,cancellationToken);
        var list = _mapper.Map<IEnumerable<CountryLookUpDto>>(entities.Items);
        return new PagedResultDto<CountryLookUpDto>
        {
            Items = list,
            TotalCount = entities.TotalCount,
            PageNumber = entities.PageNumber,
            PageSize = entities.PageSize
        };
    }

    public async Task<PagedResultDto<DialCodeLookUpDto>> GetDialCodesLookUpAsync(BasicFilterDto filter, CancellationToken cancellationToken = default)
    {
        var dtos = await _repo.GetDialCodesLookUpAsync(filter,cancellationToken);
        return dtos;
    }


    

    // Division One Methods

    public async Task<PagedResultDto<DivisionOneDto>> GetAllDivisionOneAsync(DivisionOneFilterDto filter, CancellationToken cancellationToken = default)
    {
        var paged = await _repo.GetAllDivisionOneAsync(filter, cancellationToken);
        var list = _mapper.Map<IEnumerable<DivisionOneDto>>(paged.Items);

        return new PagedResultDto<DivisionOneDto>
        {
            Items = list,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    public async Task<PagedResultDto<DivisionOneLookUpDto>> GetDivisionOneActiveByCountryIdAsync(DivisionOneLookUpFilterDto filter, CancellationToken cancellationToken = default)
    {
        var items = await _repo.GetDivisionOneActiveByCountryIdAsync(filter, cancellationToken);
        var data = _mapper.Map<IEnumerable<DivisionOneLookUpDto>>(items.Items);
        return new PagedResultDto<DivisionOneLookUpDto>
        {
            Items = data,
            TotalCount = items.TotalCount,
            PageNumber = items.PageNumber,
            PageSize = items.PageSize
        };
        
    }


    // Division Two Methods


    public async Task<PagedResultDto<DivisionTwoDto>> GetAllDivisionTwoAsync(DivisionTwoFilterDto filter, CancellationToken cancellationToken = default)
    {
        var paged = await _repo.GetAllDivisionTwoAsync(filter, cancellationToken);
        var list = _mapper.Map<IEnumerable<DivisionTwoDto>>(paged.Items);

        return new PagedResultDto<DivisionTwoDto>
        {
            Items = list,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    public async Task<PagedResultDto<DivisionTwoLookUpDto>> GetDivisionTwoActiveByDivisionOneIdAsync(DivisionTwoLookUpFilterDto filter, CancellationToken cancellationToken = default)
    {
        var items = await _repo.GetDivisionTwoActiveByDivisionOneIdAsync(filter, cancellationToken);
        var data= _mapper.Map<IEnumerable<DivisionTwoLookUpDto>>(items.Items);
        return new PagedResultDto<DivisionTwoLookUpDto>
        {
            Items = data,
            TotalCount = items.TotalCount,
            PageNumber = items.PageNumber,
            PageSize = items.PageSize
        };

    }


    // Division Three Methods

    public async Task<PagedResultDto<DivisionThreeDto>> GetAllDivisionThreeAsync(DivisionThreeFilterDto filter, CancellationToken cancellationToken = default)
    {
        var paged = await _repo.GetAllDivisionThreeAsync(filter, cancellationToken);
        var list = _mapper.Map<IEnumerable<DivisionThreeDto>>(paged.Items);

        return new PagedResultDto<DivisionThreeDto>
        {
            Items = list,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    public async Task<PagedResultDto<DivisionThreeLookUpDto>> GetDivisionThreeActiveByDivisionTwoIdAsync(DivisionThreeLookUpFilterDto filter, CancellationToken cancellationToken = default)
    {
        var paged = await _repo.GetDivisionThreeActiveByDivisionTwoIdAsync(filter, cancellationToken);
        var data= _mapper.Map<IEnumerable<DivisionThreeLookUpDto>>(paged.Items);
        return new PagedResultDto<DivisionThreeLookUpDto>
        {
            Items = data,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }
}
