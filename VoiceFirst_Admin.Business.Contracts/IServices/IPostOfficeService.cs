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
    Task<IEnumerable<PostOfficeLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<PagedResultDto<PostOfficeDto>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default);
    Task<ApiResponse<PostOfficeDto>> UpdateAsync(PostOfficeUpdateDto dto, int id, int loginId, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> RestoreAsync(int id, int loginId, CancellationToken cancellationToken = default);
    Task<PostOfficeDto?> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
}
