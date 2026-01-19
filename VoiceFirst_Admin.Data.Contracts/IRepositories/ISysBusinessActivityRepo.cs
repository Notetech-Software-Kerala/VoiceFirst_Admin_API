using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface ISysBusinessActivityRepo
    {
        Task<int> CreateAsync(SysBusinessActivity entity, CancellationToken cancellationToken = default);
        Task<SysBusinessActivity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResultDto<SysBusinessActivity>> GetAllAsync(CommonFilterDto1 filter, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(SysBusinessActivity entity, CancellationToken cancellationToken = default);
        Task<bool> BusinessActivityExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    }
}
