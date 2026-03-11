using System.Data;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueMediaType;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface ISysIssueMediaTypeRepo
    {
        Task<int> CreateAsync(SysIssueMediaType entity, CancellationToken ct = default);
        Task<SysIssueMediaTypeDTO?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PagedResultDto<SysIssueMediaTypeDTO>> GetAllAsync(IssueMediaTypeFilterDTO filter, CancellationToken ct = default);
        Task<bool> UpdateAsync(SysIssueMediaType entity, CancellationToken ct = default);
        Task<SysIssueMediaTypeDTO> ExistsAsync(string name, int? excludeId = null, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, int deletedBy, CancellationToken ct = default);
        Task<List<SysIssueMediaTypeActiveDTO?>> GetActiveAsync(CancellationToken ct = default);
        Task<bool> RecoverAsync(int id, int loginId, CancellationToken ct = default);
        Task<SysIssueMediaTypeDTO> IsIdExistAsync(int id, CancellationToken ct = default);
        Task<BulkValidationResult> IsBulkIdsExistAsync(IEnumerable<int> ids, CancellationToken ct = default);
        Task<BulkValidationResult> IsBulkIdsExistAsync(IEnumerable<int> ids, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default);
    }
}
