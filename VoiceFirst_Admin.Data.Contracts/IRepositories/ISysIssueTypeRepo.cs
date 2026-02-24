using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface ISysIssueTypeRepo
    {
        Task<bool> BulkUpdateMediaRulesAsync(
           IEnumerable<SysIssueMediaRule> entities,
           IDbConnection connection,
           IDbTransaction transaction,
           CancellationToken cancellationToken = default);
        Task<int> CreateAsync(SysIssueType entity, CancellationToken cancellationToken = default);
        Task<int> CreateAsync(SysIssueType entity, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(SysIssueType entity, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<SysIssueTypeDTO?> GetByIdAsync(int issueTypeId, CancellationToken cancellationToken = default);
        Task<PagedResultDto<SysIssueTypeDTO>> GetAllAsync(IssueTypeFilterDTO filter, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(SysIssueType entity, CancellationToken cancellationToken = default);
        Task<SysIssueTypeDTO> IssueTypeExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, int deletedBy, CancellationToken cancellationToken = default);
        Task<List<SysIssueTypeActiveDTO?>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<bool> RecoverIssueTypeAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task<SysIssueTypeDTO> IsIdExistAsync(int issueTypeId, CancellationToken cancellationToken = default);
        Task<int> CreateMediaRuleAsync(SysIssueMediaRule rule, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<bool> BulkInsertMediaRuleTypesAsync(int ruleId, IEnumerable<IssueMediaRuleTypeCreateDTO> mediaTypes, int createdBy, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<Dictionary<string, bool>> IsBulkMediaRulesExistAsync(int issueTypeId, IEnumerable<int> issueMediaFormatIds, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<bool> BulkInsertMediaRulesAsync(int issueTypeId, IEnumerable<IssueMediaRuleCreateDTO> dtos, int createdBy, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<IEnumerable<SysIssueMediaRule>> GetRulesByIssueTypeAndFormatsAsync(int issueTypeId, IEnumerable<int> formatIds, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<Dictionary<string, bool>> IsBulkMediaTypesExistAsync(IEnumerable<int> mediaTypeIds, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<int> BulkUpdateMediaRuleTypesAsync(IEnumerable<SysIssueMediaRuleType> dtos, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<bool> BulkInsertMediaRuleTypesAsync(IEnumerable<SysIssueMediaRuleType> dtos, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<SysIssueMediaRule?> GetMediaRuleByIssueTypeAndFormatAsync(int issueTypeId, int issueMediaFormatId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<bool> UpdateMediaRuleAsync(SysIssueMediaRule rule, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<SysIssueMediaRuleType?> GetMediaRuleTypeAsync(int ruleId, int issueMediaTypeId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<bool> UpdateMediaRuleTypeAsync(SysIssueMediaRuleType dto, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<int> CreateMediaRuleTypeAsync(SysIssueMediaRuleType dto, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    }
}
