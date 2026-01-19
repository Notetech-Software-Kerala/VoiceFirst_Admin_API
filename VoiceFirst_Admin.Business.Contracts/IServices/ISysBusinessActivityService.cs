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
        Task<SysBusinessActivityDetailsDTO> CreateAsync(SysBusinessActivityCreateDTO dto, int loginId, CancellationToken cancellationToken = default);
        Task<SysBusinessActivityDetailsDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResultDto<SysBusinessActivity>> GetAllAsync(CommonFilterDto1 filter, CancellationToken cancellationToken = default);
        Task<SysBusinessActivityDetailsDTO> UpdateAsync(SysBusinessActivityUpdateDTO dto,int sysBusinessActivityId, int loginId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
