using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueMediaFormat;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface ISysIssueMediaFormatService
    {
        Task<ApiResponse<SysIssueMediaFormatDTO>> CreateAsync(SysIssueMediaFormatCreateDTO dto, int loginId, CancellationToken ct = default);
        Task<ApiResponse<SysIssueMediaFormatDTO>?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ApiResponse<PagedResultDto<SysIssueMediaFormatDTO>>> GetAllAsync(IssueMediaFormatFilterDTO filter, CancellationToken ct = default);
        Task<ApiResponse<SysIssueMediaFormatDTO>> UpdateAsync(SysIssueMediaFormatUpdateDTO dto, int id, int loginId, CancellationToken ct = default);
        Task<ApiResponse<SysIssueMediaFormatDTO>> DeleteAsync(int id, int loginId, CancellationToken ct = default);
        Task<ApiResponse<List<SysIssueMediaFormatActiveDTO>>> GetActiveAsync(CancellationToken ct);
        Task<ApiResponse<SysIssueMediaFormatDTO>> RecoverAsync(int id, int loginId, CancellationToken ct = default);
    }
}
