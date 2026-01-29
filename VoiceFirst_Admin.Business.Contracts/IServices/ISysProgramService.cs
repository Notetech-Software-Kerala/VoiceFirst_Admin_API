using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface ISysProgramService
    {
        Task<ApiResponse<List<SysProgramLookUp>>>
          GetAllActiveForPlanAsync(
          CancellationToken cancellationToken = default);

        Task<ApiResponse<int>> DeleteAsync(int id,
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


        Task<ApiResponse<PagedResultDto<SysProgramDto>>> GetAllAsync(
            SysProgramFilterDTO filter,
            CancellationToken cancellationToken = default);


        Task<ApiResponse<List<SysProgramLookUp>>> GetProgramLookupAsync(
            CancellationToken cancellationToken = default);


        Task<ApiResponse<List<SysProgramLookUp>>> 
            GetAllActiveByApplicationIdAsync(
            int applicationId,
            CancellationToken cancellationToken = default);

        Task<ApiResponse<SysProgramDto>> UpdateAsync(
            int programId,
            SysProgramUpdateDTO dto,
            int loginId,
            CancellationToken cancellationToken = default);

        Task<ApiResponse<List<SysProgramActionLinkLookUp>>> 
            GetActionLookupByProgramIdAsync(
            int programId,
            CancellationToken cancellationToken = default);
    }
}
