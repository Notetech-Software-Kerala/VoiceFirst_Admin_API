using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Enums;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;
using static Dapper.SqlMapper;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class SysBusinessActivityRepo : ISysBusinessActivityRepo
    {
        private readonly IDapperContext _context;

    

        public SysBusinessActivityRepo(IDapperContext context)
        {
            _context = context;
        }


        public async Task<SysBusinessActivityDTO> BusinessActivityExistsAsync
            (string name, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var sql = "SELECT IsDeleted As Deleted,SysBusinessActivityId As ActivityId  FROM SysBusinessActivity WHERE BusinessActivityName = @ActivityName";
            if (excludeId.HasValue)
                sql += " AND SysBusinessActivityId <> @ExcludeId";

            var cmd = new CommandDefinition(sql, new { ActivityName = name, ExcludeId = excludeId }, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            var entity = await connection.QueryFirstOrDefaultAsync<SysBusinessActivityDTO>(cmd);
            return entity;
        }


        public async Task<int> CreateAsync
            (SysBusinessActivity entity, List<int> CustomFieldIds,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
                INSERT INTO SysBusinessActivity (BusinessActivityName,CreatedBy)
                VALUES (@BusinessActivityName,@CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

               
                using var connection = _context.CreateConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                var cmd = new CommandDefinition(sql, new
                {
                    entity.BusinessActivityName,
                    entity.CreatedBy,
                }, transaction: transaction, cancellationToken: cancellationToken);

                int id = await connection.ExecuteScalarAsync<int>(cmd);

                if (CustomFieldIds != null && CustomFieldIds.Count > 0)
                {
                    await InsertCustomFieldLinksAsync(connection, transaction, id, entity.CreatedBy, CustomFieldIds, cancellationToken);
                }

                transaction.Commit();

                return id;
            }
            catch
            {
                try { transaction.Rollback(); } catch { }
                return 0;
            }
        }
        public async Task<bool> UpdateAsync(
                SysBusinessActivity entity,
                List<int>? addCustomFieldIds,
                List<SysBusinessActivityUserCustomFieldLink>? updateCustomFieldIds,
                CancellationToken cancellationToken = default)
        {
            using var connection = _context.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                await UpdateBusinessActivityAsync(connection, transaction, entity, cancellationToken);
                
                await InsertCustomFieldLinksAsync(connection, transaction, entity.SysBusinessActivityId, entity.UpdatedBy??0, addCustomFieldIds, cancellationToken);
                await UpdateCustomFieldLinksAsync(connection, transaction, entity.SysBusinessActivityId, entity.UpdatedBy ?? 0, updateCustomFieldIds, cancellationToken);

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private async Task UpdateBusinessActivityAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            SysBusinessActivity entity,
            CancellationToken cancellationToken)
        {
            var sets = new List<string>();
            var parameters = new DynamicParameters();

            parameters.Add("BusinessActivityName", entity.BusinessActivityName);
            parameters.Add("Active", entity.IsActive.HasValue
                ? (entity.IsActive.Value ? 1 : 0)
                : (int?)null);

            if (!string.IsNullOrWhiteSpace(entity.BusinessActivityName))
                sets.Add("BusinessActivityName = @BusinessActivityName");

            if (entity.IsActive.HasValue)
                sets.Add("IsActive = @Active");

            if (sets.Count == 0)
                return;

            sets.Add("UpdatedBy = @UpdatedBy");
            sets.Add("UpdatedAt = SYSDATETIME()");

            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("ActivityId", entity.SysBusinessActivityId);

            var sql = $@"
UPDATE SysBusinessActivity
SET {string.Join(", ", sets)}
WHERE SysBusinessActivityId = @ActivityId
  AND IsDeleted = 0
  AND (
        (@BusinessActivityName IS NOT NULL AND BusinessActivityName <> @BusinessActivityName)
     OR (@Active IS NOT NULL AND IsActive <> @Active)
  );";

            await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    parameters,
                    transaction: transaction,
                    cancellationToken: cancellationToken));
        }

        private async Task InsertCustomFieldLinksAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            int SysBusinessActivityId,
            int CreatedBy,
            List<int>? addCustomFieldIds,
            CancellationToken cancellationToken)
        {
            if (addCustomFieldIds == null || addCustomFieldIds.Count == 0)
                return;

            const string sql = @"
INSERT INTO SysBusinessActivityUserCustomFieldLink
    (SysBusinessActivityId, SysUserCustomFieldId, CreatedBy)
VALUES
    (@ActivityId, @FieldId, @CreatedBy);";

            foreach (var fieldId in addCustomFieldIds.Distinct())
            {
                await connection.ExecuteAsync(
                    new CommandDefinition(
                        sql,
                        new
                        {
                            ActivityId = SysBusinessActivityId,
                            FieldId = fieldId,
                            CreatedBy = CreatedBy
                        },
                        transaction: transaction,
                        cancellationToken: cancellationToken));
            }
        }

        private async Task UpdateCustomFieldLinksAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            int SysBusinessActivityId,
            int UpdatedBy,
            List<SysBusinessActivityUserCustomFieldLink>? updateCustomFieldIds,
            CancellationToken cancellationToken)
        {
            if (updateCustomFieldIds == null || updateCustomFieldIds.Count == 0)
                return;

            const string sql = @"
UPDATE SysBusinessActivityUserCustomFieldLink
SET
    IsActive = @IsActive,
    UpdatedBy = @UpdatedBy,
    UpdatedAt = SYSDATETIME()
WHERE SysBusinessActivityUserCustomFieldLinkId = @LinkId
  AND SysBusinessActivityId = @ActivityId";

            foreach (var item in updateCustomFieldIds)
            {
                await connection.ExecuteAsync(
                    new CommandDefinition(
                        sql,
                        new
                        {
                            LinkId = item.SysBusinessActivityUserCustomFieldLinkId,
                            ActivityId = SysBusinessActivityId,
                            IsActive = item.IsActive,
                            UpdatedBy = UpdatedBy 
                        },
                        transaction: transaction,
                        cancellationToken: cancellationToken));
            }
        }

        public async Task<SysBusinessActivityDTO?> GetByIdAsync(
          int ActivityId,
          CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT 
                    s.SysBusinessActivityId    As ActivityId    ,
                    s.BusinessActivityName      As ActivityName   ,
                    s.IsActive             As Active        ,
                    s.IsDeleted            As Deleted      ,
                    s.CreatedAt             As CreatedDate    ,
                    s.UpdatedAt             As   ModifiedDate  ,
                    s.DeletedAt              As   DeletedDate  ,

                    -- Created User
                    CONCAT(cu.FirstName, ' ', ISNULL(cu.LastName, '')) AS CreatedUser,

                    -- Updated User
                    CONCAT(uu.FirstName, ' ', ISNULL(uu.LastName, '')) AS ModifiedUser,

                    -- Deleted User
                    CONCAT(du.FirstName, ' ', ISNULL(du.LastName, '')) AS DeletedUser

                FROM dbo.SysBusinessActivity s
                INNER JOIN dbo.Users cu ON cu.UserId = s.CreatedBy
                LEFT JOIN dbo.Users uu ON uu.UserId = s.UpdatedBy
                LEFT JOIN dbo.Users du ON du.UserId = s.DeletedBy
                WHERE s.SysBusinessActivityId = @ActivityId;
                ";

            using var connection = _context.CreateConnection();
            var entity = await connection.QuerySingleOrDefaultAsync<SysBusinessActivityDTO>(
                new CommandDefinition(sql, new { ActivityId = ActivityId }, cancellationToken: cancellationToken)
            );
            return entity;
        }
        public async Task<IEnumerable<SysBusinessActivityUserCustomFieldLink?>> GetCustomFieldByIdAsync(int ActivityId, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT 
                    s.SysBusinessActivityUserCustomFieldLinkId,
                    s.SysBusinessActivityId  ,
                    s.SysUserCustomFieldId  ,
                    cf.FieldDataType  ,
                    cf.FieldName  ,
                    s.IsActive ,
                    s.CreatedAt ,
                    s.UpdatedAt ,

                    -- Created User
                    CONCAT(cu.FirstName, ' ', ISNULL(cu.LastName, '')) AS CreatedUserName,

                    -- Updated User
                    CONCAT(uu.FirstName, ' ', ISNULL(uu.LastName, '')) AS UpdatedUserName


                FROM dbo.SysBusinessActivityUserCustomFieldLink s
                INNER JOIN dbo.Users cu ON cu.UserId = s.CreatedBy
                INNER JOIN dbo.SysUserCustomField cf ON cf.SysUserCustomFieldId = s.SysUserCustomFieldId
                LEFT JOIN dbo.Users uu ON uu.UserId = s.UpdatedBy
                WHERE s.SysBusinessActivityId = @ActivityId;
                ";

            using var connection = _context.CreateConnection();
            var entity = await connection.QueryAsync<SysBusinessActivityUserCustomFieldLink>(
                new CommandDefinition(sql, new { ActivityId = ActivityId }, cancellationToken: cancellationToken)
            );
            return entity;
        }




        public async Task<SysBusinessActivityDTO> IsIdExistAsync(
          int activityId,
          CancellationToken cancellationToken = default)
            {
                const string sql = @"
                SELECT  s.SysBusinessActivityId    As ActivityId ,
                        s.IsDeleted            As Deleted      
                FROM dbo.SysBusinessActivity s
                WHERE SysBusinessActivityId = @ActivityId
                 ;
                 ";

                using var connection = _context.CreateConnection();

                var dto = await connection.QuerySingleOrDefaultAsync<SysBusinessActivityDTO>(
                    new CommandDefinition(
                        sql,
                        new { ActivityId = activityId },
                        cancellationToken: cancellationToken
                    )
                );
                return dto;
            }
        public async Task<SysBusinessActivityUserCustomFieldLink> IsCustomFieldExistByActivityAsync(
          int activityId,int customFieldId,
          CancellationToken cancellationToken = default)
        {
            const string sql = @"
        SELECT *
        FROM SysBusinessActivityUserCustomFieldLink
        WHERE SysUserCustomFieldId = @SysUserCustomFieldId
          AND  SysBusinessActivityId = @ActivityId";

            using var connection = _context.CreateConnection();

            var dto = await connection.QuerySingleOrDefaultAsync<SysBusinessActivityUserCustomFieldLink>(
                new CommandDefinition(
                    sql,
                    new
                    {
                        SysUserCustomFieldId = customFieldId,
                        ActivityId = activityId
                    },
                    cancellationToken: cancellationToken
                )
            );

            return dto;
        }
        public async Task<SysBusinessActivityUserCustomFieldLink?> GetCustomFieldLinkByIdAsync(
    int customFieldLinkId,
    int? activityId,
    CancellationToken cancellationToken = default)
        {
            const string sql = @"
        SELECT *
        FROM SysBusinessActivityUserCustomFieldLink
        WHERE SysBusinessActivityUserCustomFieldLinkId = @CustomFieldLinkId
          AND (@ActivityId IS NULL OR SysBusinessActivityId = @ActivityId)";

            using var connection = _context.CreateConnection();

            var dto = await connection.QuerySingleOrDefaultAsync<SysBusinessActivityUserCustomFieldLink>(
                new CommandDefinition(
                    sql,
                    new
                    {
                        CustomFieldLinkId = customFieldLinkId,
                        ActivityId = activityId
                    },
                    cancellationToken: cancellationToken
                )
            );

            return dto;
        }



        public async Task<PagedResultDto<SysBusinessActivityDTO>>
         GetAllAsync(BusinessActivityFilterDTO filter, CancellationToken cancellationToken = default)
        {
            var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var limit = filter.Limit <= 0 ? 10 : filter.Limit;
            var offset = (page - 1) * limit;

            var parameters = new DynamicParameters();
            parameters.Add("Offset", offset);
            parameters.Add("Limit", limit);


            var baseSql = new StringBuilder(@"
            FROM SysBusinessActivity spa
            INNER JOIN Users uC ON uC.UserId = spa.CreatedBy
            LEFT JOIN Users uU ON uU.UserId = spa.UpdatedBy
            LEFT JOIN Users uD ON uD.UserId = spa.DeletedBy WHERE 1=1
            ");

            if (filter.Deleted.HasValue)
            {
                baseSql.Append(" AND spa.IsDeleted = @IsDeleted");
                parameters.Add("IsDeleted", filter.Deleted.Value);
            }

            if (filter.Active.HasValue)
            {
                baseSql.Append(" AND spa.IsActive = @IsActive");
                parameters.Add("IsActive", filter.Active.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.CreatedFromDate) &&
                DateTime.TryParse(filter.CreatedFromDate, out var createdFrom))
            {
                baseSql.Append(" AND spa.CreatedAt >= @CreatedFrom");
                parameters.Add("CreatedFrom", createdFrom);
            }
            if (!string.IsNullOrWhiteSpace(filter.CreatedToDate) &&
                DateTime.TryParse(filter.CreatedToDate, out var createdTo))
            {
                baseSql.Append(" AND spa.CreatedAt < DATEADD(day, 1, @CreatedTo)");
                parameters.Add("CreatedTo", createdTo.Date);
            }


            if (!string.IsNullOrWhiteSpace(filter.UpdatedFromDate) &&
                DateTime.TryParse(filter.UpdatedFromDate, out var updatedFrom))
            {
                baseSql.Append(" AND spa.UpdatedAt >= @UpdatedFrom");
                parameters.Add("UpdatedFrom", updatedFrom);
            }
            if (!string.IsNullOrWhiteSpace(filter.UpdatedToDate) &&
                DateTime.TryParse(filter.UpdatedToDate, out var updatedTo))
            {
                baseSql.Append(" AND spa.UpdatedAt < DATEADD(day, 1, @UpdatedTo)");
                parameters.Add("UpdatedTo", updatedTo.Date);
            }


            if (!string.IsNullOrWhiteSpace(filter.DeletedFromDate) &&
                DateTime.TryParse(filter.DeletedFromDate, out var deletedFrom))
            {
                baseSql.Append(" AND spa.DeletedAt >= @DeletedFrom");
                parameters.Add("DeletedFrom", deletedFrom);
            }
            if (!string.IsNullOrWhiteSpace(filter.DeletedToDate) &&
                DateTime.TryParse(filter.DeletedToDate, out var deletedTo))
            {
                baseSql.Append(" AND spa.DeletedAt < DATEADD(day, 1, @DeletedTo)");
                parameters.Add("DeletedTo", deletedTo.Date);
            }


            var searchByMap = new Dictionary<BusinessActivitySearchBy, string>
            {
                [BusinessActivitySearchBy.ActivityName] = "spa.BusinessActivityName",
                [BusinessActivitySearchBy.CreatedUser] = "CONCAT(uC.FirstName,' ',uC.LastName)",
                [BusinessActivitySearchBy.ModifiedUser] = "CONCAT(uU.FirstName,' ',uU.LastName)",
                [BusinessActivitySearchBy.DeletedUser] = "CONCAT(uD.FirstName,' ',uD.LastName)"
            };

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                if (filter.SearchBy.HasValue && searchByMap.TryGetValue(filter.SearchBy.Value, out var col))
                    baseSql.Append($" AND {col} LIKE @Search");
                else
                    baseSql.Append(@"
            AND (
                spa.BusinessActivityName LIKE @Search
             OR uC.FirstName LIKE @Search OR uC.LastName LIKE @Search
             OR uU.FirstName LIKE @Search OR uU.LastName LIKE @Search
             OR uD.FirstName LIKE @Search OR uD.LastName LIKE @Search
            )");

                parameters.Add("Search", $"%{filter.SearchText}%");
            }



            var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {

                ["ActivityId"] = "spa.SysBusinessActivityId",
                ["ActivityName"] = "spa.BusinessActivityName",
                ["Active"] = "spa.IsActive",
                ["Deleted"] = "spa.IsDeleted",
                ["CreatedDate"] = "spa.CreatedAt",
                ["ModifiedDate"] = "spa.UpdatedAt",
                ["DeletedDate"] = "spa.DeletedAt",
            };

            var sortOrder = filter.SortOrder == SortOrder.Desc ? "DESC" : "ASC";
            // 🔹 CHANGED: Default sorting is now ProgramName (Alphabetical order)
            var sortKey = string.IsNullOrWhiteSpace(filter.SortBy)
                ? "ActivityName"
                : filter.SortBy;

            if (!sortMap.TryGetValue(sortKey, out var sortColumn))
                sortColumn = sortMap["ActivityId"];


            

            var countSql = "SELECT COUNT(1) " + baseSql;


            var itemsSql = $@"
            SELECT
                spa.SysBusinessActivityId,
                spa.BusinessActivityName,
                spa.CreatedAt,
                spa.IsActive,
                spa.UpdatedAt,
                spa.IsDeleted,
                spa.DeletedAt,
                uC.UserId AS CreatedById,
                CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
                uU.UserId AS UpdatedById,
                CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName,
                uD.UserId AS DeletedById,
                CONCAT(uD.FirstName, ' ', uD.LastName) AS DeletedUserName,
                -- Additional aliases to directly map as requested (non-breaking)
                spa.SysBusinessActivityId AS ActivityId,
                spa.BusinessActivityName AS ActivityName,
                spa.IsActive AS Active,
                spa.IsDeleted AS Deleted,
                spa.CreatedAt AS CreatedDate,
                spa.UpdatedAt AS ModifiedDate,
                spa.DeletedAt AS DeletedDate,
                CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
                ISNULL(CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')), '') AS ModifiedUser,
                ISNULL(CONCAT(uD.FirstName, ' ', ISNULL(uD.LastName, '')), '') AS DeletedUser
            {baseSql}
            ORDER BY {sortColumn} {sortOrder}
            OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;
            ";

            using var connection = _context.CreateConnection();

            var totalCount = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

            var items = await connection.QueryAsync<SysBusinessActivityDTO>(
                new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken));

            return new PagedResultDto<SysBusinessActivityDTO>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = limit
            };
        }



        public async Task<List<SysBusinessActivityActiveDTO?>> GetActiveAsync(
         CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT 
                SysBusinessActivityId As ActivityId,
                BusinessActivityName  As ActivityName     
                FROM dbo.SysBusinessActivity 
                WHERE  isDeleted = 0 And isActive = 1 ORDER BY BusinessActivityName ASC;
                ";

            using var connection = _context.CreateConnection();
            var entity = await connection.QueryAsync<SysBusinessActivityActiveDTO?>(
                new CommandDefinition(sql, cancellationToken: cancellationToken)
            );
            return entity.ToList();
        }


      
        



        public async Task<bool> DeleteAsync
            (int id, int deletedBy,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE SysBusinessActivity SET IsDeleted = 1, DeletedAt = SYSDATETIME(),DeletedBy = @deletedBy  WHERE SysBusinessActivityId = @ActivityId And IsDeleted = 0;";

            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(sql, new { ActivityId = id, deletedBy }, cancellationToken: cancellationToken));
            return affectedRows > 0;
        }

       // public async Task<SysBusinessActivityDTO> DeleteAsync(
       //int id,
       //int deletedBy,
       //CancellationToken cancellationToken = default)
       // {
       //     const string sql = @"
       //       DECLARE @Result TABLE (Status INT);

       //     UPDATE dbo.SysBusinessActivity
       //     SET IsDeleted = 1,
       //         DeletedAt = SYSUTCDATETIME(),
       //         DeletedBy = @deletedBy
       //     OUTPUT 
       //         INSERTED.SysBusinessActivityId,
       //         INSERTED.Name,
       //         INSERTED.IsDeleted,
       //         INSERTED.DeletedAt,
       //         U.UserName AS DeletedByName


       //             s.SysBusinessActivityId    As ActivityId    ,
       //             s.BusinessActivityName      As ActivityName   ,
       //             s.IsActive             As Active        ,
       //             s.IsDeleted            As Deleted      ,
       //             s.CreatedAt             As CreatedDate    ,
       //             s.UpdatedAt             As   ModifiedDate  ,
       //             s.DeletedAt              As   DeletedDate  ,

       //             -- Created User
       //             CONCAT(cu.FirstName, ' ', ISNULL(cu.LastName, '')) AS CreatedUser,

       //             -- Updated User
       //             CONCAT(uu.FirstName, ' ', ISNULL(uu.LastName, '')) AS ModifiedUser,

       //             -- Deleted User
       //             CONCAT(du.FirstName, ' ', ISNULL(du.LastName, '')) AS DeletedUser

       //         FROM dbo.SysBusinessActivity s
       //         INNER JOIN dbo.Users cu ON cu.UserId = s.CreatedBy
       //         LEFT JOIN dbo.Users uu ON uu.UserId = s.UpdatedBy
       //         LEFT JOIN dbo.Users du ON du.UserId = s.DeletedBy


       //     FROM dbo.SysBusinessActivity SBA
       //     JOIN dbo.Users U
       //         ON U.UserId = @deletedBy
       //     WHERE SysBusinessActivityId = @ActivityId
       //         AND IsDeleted = 0;

       //     IF NOT EXISTS (SELECT 1 FROM @Result)
       //     BEGIN
       //         INSERT INTO @Result
       //         SELECT CASE 
       //             WHEN EXISTS (
       //                 SELECT 1 
       //                 FROM dbo.SysBusinessActivity 
       //                 WHERE SysBusinessActivityId = @ActivityId
       //             )
       //             THEN 1
       //             ELSE 0
       //         END
       //     END

       //     SELECT TOP 1 Status FROM @Result;

       //         ";

       //     using var connection = _context.CreateConnection();


       //     var sysBusinessActivityDTO = await connection.QuerySingleAsync<DeleteResult>(
       //         new CommandDefinition(
       //             sql,
       //             new { ActivityId = id, deletedBy },
       //             cancellationToken: cancellationToken));

       //     return result;
       // }



        public async Task<bool>RecoverBusinessActivityAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE SysBusinessActivity SET IsDeleted = 0 ,DeletedBy = NULL, DeletedAt = NULL , UpdatedBy = @LoginId, UpdatedAt = SYSDATETIME() WHERE SysBusinessActivityId = @ActivityId And IsDeleted = 1";
            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(sql, new { ActivityId = id, LoginId = loginId }, cancellationToken: cancellationToken));
            return affectedRows > 0;
        }

    }
}
