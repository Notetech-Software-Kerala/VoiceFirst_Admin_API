using System.Data;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories;

public interface ISysIssueMediaRuleRepo
{
    Task<bool>
              CheckMediaFormatLinksExistAsync(
                          int issueTypeId,
                  IEnumerable<int> formatIds,
                  bool update,
                  IDbConnection connection,
                  IDbTransaction transaction,
                  CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SysIssueMediaRule rule, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default);
    Task<bool> BulkInsertAsync(int issueTypeId, IEnumerable<IssueMediaRuleCreateDTO> dtos, int createdBy, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default);
    Task<bool> BulkUpdateAsync(IEnumerable<SysIssueMediaRule> entities, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default);
    Task<bool> UpdateAsync(SysIssueMediaRule rule, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default);
    Task<BulkValidationResult> IsBulkExistAsync(int issueTypeId, IEnumerable<int> formatIds, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default);
    Task<IEnumerable<SysIssueMediaRule>> GetByIssueTypeAndFormatsAsync(int issueTypeId, IEnumerable<int> formatIds, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default);
    Task<SysIssueMediaRule?> GetByIssueTypeAndFormatAsync(int issueTypeId, int formatId, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default);
}
