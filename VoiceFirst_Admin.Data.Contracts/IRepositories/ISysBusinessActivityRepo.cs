using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface ISysBusinessActivityRepo
    {
        Task<int> CreateAsync(SysBusinessActivity entity, CancellationToken cancellationToken = default);
        Task<SysBusinessActivityDTO?> GetByIdAsync(int ActivityId, CancellationToken cancellationToken = default);
        Task<PagedResultDto<SysBusinessActivityDTO>> GetAllAsync(BusinessActivityFilterDTO filter, CancellationToken cancellationToken = default);     
        Task<bool> UpdateAsync(SysBusinessActivity entity, CancellationToken cancellationToken = default);
        Task<SysBusinessActivityDTO> BusinessActivityExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id,int deletedBy, CancellationToken cancellationToken = default);
        Task<List<SysBusinessActivityActiveDTO?>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<bool> RecoverBusinessActivityAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task<SysBusinessActivityDTO> IsIdExistAsync(
          int activityId,
          CancellationToken cancellationToken = default);

    }
}
