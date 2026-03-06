using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface ISysUserCustomFieldService
    {
        Task<ApiResponse<object>> CreateAsync(UserCustomFieldCreateDto dto, int loginId, CancellationToken cancellationToken = default);
        Task<SysUserCustomFieldDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<object>> UpdateAsync(SysUserCustomFieldUpdateDto dto, int id, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<object>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default);
    }
}
