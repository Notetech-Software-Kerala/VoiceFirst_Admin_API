using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface ISysProgramRepo
    {
        Task<SysProgram?> ExistsByNameAsync(int applicationId, string name, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<SysProgram?> ExistsByLabelAsync(int applicationId, string label, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<SysProgram?> ExistsByRouteAsync(int applicationId, string route, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<SysProgram?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<SysProgram> CreateAsync(SysProgram entity, List<int> permissionIds, CancellationToken cancellationToken = default);
        Task<IEnumerable<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink.SysProgramActionLinkDTO>> GetLinksByProgramIdAsync(int programId, CancellationToken cancellationToken = default);
    }
}
