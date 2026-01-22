using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;
using static Dapper.SqlMapper;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class SysBusinessActivityRepo : ISysBusinessActivityRepo
    {
        private readonly IDapperContext _context;

     //   private static readonly Dictionary<string, string> SortColumnMap =
     //new(StringComparer.OrdinalIgnoreCase)
     //{
     //    ["name"] = "s.BusinessActivityName",
     //    ["active"] = "s.IsActive",
     //    ["delete"] = "s.IsDeleted",
     //    ["createdUser"] = "cu.FirstName",
     //    ["modifiedUser"] = "uu.FirstName",
     //    ["deletedUser"] = "du.FirstName"
     //};




        public SysBusinessActivityRepo(IDapperContext context)
        {
            _context = context;
        }

   

        public async Task<bool> DeleteAsync(int id,int deletedBy, CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE SysBusinessActivity SET IsDeleted = 1, DeletedAt = SYSDATETIME(),DeletedBy = @deletedBy  WHERE SysBusinessActivityId = @ActivityId";

            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(sql, new { ActivityId = id , deletedBy }, cancellationToken: cancellationToken));
            return affectedRows > 0;
        }


        public async Task<int> CreateAsync
            (SysBusinessActivity entity, 
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
                INSERT INTO SysBusinessActivity (BusinessActivityName,CreatedBy)
                VALUES (@BusinessActivityName,@CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            var cmd = new CommandDefinition(sql, new
            {
                entity.BusinessActivityName,
                entity.CreatedBy,
            }, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            int id = await connection.ExecuteScalarAsync<int>(cmd);      
            return id;
        }

        public async Task<SysBusinessActivity?> GetByIdAsync(
     int id,
     CancellationToken cancellationToken = default)
        {
            const string sql = @"
    SELECT 
        s.SysBusinessActivityId        ,
        s.BusinessActivityName         ,
        s.IsActive                     ,
        s.IsDeleted                    ,
        s.CreatedAt                  ,
        s.UpdatedAt                  ,
        s.DeletedAt                   ,

        -- Created User
        CONCAT(cu.FirstName, ' ', ISNULL(cu.LastName, '')) AS CreatedUserName,

        -- Updated User
        CONCAT(uu.FirstName, ' ', ISNULL(uu.LastName, '')) AS UpdatedUserName,

        -- Deleted User
        CONCAT(du.FirstName, ' ', ISNULL(du.LastName, '')) AS DeletedUserName

    FROM dbo.SysBusinessActivity s
    INNER JOIN dbo.Users cu ON cu.UserId = s.CreatedBy
    LEFT JOIN dbo.Users uu ON uu.UserId = s.UpdatedBy
    LEFT JOIN dbo.Users du ON du.UserId = s.DeletedBy
    WHERE s.SysBusinessActivityId = @ActivityId;
    ";

            using var connection = _context.CreateConnection();
            var entity= await connection.QuerySingleOrDefaultAsync<SysBusinessActivity>(
                new CommandDefinition(sql, new { ActivityId = id }, cancellationToken: cancellationToken)
            );
            return entity;
        }



        public async Task<IEnumerable<SysBusinessActivity?>> GetActiveAsync(
        CancellationToken cancellationToken = default)
            {
                const string sql = @"
                SELECT 
                SysBusinessActivityId,
                BusinessActivityName       
                FROM dbo.SysBusinessActivity 
                WHERE  isDeleted = 0 And isActive = 1 ORDER BY BusinessActivityName ASC;
                ";

                using var connection = _context.CreateConnection();
                var entity = await connection.QueryAsync<SysBusinessActivity>(
                    new CommandDefinition(sql,cancellationToken: cancellationToken)
                );
                return entity;
            }


       


        public async Task<PagedResultDto<SysBusinessActivity>> GetAllAsync(BusinessActivityFilterDTO filter, CancellationToken cancellationToken = default)
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
                [BusinessActivitySearchBy.UpdatedUser] = "CONCAT(uU.FirstName,' ',uU.LastName)",
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
            var sortKey = string.IsNullOrWhiteSpace(filter.SortBy) ? "ActivityId" : filter.SortBy;

            if (!sortMap.TryGetValue(sortKey, out var sortColumn))
                sortColumn = sortMap["ActivityId"];

          
            var countSql = "SELECT COUNT(1) " + baseSql;

    
            var itemsSql = $@"
            SELECT
                spa.SysBusinessActivityId, spa.BusinessActivityName, spa.CreatedAt, spa.IsActive, spa.UpdatedAt, spa.IsDeleted, spa.DeletedAt, uC.UserId AS CreatedById, CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName, uU.UserId AS UpdatedById, CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName, uD.UserId AS DeletedById, CONCAT(uD.FirstName, ' ', uD.LastName) AS DeletedUserName
            {baseSql}
            ORDER BY {sortColumn} {sortOrder}
            OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;
            ";

            using var connection = _context.CreateConnection();

            var totalCount = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

            var items = await connection.QueryAsync<SysBusinessActivity>(
                new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken));

            return new PagedResultDto<SysBusinessActivity>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = limit
            };
        }






        public async Task<bool> UpdateAsync(SysBusinessActivity entity, CancellationToken cancellationToken = default)
        {
            var sets = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(entity.BusinessActivityName))
            { 
                sets.Add("BusinessActivityName = @BusinessActivityName");
                parameters.Add("BusinessActivityName", entity.BusinessActivityName);
            }

            if (entity.IsActive.HasValue)
            {
                sets.Add("IsActive = @Active");
                parameters.Add("Active", entity.IsActive.Value ? 1 : 0);
            }

            // If nothing to update, return false (no-op)
            if (sets.Count == 0)
                return false;

            // always set UpdatedBy/UpdatedAt
            sets.Add("UpdatedBy = @UpdatedBy");
            sets.Add("UpdatedAt = SYSDATETIME()");
            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("ActivityId", entity.SysBusinessActivityId);

            var sql = new StringBuilder();
            sql.Append("UPDATE SysBusinessActivity SET ");
            sql.Append(string.Join(", ", sets));
            sql.Append(" WHERE SysBusinessActivityId = @ActivityId AND IsDeleted = 0;");

            var cmd = new CommandDefinition(sql.ToString(), parameters, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            var affected = await connection.ExecuteAsync(cmd);
            return affected > 0;
        }

        public async Task<SysBusinessActivity> BusinessActivityExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var sql = "SELECT * FROM SysBusinessActivity WHERE BusinessActivityName = @ActivityName";
            if (excludeId.HasValue)
                sql += " AND SysBusinessActivityId <> @ExcludeId";

            var cmd = new CommandDefinition(sql, new { ActivityName = name, ExcludeId = excludeId }, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            var entity = await connection.QueryFirstOrDefaultAsync<SysBusinessActivity>(cmd);
            return entity;
        }

        public async Task<int>RecoverBusinessActivityAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE SysBusinessActivity SET IsDeleted = 0 ,DeletedBy = NULL, DeletedAt = NULL , UpdatedBy = @LoginId, UpdatedAt = SYSDATETIME() WHERE SysBusinessActivityId = @ActivityId";
            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(sql, new { ActivityId = id, LoginId = loginId }, cancellationToken: cancellationToken));
            return affectedRows;
        }

    }
}
