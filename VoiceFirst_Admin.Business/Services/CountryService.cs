using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.Country;
using VoiceFirst_Admin.Utilities.DTOs.Features.Division;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

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

    public async Task<IEnumerable<CountryDto>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repo.GetActiveAsync(cancellationToken);
        var list = _mapper.Map<IEnumerable<CountryDto>>(entities);
        return list;
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

    public async Task<IEnumerable<DivisionOneDto>> GetDivisionOneActiveByCountryIdAsync(int countryId, CancellationToken cancellationToken = default)
    {
        var items = await _repo.GetDivisionOneActiveByCountryIdAsync(countryId, cancellationToken);
        return _mapper.Map<IEnumerable<DivisionOneDto>>(items);
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

    public async Task<IEnumerable<DivisionTwoDto>> GetDivisionTwoActiveByDivisionOneIdAsync(int divisionOneId, CancellationToken cancellationToken = default)
    {
        var items = await _repo.GetDivisionTwoActiveByDivisionOneIdAsync(divisionOneId, cancellationToken);
        return _mapper.Map<IEnumerable<DivisionTwoDto>>(items);
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

    public async Task<IEnumerable<DivisionThreeDto>> GetDivisionThreeActiveByDivisionTwoIdAsync(int divisionTwoId, CancellationToken cancellationToken = default)
    {
        var items = await _repo.GetDivisionThreeActiveByDivisionTwoIdAsync(divisionTwoId, cancellationToken);
        return _mapper.Map<IEnumerable<DivisionThreeDto>>(items);
    }
}
