using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
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
            ["id"] = "SysBusinessActivityId",
            ["name"] = "BusinessActivityName",
            ["createdAt"] = "CreatedAt",
            ["updatedAt"] = "UpdatedAt",
            ["isActive"] = "Active"
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

        public async Task<SysBusinessActivity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT TOP 1 * FROM SysBusinessActivity WHERE SysBusinessActivityId = @Id And Deleted = 0;";

            var cmd = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<SysBusinessActivity>(cmd);
        }

        //public async Task<IEnumerable<SysBusinessActivity>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default)
        //{
        //    var sql = new StringBuilder("SELECT * FROM SysBusinessActivity WHERE  Deleted = 0 1=1");
        //    var parameters = new DynamicParameters();

        //    // deleted filter (default exclude deleted)
        //    if (filter.Active.HasValue)
        //    {
        //        sql.Append(" AND Active = @Active");
        //        parameters.Add("Active", filter.Active.Value ? 1 : 0);
        //    }



        //    // search by name (use SortBy field for generic use? keep simple)
        //    // If caller uses SortBy as a search term, prefer a separate search param — here we support SortBy as column.
        //    if (!string.IsNullOrWhiteSpace(filter.SortBy))
        //    {
        //        var sortOrder = string.Equals(filter.SortOrder, "desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
        //        var allowedSort = new[] { "BusinessActivityName", "CreatedAt", "UpdatedAt", "Active" };
        //        if (allowedSort.Contains(filter.SortBy))
        //        {
        //            sql.Append($" ORDER BY {filter.SortBy} {sortOrder}");
        //        }
        //    }
        //    else
        //    {
        //        sql.Append(" ORDER BY SysBusinessActivityId DESC");
        //    }

        //    // paging
        //    if (filter.Limit > 0)
        //    {
        //        var offset = (Math.Max(filter.PageNumber, 1) - 1) * filter.Limit;
        //        sql.Append(" OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY");
        //        parameters.Add("Offset", offset);
        //        parameters.Add("Limit", filter.Limit);
        //    }

        //    var cmd = new CommandDefinition(sql.ToString(), parameters, cancellationToken: cancellationToken);
        //    using var connection = _context.CreateConnection();
        //    return await connection.QueryAsync<SysBusinessActivity>(cmd);
        //}


        public async Task<PagedResultDto<SysBusinessActivity>> GetAllAsync(
            CommonFilterDto1 filter,
            CancellationToken cancellationToken = default)
                {
                    var baseSql = new StringBuilder(@"
                FROM SysBusinessActivity
                WHERE Deleted = 0
            ");

            var parameters = new DynamicParameters();

            // 🔐 Active filter
            if (filter.IsActive.HasValue)
            {
                baseSql.Append(" AND Active = @Active");
                parameters.Add("Active", filter.IsActive.Value);
            }

            // 🔍 Search
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                baseSql.Append(" AND BusinessActivityName LIKE @Search");
                parameters.Add("Search", $"%{filter.Search}%");
            }

            // 🔢 Total count (enterprise requirement)
            var countSql = $"SELECT COUNT(1) {baseSql}";

            // 🔃 Sorting
            var sortColumn = SortColumnMap.TryGetValue(
                filter.SortBy ?? string.Empty,
                out var column)
                ? column
                : "SysBusinessActivityId";

            var sortOrder = filter.SortOrder == SortDirection.Asc ? "ASC" : "DESC";

            var dataSql = new StringBuilder();
            dataSql.Append("SELECT * ");
            dataSql.Append(baseSql);
            dataSql.Append($" ORDER BY {sortColumn} {sortOrder}");

            // 📄 Paging
            var offset = (Math.Max(filter.PageNumber, 1) - 1) * filter.PageSize;
            dataSql.Append(" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");

            parameters.Add("Offset", offset);
            parameters.Add("PageSize", filter.PageSize);

            using var connection = _context.CreateConnection();

            var totalCount = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

            var items = await connection.QueryAsync<SysBusinessActivity>(
                new CommandDefinition(dataSql.ToString(), parameters, cancellationToken: cancellationToken));

            return new PagedResultDto<SysBusinessActivity>
            {
                Items = items,
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
                sets.Add("BusinessActivity = @BusinessActivityName");
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
