using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices;

public interface IPostOfficeService
{
    Task<ApiResponse<PostOfficeDto>> CreateAsync(PostOfficeCreateDto dto, int loginId, CancellationToken cancellationToken = default);
    Task<PostOfficeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<ZipCodeLookUp>?>> GetZipCodesByPostOfficeIdAsync(int id,
    int? placeId = null, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<PostOfficeLookupDto>>> GetLookupAsync(PostOfficeLookUpFilterDto filter,CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<PostOfficeDetailLookupDto>>> GetPostOfficeDetailsByZipCodeAsync(string zipCode, CancellationToken cancellationToken = default);
    Task<PagedResultDto<PostOfficeDto>> GetAllAsync(PostOfficeFilterDto filter, CancellationToken cancellationToken = default);
    Task<ApiResponse<PostOfficeDto>> UpdateAsync(PostOfficeUpdateDto dto, int id, int loginId, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> RestoreAsync(int id, int loginId, CancellationToken cancellationToken = default);
    Task<PostOfficeDto?> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
}
