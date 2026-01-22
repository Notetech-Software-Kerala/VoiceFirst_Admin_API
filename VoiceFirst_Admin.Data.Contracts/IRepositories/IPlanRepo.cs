using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.Plan;
using VoiceFirst_Admin.Utilities.DTOs.Features.PlanProgramActoinLink;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface IPlanRepo
    {
        Task<IEnumerable<PlanActiveDto>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<VoiceFirst_Admin.Utilities.DTOs.Features.PlanProgramActoinLink.ProgramPlanDetailDto>> GetProgramDetailsByPlanIdAsync(int planId, CancellationToken cancellationToken = default);
        Task<VoiceFirst_Admin.Utilities.Models.Entities.Plan?> GetByNameAsync(string planName, CancellationToken cancellationToken = default);
        Task<int> CreatePlanAsync(VoiceFirst_Admin.Utilities.Models.Entities.Plan plan, CancellationToken cancellationToken = default);
        Task LinkProgramActionLinksAsync(int planId, IEnumerable<int> programActionLinkIds, int createdBy, CancellationToken cancellationToken = default);
        Task<bool> UpdatePlanAsync(int planId, string? planName, bool? active, int updatedBy, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, int deletedBy, CancellationToken cancellationToken = default);
        Task<int> RecoverPlanAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task UpsertPlanProgramActionLinksAsync(int planId, System.Collections.Generic.IEnumerable<VoiceFirst_Admin.Utilities.DTOs.Features.PlanProgramActoinLink.PlanProgramActionLinkUpdateDto> actions, int userId, CancellationToken cancellationToken = default);
        Task<VoiceFirst_Admin.Utilities.DTOs.Shared.PagedResultDto<PlanDetailDto>> GetAllAsync(VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanFilterDto filter, CancellationToken cancellationToken = default);
    }
}
