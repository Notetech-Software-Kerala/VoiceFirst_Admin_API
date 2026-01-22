using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.Country;
using VoiceFirst_Admin.Utilities.DTOs.Features.Division;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories;

public interface ICountryRepo
{
    Task<PagedResultDto<Country>> GetAllAsync(CountryFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<Country>> GetActiveAsync(CancellationToken cancellationToken = default);

    Task<PagedResultDto<DivisionOne>> GetAllDivisionOneAsync(DivisionOneFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<DivisionOne>> GetDivisionOneActiveByCountryIdAsync(int countryId, CancellationToken cancellationToken = default);

    Task<PagedResultDto<DivisionTwo>> GetAllDivisionTwoAsync(DivisionTwoFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<DivisionTwo>> GetDivisionTwoActiveByDivisionOneIdAsync(int divisionOneId, CancellationToken cancellationToken = default);

    Task<PagedResultDto<DivisionThree>> GetAllDivisionThreeAsync(DivisionThreeFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<DivisionThree>> GetDivisionThreeActiveByDivisionTwoIdAsync(int divisionTwoId, CancellationToken cancellationToken = default);
}
