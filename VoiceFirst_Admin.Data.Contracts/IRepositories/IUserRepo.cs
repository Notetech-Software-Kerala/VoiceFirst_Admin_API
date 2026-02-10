using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.UserRoleLink;
using VoiceFirst_Admin.Utilities.DTOs.Features.Users;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface IUserRepo
    {



        Task<Users?> GetUserByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
         Users entity,
         IDbConnection connection,
         IDbTransaction transaction,
         CancellationToken cancellationToken = default);


        Task<PagedResultDto<EmployeeDto>> GetAllAsync(
        EmployeeFilterDto filter,
        int loginUserId,
        CancellationToken cancellationToken = default);


        Task<int> CreateAsync(
       Users user,
       IDbConnection connection,
       IDbTransaction transaction,
       CancellationToken cancellationToken);

        Task<EmployeeDto> IsIdExistAsync(
       int userId,
       CancellationToken cancellationToken = default);


        Task<bool>
            DeleteAsync(
            int id,
            int deletedBy,
            CancellationToken cancellationToken = default);


        Task<bool>
            RecoverAsync
            (int id,
            int loginId,
            CancellationToken cancellationToken = default);

        Task<EmployeeDetailDto?> GetByIdAsync
         (int id, IDbConnection connection,
           IDbTransaction transaction,
           CancellationToken cancellationToken = default);


       

        Task<Users> CheckEmailExistsAsync(
        string email,
        int? excludeId,
        CancellationToken cancellationToken);


        Task<Users> CheckMobileNoExistsAsync(
        string mobileNo,
        int? excludeId,
        CancellationToken cancellationToken);
    }
}
