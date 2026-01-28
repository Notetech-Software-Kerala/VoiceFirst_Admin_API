using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.Plan;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface IPlanService
    {
        Task<ApiResponse<IEnumerable<PlanActiveDto>>> 
            GetActiveAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<VoiceFirst_Admin.Utilities.DTOs.Features.PlanProgramActoinLink.ProgramPlanDetailDto>>>
            GetProgramDetailsByPlanIdAsync(int planId, CancellationToken cancellationToken = default);
        Task<ApiResponse<VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanDto>> CreateAsync(VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanCreateDto dto, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> UpdateAsync(int planId, VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanUpdateDto dto, int loginId, CancellationToken cancellationToken = default);
        Task<VoiceFirst_Admin.Utilities.DTOs.Shared.PagedResultDto<VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanDetailDto>> GetAllAsync(VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanFilterDto filter, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<int>> RecoverPlanAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<int>> LinkPlansRoleAsync(int roleId, System.Collections.Generic.List<int> planIds, int loginId, CancellationToken cancellationToken = default);
    }
}
