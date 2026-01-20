using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface IProgramActionService
    {
        Task<ApiResponse<ProgramActionDto>> CreateAsync(ProgramActionCreateDto dto, int loginId, CancellationToken cancellationToken = default);
        Task<ProgramActionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProgramActionLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);
        Task<PagedResultDto<ProgramActionDto>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default);
        Task<ApiResponse<ProgramActionDto>> UpdateAsync(ProgramActionUpdateDto dto,int id, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<object>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<object>> RestoreAsync(int id, int loginId, CancellationToken cancellationToken = default);
        Task<ProgramActionDto?> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
    }
}