using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.Plan;
using VoiceFirst_Admin.Utilities.DTOs.Features.PlanProgramActoinLink;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface IPlanRepo
    {
        Task<PlanDto> PlanExistsAsync
           (string name, 
            int? excludeId = null, 
            CancellationToken cancellationToken = default);

        Task<PlanDetailDto> IsIdExistAsync(
         int planId,
         CancellationToken cancellationToken = default);
        Task<IEnumerable<PlanActiveDto>> GetActiveAsync
            (CancellationToken cancellationToken = default);

        Task<IEnumerable<ProgramPlanDetailDto>>
            GetProgramDetailsByPlanIdAsync(int planId,
             IDbConnection connection,
            IDbTransaction transaction, 
            CancellationToken cancellationToken = default);

        Task<Plan?> GetByNameAsync(string planName, CancellationToken cancellationToken = default);
        Task<int> CreatePlanAsync(
       Plan plan,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);


        Task<bool> BulkInsertActionLinksAsync(
        int planId,
        IEnumerable<int> programActionLinkIds,
        int createdBy,
        IDbConnection connection,
        IDbTransaction tx,
        CancellationToken cancellationToken);


        //Task LinkProgramActionLinksAsync(
        //    int planId,
        //    IEnumerable<int> programActionLinkIds,
        //    int createdBy,
        //    IDbConnection connection,
        //    IDbTransaction transaction,
        //    CancellationToken cancellationToken = default);

        //Task<int> CreatePlanAsync(VoiceFirst_Admin.Utilities.Models.Entities.Plan plan, CancellationToken cancellationToken = default);
        //Task LinkProgramActionLinksAsync(int planId, IEnumerable<int> programActionLinkIds, int createdBy, CancellationToken cancellationToken = default);
        Task<bool> UpdatePlanAsync(int planId, string? planName, bool? active, int updatedBy, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, int deletedBy, CancellationToken cancellationToken = default);
        Task<bool> RecoverAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task<int> LinkPlanRoleAsync(int roleId, List<int> planId, int createdBy, CancellationToken cancellationToken = default);
        Task<IEnumerable<int>> GetExistingPlanIdsAsync(IEnumerable<int> planIds, CancellationToken cancellationToken = default);
        Task<PlanDetailDto?> GetByIdAsync(int planId, IDbConnection connection,
            IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task UpsertPlanProgramActionLinksAsync(int planId, 
            IEnumerable<PlanProgramActionLinkUpdateDto> actions, int userId, 
            CancellationToken cancellationToken = default);
        Task<PagedResultDto<PlanDetailDto>>
            GetAllAsync(PlanFilterDto filter, CancellationToken cancellationToken = default);
    }
}
