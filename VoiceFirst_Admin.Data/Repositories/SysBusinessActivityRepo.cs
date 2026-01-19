using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class SysBusinessActivityRepo : ISysBusinessActivityRepo
    {
        private readonly IDapperContext _context;

        private static readonly Dictionary<string, string> SortColumnMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = "s.SysBusinessActivityId",
            ["name"] = "s.BusinessActivityName",
            ["createdDate"] = "s.CreatedAt",
            ["modifiedDate"] = "s.UpdatedAt",
            ["deletedDate"] = "s.DeletedAt",
            ["active"] = "s.IsActive",
            ["delete"] = "s.IsDeleted",
            ["createdUser"] = "cu.FirstName",
            ["modifiedUser"] = "uu.FirstName",
            ["deletedUser"] = "du.FirstName"
        };



        public SysBusinessActivityRepo(IDapperContext context)
        {
            _context = context;
        }

   

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE SysBusinessActivity SET Active = 0 ,Deleted = 1, DeletedAt = SYSDATETIME()  WHERE SysBusinessActivityId = @Id";

            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
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
        CONCAT(cu.FirstName, ' ', ISNULL(cu.LastName, '')) AS CreatedUser,

        -- Updated User
        CONCAT(uu.FirstName, ' ', ISNULL(uu.LastName, '')) AS UpdatedUser,

        -- Deleted User
        CONCAT(du.FirstName, ' ', ISNULL(du.LastName, '')) AS DeletedUser

    FROM dbo.SysBusinessActivity s
    INNER JOIN dbo.Users cu ON cu.UserId = s.CreatedBy
    LEFT JOIN dbo.Users uu ON uu.UserId = s.UpdatedBy
    LEFT JOIN dbo.Users du ON du.UserId = s.DeletedBy
    WHERE s.SysBusinessActivityId = @Id;
    ";

            using var connection = _context.CreateConnection();
            var entity= await connection.QuerySingleOrDefaultAsync<SysBusinessActivity>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken)
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
                WHERE  isDeleted = 0 And isActive = 1;
                ";

                using var connection = _context.CreateConnection();
                var entity = await connection.QueryAsync<SysBusinessActivity>(
                    new CommandDefinition(sql,cancellationToken: cancellationToken)
                );
                return entity;
            }


        private static string BuildUserCondition(string? sortBy)
        {
            return sortBy?.ToLower() switch
            {
                "createduser" =>
                    "s.CreatedBy IS NOT NULL AND (cu.FirstName LIKE @Search OR cu.LastName LIKE @Search)",

                "modifieduser" =>
                    "s.UpdatedBy IS NOT NULL AND (uu.FirstName LIKE @Search OR uu.LastName LIKE @Search)",

                "deleteduser" =>
                    "s.DeletedBy IS NOT NULL AND (du.FirstName LIKE @Search OR du.LastName LIKE @Search)",

                _ => @"
            (cu.FirstName LIKE @Search OR cu.LastName LIKE @Search)
            OR (uu.FirstName LIKE @Search OR uu.LastName LIKE @Search)
            OR (du.FirstName LIKE @Search OR du.LastName LIKE @Search)
        "
            };
        }



        public async Task<PagedResultDto<SysBusinessActivity>> GetAllAsync(
       CommonFilterDto1 filter,
       CancellationToken cancellationToken = default)
        {
            var baseSql = new StringBuilder(@"
        FROM dbo.SysBusinessActivity s
        INNER JOIN dbo.Users cu ON cu.UserId = s.CreatedBy
        LEFT JOIN dbo.Users uu ON uu.UserId = s.UpdatedBy
        LEFT JOIN dbo.Users du ON du.UserId = s.DeletedBy
        WHERE 1 = 1
    ");

            var parameters = new DynamicParameters();

            // 🔐 Active filter
            if (filter.Active.HasValue)
            {
                baseSql.Append(" AND s.IsActive = @IsActive");
                parameters.Add("IsActive", filter.Active.Value);
            }

            // 🔐 Delete filter
            if (filter.Delete.HasValue)
            {
                baseSql.Append(" AND s.IsDeleted = @IsDeleted");
                parameters.Add("IsDeleted", filter.Delete.Value);
            }

            // 🔍 Search
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                bool isUserSort =
                    filter.SortBy?.Equals("createdUser", StringComparison.OrdinalIgnoreCase) == true ||
                    filter.SortBy?.Equals("modifiedUser", StringComparison.OrdinalIgnoreCase) == true ||
                    filter.SortBy?.Equals("deletedUser", StringComparison.OrdinalIgnoreCase) == true;

                bool isNameSort =
                    filter.SortBy?.Equals("name", StringComparison.OrdinalIgnoreCase) == true;

                // ---------- DATE SEARCH ----------
                if (DateTime.TryParse(filter.Search, out var searchDate))
                {
                    string dateCondition = filter.SortBy?.ToLower() switch
                    {
                        "createddate" => "CONVERT(DATE, s.CreatedAt) = @SearchDate",
                        "modifieddate" => "CONVERT(DATE, s.UpdatedAt) = @SearchDate",
                        "deleteddate" => "CONVERT(DATE, s.DeletedAt) = @SearchDate",
                        _ => @"
                    CONVERT(DATE, s.CreatedAt) = @SearchDate
                    OR CONVERT(DATE, s.UpdatedAt) = @SearchDate
                    OR CONVERT(DATE, s.DeletedAt) = @SearchDate
                "
                    };

                    baseSql.Append($@"
                AND (
                    {(
                                isNameSort
                                    ? "s.BusinessActivityName LIKE @Search"
                                    : $"{BuildUserCondition(filter.SortBy)} OR {dateCondition}"
                            )}
                )
            ");

                    parameters.Add("SearchDate", searchDate.Date);
                }
                // ---------- TEXT / USER / NAME SEARCH ----------
                else
                {
                    baseSql.Append($@"
                AND (
                    {(
                                isNameSort
                                    ? "s.BusinessActivityName LIKE @Search"
                                    : BuildUserCondition(filter.SortBy)
                            )}
                )
            ");
                }

                parameters.Add("Search", $"%{filter.Search}%");
            }

            // 🔢 Total count
            var countSql = $"SELECT COUNT(1) {baseSql}";

            // 🔃 Sorting (SAFE)
            var sortColumn = SortColumnMap.TryGetValue(
                filter.SortBy ?? string.Empty,
                out var column)
                ? column
                : "s.SysBusinessActivityId";

            var sortOrder = filter.SortOrder == SortDirection.Asc ? "ASC" : "DESC";

            // 📄 Data query
            var dataSql = new StringBuilder(@"
        SELECT
            s.SysBusinessActivityId,
            s.BusinessActivityName,
            s.IsActive,
            s.CreatedBy,
            s.CreatedAt,
            s.UpdatedBy,
            s.UpdatedAt,
            s.IsDeleted,
            s.DeletedBy,
            s.DeletedAt,
            CONCAT(cu.FirstName, ' ', ISNULL(cu.LastName, '')) AS CreatedUser,
            CONCAT(uu.FirstName, ' ', ISNULL(uu.LastName, '')) AS UpdatedUser,
            CONCAT(du.FirstName, ' ', ISNULL(du.LastName, '')) AS DeletedUser
    ");

            dataSql.Append(baseSql);
            dataSql.Append($" ORDER BY {sortColumn} {sortOrder}");
            dataSql.Append(" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");

            var offset = (Math.Max(filter.PageNumber, 1) - 1) * filter.PageSize;
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", filter.PageSize);

            using var connection = _context.CreateConnection();

            var totalCount = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

            var items = await connection.QueryAsync<SysBusinessActivity>(
                new CommandDefinition(dataSql.ToString(), parameters, cancellationToken: cancellationToken));

            return new PagedResultDto<SysBusinessActivity>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
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
                sets.Add("Active = @Active");
                parameters.Add("Active", entity.IsActive.Value ? 1 : 0);
            }

            // If nothing to update, return false (no-op)
            if (sets.Count == 0)
                return false;

            // always set UpdatedBy/UpdatedAt
            sets.Add("UpdatedBy = @UpdatedBy");
            sets.Add("UpdatedAt = SYSDATETIME()");
            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("Id", entity.SysBusinessActivityId);

            var sql = new StringBuilder();
            sql.Append("UPDATE SysBusinessActivity SET ");
            sql.Append(string.Join(", ", sets));
            sql.Append(" WHERE SysBusinessActivityId = @Id AND Deleted = 0;");

            var cmd = new CommandDefinition(sql.ToString(), parameters, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            var affected = await connection.ExecuteAsync(cmd);
            return affected > 0;
        }

        public async Task<bool> BusinessActivityExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var sql = "SELECT COUNT(1) FROM SysBusinessActivity WHERE BusinessActivityName = @Name";
            if (excludeId.HasValue)
                sql += " AND SysBusinessActivityId <> @ExcludeId";

            var cmd = new CommandDefinition(sql, new { Name = name, ExcludeId = excludeId }, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(cmd);
            return count > 0;
        }


    }
}
