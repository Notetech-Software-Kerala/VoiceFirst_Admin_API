using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface ISysIssueTypeService
    {
        Task<ApiResponse<SysIssueTypeDTO>> CreateAsync(SysIssueTypeCreateDTO dto, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<SysIssueTypeDTO>?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<PagedResultDto<SysIssueTypeDTO>>> GetAllAsync(IssueTypeFilterDTO filter, CancellationToken cancellationToken = default);
        Task<ApiResponse<SysIssueTypeDTO>> UpdateAsync(SysIssueTypeUpdateDTO dto, int sysIssueTypeId, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<SysIssueTypeDTO>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<List<SysIssueTypeActiveDTO>>> GetActiveAsync(CancellationToken cancellationToken);
        Task<ApiResponse<SysIssueTypeDTO>> RecoverIssueTypeAsync(int id, int loginId, CancellationToken cancellationToken = default);
    }
}
