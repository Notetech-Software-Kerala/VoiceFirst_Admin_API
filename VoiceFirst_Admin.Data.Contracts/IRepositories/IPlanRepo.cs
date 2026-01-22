using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.Plan;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface IPlanRepo
    {
        Task<IEnumerable<PlanActiveDto>> GetActiveAsync(CancellationToken cancellationToken = default);
    }
}
