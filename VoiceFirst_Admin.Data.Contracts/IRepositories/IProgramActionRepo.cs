using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface IProgramActionRepo
    {
        Task<Dictionary<string, bool>> IsBulkIdsExistAsync(
        List<dynamic> sysProgramActionIds,
        CancellationToken cancellationToken = default);

        Task<SysProgramActions> GetActiveByIdAsync
        (int SysProgramActionId, CancellationToken cancellationToken = default);
        Task<ProgramActionDto> IsIdExistAsync
          (int SysProgramActionId, CancellationToken cancellationToken = default);
        Task<SysProgramActions> CreateAsync(SysProgramActions entity, CancellationToken cancellationToken = default);
        Task<IEnumerable<SysProgramActions>> GetLookupAsync(CancellationToken cancellationToken = default);
        Task<SysProgramActions?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResultDto<SysProgramActions>> GetAllAsync(ProgramActionFilterDto filter, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(SysProgramActions entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(SysProgramActions entity, CancellationToken cancellationToken = default);
        Task<bool> RestoreAsync(SysProgramActions entity, CancellationToken cancellationToken = default);
        Task<SysProgramActions> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);

    }
}
