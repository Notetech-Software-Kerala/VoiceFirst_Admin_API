using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.UserRoleLink;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class UserRoleLinkRepo: IUserRoleLinkRepo
    {
        private readonly IDapperContext _context;

        public UserRoleLinkRepo(IDapperContext context)
        {
            _context = context;
        }



        public async Task<bool> BulkInsertUserRoleLinksAsync(
         int userId,
         IEnumerable<int> roleIds,
         int createdBy,
         IDbConnection connection,
         IDbTransaction tx,
         CancellationToken cancellationToken)
        {
            if (roleIds == null || !roleIds.Any())
                return false;

            // -------------------------
            // CONVERT dynamic → int
            // -------------------------

            if (roleIds.Count() == 0)
                return false;

            // -------------------------
            // SQL
            // -------------------------
            const string sql = @"
            INSERT INTO UserRoleLink
                (UserId, SysRoleId, CreatedBy)
            VALUES
                (@UserId, @SysRoleId, @CreatedBy);";

            // -------------------------
            // PARAMETER OBJECTS
            // -------------------------
            var parameters = roleIds.Select(id => new
            {
                UserId = userId,
                SysRoleId = id,
                CreatedBy = createdBy
            });

            var rowsAffected = await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    parameters,
                    transaction: tx,
                    cancellationToken: cancellationToken));

            return rowsAffected > 0;
        }

     
        public async Task<bool>
            CheckUserRoleLinksExistAsync(
                        int userId,
                IEnumerable<int> roleIds,
                bool update,
                IDbConnection connection,
                IDbTransaction transaction,
                CancellationToken cancellationToken = default)
        {
            // If no IDs are sent, treat as invalid
            if (roleIds == null || !roleIds.Any())
                return false;

            const string sql = @"
                    SELECT COUNT(1)
                    FROM UserRoleLink
                    WHERE SysRoleId IN @roleIds And userId = @userId;
                ";

            var exists = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    sql,
                    new { roleIds = roleIds, userId = userId },
                    transaction,
                    cancellationToken: cancellationToken
                ));
            if (!update)
            {
                // INSERT case
                // true  → already exists (block insert)
                // false → safe to insert
                return exists > 0;
            }

            // UPDATE case
            // true  → all records exist
            // false → some records missing
            return exists == roleIds.Count();
        }


        public async Task<IEnumerable<UserRoleLinksDto>>
       GetRoleLinksByUserIdAsync(int userId, IDbConnection connection,
       IDbTransaction transaction, CancellationToken cancellationToken = default)
        {
            const string sql = @"
            SELECT 
                l.SysRoleId AS RoleId,
                a.RoleName AS RoleName,
                l.IsActive AS Active,
              
                CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
                l.CreatedAt AS CreatedDate,
                CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')) AS ModifiedUser,
                l.UpdatedAt AS ModifiedDate
            FROM UserRoleLink l
            INNER JOIN SysRoles a ON a.SysRoleId = l.SysRoleId
            INNER JOIN Users uC ON uC.UserId = l.CreatedBy
            LEFT JOIN Users uU ON uU.UserId = l.UpdatedBy
            WHERE l.UserId = @UserId ;
            ";

            return await connection.QueryAsync<UserRoleLinksDto>(
                new CommandDefinition(sql, new { UserId = userId },
                transaction, cancellationToken: cancellationToken));
        }

        public async Task<bool> BulkUpdateUserRoleLinksAsync(
           int UserId,
           IEnumerable<UserRoleLinkUpdateDto> dtos,
           int updatedBy,
           IDbConnection connection,
           IDbTransaction tx,
           CancellationToken cancellationToken)
        {
            const string sql = @"
        UPDATE UserRoleLink
        SET IsActive   = @IsActive,
            UpdatedBy = @UpdatedBy,
            UpdatedAt = SYSDATETIME()
        WHERE UserId = @UserId
          AND SysRoleId = @RoleId
          AND IsActive <> @IsActive;
        ";

            var parameters = dtos.Select(dto => new
            {
                UserId = UserId,
                RoleId = dto.RoleId,
                IsActive = dto.Active,
                UpdatedBy = updatedBy
            });

            var rowsAffected = await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    parameters,
                    transaction: tx,
                    cancellationToken: cancellationToken
                ));

            return rowsAffected > 0;
        }

    }
}
