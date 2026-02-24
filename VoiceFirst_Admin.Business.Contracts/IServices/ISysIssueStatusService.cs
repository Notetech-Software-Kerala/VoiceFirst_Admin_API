using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueStatus;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface ISysIssueStatusService
    {
        Task<ApiResponse<SysIssueStatusDTO>> CreateAsync(SysIssueStatusCreateDTO dto, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<SysIssueStatusDTO>?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<PagedResultDto<SysIssueStatusDTO>>> GetAllAsync(IssueStatusFilterDTO filter, CancellationToken cancellationToken = default);
        Task<ApiResponse<SysIssueStatusDTO>> UpdateAsync(SysIssueStatusUpdateDTO dto, int id, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<SysIssueStatusDTO>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<List<SysIssueStatusActiveDTO>>> GetActiveAsync(CancellationToken cancellationToken);
        Task<ApiResponse<SysIssueStatusDTO>> RecoverIssueStatusAsync(int id, int loginId, CancellationToken cancellationToken = default);
    }
}
