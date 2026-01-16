using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface ISysBusinessActivityService
    {
        Task<int> CreateAsync(SysBusinessActivityCreateDTO dto,int userId, CancellationToken cancellationToken = default);
        Task<SysBusinessActivityUpdateDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<SysBusinessActivityUpdateDTO>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(int id, SysBusinessActivityUpdateDTO dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
