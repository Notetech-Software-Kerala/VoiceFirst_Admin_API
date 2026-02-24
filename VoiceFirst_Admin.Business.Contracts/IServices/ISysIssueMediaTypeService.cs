using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueMediaType;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface ISysIssueMediaTypeService
    {
        Task<ApiResponse<SysIssueMediaTypeDTO>> CreateAsync(SysIssueMediaTypeCreateDTO dto, int loginId, CancellationToken ct = default);
        Task<ApiResponse<SysIssueMediaTypeDTO>?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ApiResponse<PagedResultDto<SysIssueMediaTypeDTO>>> GetAllAsync(IssueMediaTypeFilterDTO filter, CancellationToken ct = default);
        Task<ApiResponse<SysIssueMediaTypeDTO>> UpdateAsync(SysIssueMediaTypeUpdateDTO dto, int id, int loginId, CancellationToken ct = default);
        Task<ApiResponse<SysIssueMediaTypeDTO>> DeleteAsync(int id, int loginId, CancellationToken ct = default);
        Task<ApiResponse<List<SysIssueMediaTypeActiveDTO>>> GetActiveAsync(CancellationToken ct);
        Task<ApiResponse<SysIssueMediaTypeDTO>> RecoverAsync(int id, int loginId, CancellationToken ct = default);
    }
}
