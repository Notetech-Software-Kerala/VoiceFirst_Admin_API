using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.Country;
using VoiceFirst_Admin.Utilities.DTOs.Features.Division;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Business.Contracts.IServices;

public interface ICountryService
{
    Task<PagedResultDto<CountryDto>> GetAllAsync(CountryFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<CountryDto>> GetActiveAsync(CancellationToken cancellationToken = default);

    Task<PagedResultDto<DivisionOneDto>> GetAllDivisionOneAsync(DivisionOneFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<DivisionOneDto>> GetDivisionOneActiveByCountryIdAsync(int countryId, CancellationToken cancellationToken = default);

    Task<PagedResultDto<DivisionTwoDto>> GetAllDivisionTwoAsync(DivisionTwoFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<DivisionTwoDto>> GetDivisionTwoActiveByDivisionOneIdAsync(int divisionOneId, CancellationToken cancellationToken = default);

    Task<PagedResultDto<DivisionThreeDto>> GetAllDivisionThreeAsync(DivisionThreeFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<DivisionThreeDto>> GetDivisionThreeActiveByDivisionTwoIdAsync(int divisionTwoId, CancellationToken cancellationToken = default);

}
