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
        Task<ApiResponse<SysUserCustomFieldDetailDto>> CreateAsync(UserCustomFieldCreateDto dto, int loginId, CancellationToken cancellationToken = default);
        Task<SysUserCustomFieldDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResultDto<SysUserCustomFieldDetailDto>> GetAllAsync(SysUserCustomFieldFilterDto filter, CancellationToken cancellationToken = default);
        Task<ApiResponse<SysUserCustomFieldDetailDto>> UpdateAsync(SysUserCustomFieldUpdateDto dto, int id, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<object>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default);
    }
}
