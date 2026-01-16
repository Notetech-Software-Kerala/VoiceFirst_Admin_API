using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;
using VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface IProgramActionService
    {
        Task<SysProgramActions> CreateAsync(ProgramActionCreateDto dto, int loginId, CancellationToken cancellationToken = default);
        Task<SysProgramActions?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<SysProgramActions>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(ProgramActionUpdateDto dto, int loginId, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
    }
}