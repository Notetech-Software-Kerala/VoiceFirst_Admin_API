using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.Role;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface IRoleService
    {
        Task<ApiResponse<RoleDto>> CreateAsync(RoleCreateDto dto, int loginId, CancellationToken cancellationToken = default);
        Task<RoleDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<RoleLookUpDto>> GetLookUpAllAsync(CancellationToken cancellationToken = default);
        Task<PagedResultDto<RoleDto>> GetAllAsync(RoleFilterDto filter, CancellationToken cancellationToken = default);
        Task<ApiResponse<RoleDetailDto>> UpdateAsync(RoleUpdateDto dto, int id, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<object>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<object>> RestoreAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task<RoleDto?> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
    }
}
