using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.Country;
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
}
