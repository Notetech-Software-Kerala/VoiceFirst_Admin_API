using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface ISysUserCustomFieldService
    {
        Task<ApiResponse<CustomFieldDetailDto>> CreateAsync(UserCustomFieldCreateDto dto, int loginId, CancellationToken cancellationToken = default);
        Task<CustomFieldDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResultDto<UserCustomFieldLookUpDto>?> GetLookUpAsync(BasicFilterDto filter, CancellationToken cancellationToken = default);
        Task<PagedResultDto<SysUserCustomFieldDto>> GetAllAsync(SysUserCustomFieldFilterDto filter, CancellationToken cancellationToken = default);
        Task<ApiResponse<CustomFieldDetailDto>> UpdateAsync(SysUserCustomFieldUpdateDto dto, int id, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<CustomFieldDetailDto>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default);
    }
}
