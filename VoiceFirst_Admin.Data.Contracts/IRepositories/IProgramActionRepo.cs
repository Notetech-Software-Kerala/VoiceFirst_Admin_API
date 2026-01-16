using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface IProgramActionRepo
    {
        Task<SysProgramActions> CreateAsync(SysProgramActions entity, CancellationToken cancellationToken = default);
        Task<SysProgramActions?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<SysProgramActions>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(SysProgramActions entity, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);

    }
}
