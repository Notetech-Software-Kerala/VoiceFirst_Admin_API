using System.Data;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories;

public interface ISysIssueMediaRuleTypeRepo
{
    Task<int> CreateAsync(SysIssueMediaRuleType entity, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default);
    Task<bool> BulkInsertAsync(int ruleId, IEnumerable<IssueMediaRuleTypeCreateDTO> mediaTypes, int createdBy, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default);
    Task<bool> BulkInsertAsync(IEnumerable<SysIssueMediaRuleType> entities, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default);
    Task<int> BulkUpdateAsync(IEnumerable<SysIssueMediaRuleType> entities, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default);
    Task<SysIssueMediaRuleType?> GetAsync(int ruleId, int mediaTypeId, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default);
    Task<bool> UpdateAsync(SysIssueMediaRuleType entity, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default);
    Task<IEnumerable<SysIssueMediaRuleType>> GetByRuleIdsAsync(IEnumerable<int> ruleIds, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default);
}
