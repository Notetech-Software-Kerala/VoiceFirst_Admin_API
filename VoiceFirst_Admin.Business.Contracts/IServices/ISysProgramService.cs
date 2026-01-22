using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface ISysProgramService
    {
        Task<ApiResponse<bool>> DeleteAsync(int id,
            int loginId,
            CancellationToken cancellationToken = default);

        Task<ApiResponse<SysProgramDto>> CreateAsync
            (SysProgramCreateDTO dto, 
            int loginId, 
            CancellationToken cancellationToken = default);

        Task<ApiResponse<SysProgramDto>> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<ApiResponse<SysProgramDto>> RecoverProgramAsync(
        int id,
        int loginId,
        CancellationToken cancellationToken = default);

        Task<VoiceFirst_Admin.Utilities.DTOs.Shared.PagedResultDto<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramDto>> GetAllAsync(
            VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramFilterDTO filter,
            CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<SysProgramByApplicationIdDTO>>> GetAllActiveByApplicationIdAsync(
            int applicationId,
            CancellationToken cancellationToken = default);

        Task<ApiResponse<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramDto>> UpdateAsync(
            int programId,
            VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramUpdateDTO dto,
            int loginId,
            CancellationToken cancellationToken = default);
    }
}
