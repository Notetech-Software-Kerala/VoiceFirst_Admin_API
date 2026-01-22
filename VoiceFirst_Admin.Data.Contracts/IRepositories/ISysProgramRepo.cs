using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface ISysProgramRepo
    {
        
        Task<SysProgram?> ExistsByNameAsync(int applicationId, string name, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<SysProgram?> ExistsByLabelAsync(int applicationId, string label, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<SysProgram?> ExistsByRouteAsync(int applicationId, string route, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<SysProgram?> GetActiveByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<SysProgramDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, int deletedBy, CancellationToken cancellationToken = default);
        Task<SysProgram> CreateAsync(SysProgram entity, List<int> permissionIds, CancellationToken cancellationToken = default);
        Task<IEnumerable<SysProgramActionLinkDTO>> GetLinksByProgramIdAsync(int programId, CancellationToken cancellationToken = default);
        Task<int> RecoverProgramAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task<IEnumerable<SysProgramByApplicationIdDTO>> GetAllActiveByApplicationIdAsync(int applicationId, CancellationToken cancellationToken = default);
        Task<VoiceFirst_Admin.Utilities.DTOs.Shared.PagedResultDto<SysProgramDto>> GetAllAsync(VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramFilterDTO filter, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(SysProgram entity, CancellationToken cancellationToken = default);
        Task UpsertProgramActionLinksAsync(int programId, IEnumerable<SysProgramActionLinkUpdateDTO> actions, int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramLookUp>> GetProgramLookupAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink.SysProgramActionLinkLookUp>> GetActionLookupByProgramIdAsync(int programId, CancellationToken cancellationToken = default);
    }
}
