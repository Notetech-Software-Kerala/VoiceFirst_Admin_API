using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.Role;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface IRoleRepo
    {
        Task<Dictionary<string, bool>> IsBulkIdsExistAsync(
   List<int> roleIds,
   CancellationToken cancellationToken = default);
        Task<SysRoles> CreateAsync(SysRoles entity, List<PlanActionLinkCreateDto> PlanActionLinkCreateDto, CancellationToken cancellationToken = default);
        Task<SysRoles?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResultDto<SysRoles>> GetAllAsync(RoleFilterDto filter, CancellationToken cancellationToken = default);
        Task<IEnumerable<RoleLookUpDto>>
                GetLookUpAllAsync(int ApplicationId, CancellationToken cancellationToken = default);        Task<bool> UpdateAsync(SysRoles entity, CancellationToken cancellationToken = default); 
        Task<bool> DeleteAsync(SysRoles entity, CancellationToken cancellationToken = default);
        Task<bool> RestoreAsync(SysRoles entity, CancellationToken cancellationToken = default);
        Task<SysRoles> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<PlanRoleProgramActionLink>> GetActionIdsByRoleIdAsync(int roleId, CancellationToken cancellationToken = default);
        Task<BulkUpsertError?> AddRoleActionLinksAsync(
            int roleId,
            int applicationId,
            List<PlanActionLinkCreateDto> planActionLink,
            int loginId,
            CancellationToken cancellationToken = default);

        Task<BulkUpsertError?> UpdateRoleActionLinksAsync(
            int roleId,
            int applicationId,
             List<PlanRoleActionLinkUpdateDto>? UpdateActionLinks,
            int loginId,
            CancellationToken cancellationToken = default);

    }
}
