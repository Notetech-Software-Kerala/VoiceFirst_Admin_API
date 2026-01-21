using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.Country;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Business.Contracts.IServices;

public interface ICountryService
{
    Task<PagedResultDto<CountryDto>> GetAllAsync(CountryFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<CountryDto>> GetActiveAsync(CancellationToken cancellationToken = default);
}
