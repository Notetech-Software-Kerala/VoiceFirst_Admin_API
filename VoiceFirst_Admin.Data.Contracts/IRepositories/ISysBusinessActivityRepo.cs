using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface ISysBusinessActivityRepo
    {
        Task<SysBusinessActivity> CreateAsync(SysBusinessActivity entity, CancellationToken cancellationToken = default);
        Task<SysBusinessActivity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<SysBusinessActivity>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(SysBusinessActivity entity, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    }
}
