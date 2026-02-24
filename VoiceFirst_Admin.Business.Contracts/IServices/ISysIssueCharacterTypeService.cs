using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueCharacterType;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface ISysIssueCharacterTypeService
    {
        Task<ApiResponse<SysIssueCharacterTypeDTO>> CreateAsync(SysIssueCharacterTypeCreateDTO dto, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<SysIssueCharacterTypeDTO>?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<PagedResultDto<SysIssueCharacterTypeDTO>>> GetAllAsync(IssueCharacterTypeFilterDTO filter, CancellationToken cancellationToken = default);
        Task<ApiResponse<SysIssueCharacterTypeDTO>> UpdateAsync(SysIssueCharacterTypeUpdateDTO dto, int id, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<SysIssueCharacterTypeDTO>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<List<SysIssueCharacterTypeActiveDTO>>> GetActiveAsync(CancellationToken cancellationToken);
        Task<ApiResponse<SysIssueCharacterTypeDTO>> RecoverAsync(int id, int loginId, CancellationToken cancellationToken = default);
    }
}
