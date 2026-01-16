using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface ISysBusinessActivityService
    {
        Task<SysBusinessActivity> CreateAsync(SysBusinessActivityCreateDTO dto, int loginId, CancellationToken cancellationToken = default);
        Task<SysBusinessActivity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<SysBusinessActivity>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(SysBusinessActivityUpdateDTO dto,int sysBusinessActivityId, int loginId, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
