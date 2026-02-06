using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.UserRoleLink;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface IUserRoleLinkRepo
    {
        Task<bool> BulkUpdateUserRoleLinksAsync(
           int UserId,
           IEnumerable<UserRoleLinkUpdateDto> dtos,
           int updatedBy,
           IDbConnection connection,
           IDbTransaction tx,
           CancellationToken cancellationToken);
        Task<bool> BulkInsertUserRoleLinksAsync(
        int userId,
        IEnumerable<int> roleIds,
        int createdBy,
        IDbConnection connection,
        IDbTransaction tx,
        CancellationToken cancellationToken);

        Task<bool>
           CheckUserRoleLinksExistAsync(
                       int userId,
               IEnumerable<int> roleIds,
               bool update,
               IDbConnection connection,
               IDbTransaction transaction,
               CancellationToken cancellationToken = default);

        Task<IEnumerable<UserRoleLinksDto>>
       GetRoleLinksByUserIdAsync(
            int userId, 
            IDbConnection connection,
            IDbTransaction transaction, 
            CancellationToken cancellationToken = default);
    }
}
