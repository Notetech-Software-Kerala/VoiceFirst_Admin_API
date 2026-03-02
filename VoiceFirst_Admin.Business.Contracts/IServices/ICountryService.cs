using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.Country;
using VoiceFirst_Admin.Utilities.DTOs.Features.Division;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices;

public interface ICountryService
{
    Task<PagedResultDto<CountryDto>> GetAllAsync(CountryFilterDto filter, CancellationToken cancellationToken = default);
    Task<PagedResultDto<CountryLookUpDto>> GetActiveAsync(BasicFilterDto filter, CancellationToken cancellationToken = default);
    Task<PagedResultDto<DialCodeLookUpDto>> GetDialCodesLookUpAsync(BasicFilterDto filter, CancellationToken cancellationToken = default);

    Task<PagedResultDto<DivisionOneDto>> GetAllDivisionOneAsync(DivisionOneFilterDto filter, CancellationToken cancellationToken = default);
    Task<PagedResultDto<DivisionOneLookUpDto>> GetDivisionOneActiveByCountryIdAsync(DivisionOneLookUpFilterDto filter, CancellationToken cancellationToken = default);

    Task<PagedResultDto<DivisionTwoDto>> GetAllDivisionTwoAsync(DivisionTwoFilterDto filter, CancellationToken cancellationToken = default);
    Task<PagedResultDto<DivisionTwoLookUpDto>> GetDivisionTwoActiveByDivisionOneIdAsync(DivisionTwoLookUpFilterDto filter, CancellationToken cancellationToken = default);

    Task<PagedResultDto<DivisionThreeDto>> GetAllDivisionThreeAsync(DivisionThreeFilterDto filter, CancellationToken cancellationToken = default);
    Task<PagedResultDto<DivisionThreeLookUpDto>> GetDivisionThreeActiveByDivisionTwoIdAsync(DivisionThreeLookUpFilterDto filter, CancellationToken cancellationToken = default);

}
