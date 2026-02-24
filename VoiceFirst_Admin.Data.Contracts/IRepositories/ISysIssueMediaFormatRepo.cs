using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueMediaFormat;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface ISysIssueMediaFormatRepo
    {
        Task<int> CreateAsync(SysIssueMediaFormat entity, CancellationToken ct = default);
        Task<SysIssueMediaFormatDTO?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PagedResultDto<SysIssueMediaFormatDTO>> GetAllAsync(IssueMediaFormatFilterDTO filter, CancellationToken ct = default);
        Task<bool> UpdateAsync(SysIssueMediaFormat entity, CancellationToken ct = default);
        Task<SysIssueMediaFormatDTO> ExistsAsync(string name, int? excludeId = null, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, int deletedBy, CancellationToken ct = default);
        Task<List<SysIssueMediaFormatActiveDTO?>> GetActiveAsync(CancellationToken ct = default);
        Task<bool> RecoverAsync(int id, int loginId, CancellationToken ct = default);
        Task<SysIssueMediaFormatDTO> IsIdExistAsync(int id, CancellationToken ct = default);
    }
}
