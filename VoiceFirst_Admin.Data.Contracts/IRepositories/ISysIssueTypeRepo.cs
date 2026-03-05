using System.Data;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface ISysIssueTypeRepo
    {
        Task<SysIssueTypeDTO> GetIdAndDeletedByNameAsync
           (string name, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<int> CreateAsync(SysIssueType entity, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(SysIssueType entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(SysIssueType entity, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<SysIssueTypeDTO?> GetByIdAsync(int issueTypeId, CancellationToken cancellationToken = default);
        Task<PagedResultDto<SysIssueTypeDTO>> GetAllAsync(IssueTypeFilterDTO filter, CancellationToken cancellationToken = default);
        Task<SysIssueTypeDTO> IssueTypeExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, int deletedBy, CancellationToken cancellationToken = default);
        Task<List<SysIssueTypeActiveDTO?>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<bool> RecoverIssueTypeAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task<SysIssueTypeDTO> IsIdExistAsync(int issueTypeId, CancellationToken cancellationToken = default);
    }
}
