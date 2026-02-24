using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueCharacterType;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface ISysIssueCharacterTypeRepo
    {
        Task<int> CreateAsync(SysIssueCharacterType entity, CancellationToken cancellationToken = default);
        Task<SysIssueCharacterTypeDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResultDto<SysIssueCharacterTypeDTO>> GetAllAsync(IssueCharacterTypeFilterDTO filter, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(SysIssueCharacterType entity, CancellationToken cancellationToken = default);
        Task<SysIssueCharacterTypeDTO> ExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, int deletedBy, CancellationToken cancellationToken = default);
        Task<List<SysIssueCharacterTypeActiveDTO?>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<bool> RecoverAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task<SysIssueCharacterTypeDTO> IsIdExistAsync(int id, CancellationToken cancellationToken = default);
    }
}
