using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueStatus;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface ISysIssueStatusRepo
    {
        Task<int> CreateAsync(SysIssueStatus entity, CancellationToken cancellationToken = default);
        Task<SysIssueStatusDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResultDto<SysIssueStatusDTO>> GetAllAsync(IssueStatusFilterDTO filter, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(SysIssueStatus entity, CancellationToken cancellationToken = default);
        Task<SysIssueStatusDTO> IssueStatusExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, int deletedBy, CancellationToken cancellationToken = default);
        Task<List<SysIssueStatusActiveDTO?>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<bool> RecoverIssueStatusAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task<SysIssueStatusDTO> IsIdExistAsync(int id, CancellationToken cancellationToken = default);
    }
}
