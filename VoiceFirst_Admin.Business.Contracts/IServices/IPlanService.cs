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
    }
}
