
using VoiceFirst_Admin.Utilities.DTOs.Features.Plan;
using VoiceFirst_Admin.Utilities.DTOs.Features.PlanProgramActoinLink;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface IPlanService
    {
        Task<ApiResponse<PlanDetailDto>>
           GetByIdAsync(int id,
           CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<PlanActiveDto>>> 
            GetActiveAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<ProgramPlanDetailDto>>>
            GetProgramDetailsByPlanIdAsync(int planId, CancellationToken cancellationToken = default);

        Task<ApiResponse<PlanDetailDto>> CreatePlanAsync(
        PlanCreateDto dto,
        int loginId,
        CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateAsync(int planId,PlanUpdateDto dto, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<PagedResultDto<PlanDetailDto>>> GetAllAsync(PlanFilterDto filter, CancellationToken cancellationToken = default);
        Task<ApiResponse<int>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<PlanDetailDto>> RecoverAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<int>> LinkPlansRoleAsync(int roleId, System.Collections.Generic.List<int> planIds, int loginId, CancellationToken cancellationToken = default);
    }
}
