using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.Users;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface IUserService
    {
        Task<ApiResponse<EmployeeDetailDto>> UpdateAsync(
             int updateUserId,
             int ApplicationId,
             EmployeeUpdateDto dto,
             int loginId,
             CancellationToken cancellationToken = default);

        Task<ApiResponse<PagedResultDto<EmployeeDto>>> GetAllAsync(
            EmployeeFilterDto filter, int loginUserId,
            CancellationToken cancellationToken = default);


        Task<ApiResponse<EmployeeDetailDto>>
           GetByIdAsync(int id,
           CancellationToken cancellationToken = default);

        Task<ApiResponse<EmployeeDetailDto>> CreateAsync(
        EmployeeCreateDto employee, int ApplicationId,
        int loginId,
        CancellationToken cancellationToken);

        Task<ApiResponse<EmployeeDetailDto>> DeleteAsync(int id,
       int loginId,
       CancellationToken cancellationToken = default);

        Task<ApiResponse<EmployeeDetailDto>> RecoverAsync(
        int id,
        int loginId,
        CancellationToken cancellationToken = default);
    }
}
