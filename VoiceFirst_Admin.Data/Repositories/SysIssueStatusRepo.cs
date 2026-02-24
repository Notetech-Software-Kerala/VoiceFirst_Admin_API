using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueStatus;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class SysIssueStatusRepo : ISysIssueStatusRepo
    {
        private readonly IDapperContext _context;

        public SysIssueStatusRepo(IDapperContext context)
        {
            _context = context;
        }

        public async Task<SysIssueStatusDTO> IssueStatusExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var sql = "SELECT IsDeleted As Deleted, SysIssueStatusId As IssueStatusId FROM SysIssueStatus WHERE IssueStatus = @Name";
            if (excludeId.HasValue) sql += " AND SysIssueStatusId <> @ExcludeId";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SysIssueStatusDTO>(new CommandDefinition(sql, new { Name = name, ExcludeId = excludeId }, cancellationToken: cancellationToken));
        }

        public async Task<int> CreateAsync(SysIssueStatus entity, CancellationToken cancellationToken = default)
        {
            const string sql = @"INSERT INTO SysIssueStatus (IssueStatus, CreatedBy) VALUES (@IssueStatus, @CreatedBy); SELECT CAST(SCOPE_IDENTITY() AS int);";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { entity.IssueStatus, entity.CreatedBy }, cancellationToken: cancellationToken));
        }

        public async Task<SysIssueStatusDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT s.SysIssueStatusId As IssueStatusId, s.IssueStatus, s.IsActive As Active, s.IsDeleted As Deleted,
                    s.CreatedAt As CreatedDate, s.UpdatedAt As ModifiedDate, s.DeletedAt As DeletedDate,
                    CONCAT(cu.FirstName,' ',ISNULL(cu.LastName,'')) AS CreatedUser,
                    CONCAT(uu.FirstName,' ',ISNULL(uu.LastName,'')) AS ModifiedUser,
                    CONCAT(du.FirstName,' ',ISNULL(du.LastName,'')) AS DeletedUser
                FROM dbo.SysIssueStatus s
                INNER JOIN dbo.Users cu ON cu.UserId = s.CreatedBy
                LEFT JOIN dbo.Users uu ON uu.UserId = s.UpdatedBy
                LEFT JOIN dbo.Users du ON du.UserId = s.DeletedBy
                WHERE s.SysIssueStatusId = @Id;";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<SysIssueStatusDTO>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }

        public async Task<SysIssueStatusDTO> IsIdExistAsync(int id, CancellationToken cancellationToken = default)
        {
            const string sql = "SELECT SysIssueStatusId As IssueStatusId, IsDeleted As Deleted FROM dbo.SysIssueStatus WHERE SysIssueStatusId = @Id;";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<SysIssueStatusDTO>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }

        public async Task<PagedResultDto<SysIssueStatusDTO>> GetAllAsync(IssueStatusFilterDTO filter, CancellationToken cancellationToken = default)
        {
            var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var limit = filter.Limit <= 0 ? 10 : filter.Limit;
            var offset = (page - 1) * limit;
            var parameters = new DynamicParameters();
            parameters.Add("Offset", offset);
            parameters.Add("Limit", limit);

            var baseSql = new StringBuilder(@"FROM SysIssueStatus sit
                INNER JOIN Users uC ON uC.UserId = sit.CreatedBy
                LEFT JOIN Users uU ON uU.UserId = sit.UpdatedBy
                LEFT JOIN Users uD ON uD.UserId = sit.DeletedBy WHERE 1=1 ");

            if (filter.Deleted.HasValue) { baseSql.Append(" AND sit.IsDeleted = @IsDeleted"); parameters.Add("IsDeleted", filter.Deleted.Value); }
            if (filter.Active.HasValue) { baseSql.Append(" AND sit.IsActive = @IsActive"); parameters.Add("IsActive", filter.Active.Value); }
            if (!string.IsNullOrWhiteSpace(filter.CreatedFromDate) && DateTime.TryParse(filter.CreatedFromDate, out var cf)) { baseSql.Append(" AND sit.CreatedAt >= @CreatedFrom"); parameters.Add("CreatedFrom", cf); }
            if (!string.IsNullOrWhiteSpace(filter.CreatedToDate) && DateTime.TryParse(filter.CreatedToDate, out var ct)) { baseSql.Append(" AND sit.CreatedAt < DATEADD(day,1,@CreatedTo)"); parameters.Add("CreatedTo", ct.Date); }
            if (!string.IsNullOrWhiteSpace(filter.UpdatedFromDate) && DateTime.TryParse(filter.UpdatedFromDate, out var uf)) { baseSql.Append(" AND sit.UpdatedAt >= @UpdatedFrom"); parameters.Add("UpdatedFrom", uf); }
            if (!string.IsNullOrWhiteSpace(filter.UpdatedToDate) && DateTime.TryParse(filter.UpdatedToDate, out var ut)) { baseSql.Append(" AND sit.UpdatedAt < DATEADD(day,1,@UpdatedTo)"); parameters.Add("UpdatedTo", ut.Date); }
            if (!string.IsNullOrWhiteSpace(filter.DeletedFromDate) && DateTime.TryParse(filter.DeletedFromDate, out var df)) { baseSql.Append(" AND sit.DeletedAt >= @DeletedFrom"); parameters.Add("DeletedFrom", df); }
            if (!string.IsNullOrWhiteSpace(filter.DeletedToDate) && DateTime.TryParse(filter.DeletedToDate, out var dt2)) { baseSql.Append(" AND sit.DeletedAt < DATEADD(day,1,@DeletedTo)"); parameters.Add("DeletedTo", dt2.Date); }

            var searchByMap = new Dictionary<IssueStatusSearchBy, string>
            {
                [IssueStatusSearchBy.IssueStatus] = "sit.IssueStatus",
                [IssueStatusSearchBy.CreatedUser] = "CONCAT(uC.FirstName,' ',uC.LastName)",
                [IssueStatusSearchBy.ModifiedUser] = "CONCAT(uU.FirstName,' ',uU.LastName)",
                [IssueStatusSearchBy.DeletedUser] = "CONCAT(uD.FirstName,' ',uD.LastName)"
            };
            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                if (filter.SearchBy.HasValue && searchByMap.TryGetValue(filter.SearchBy.Value, out var col))
                    baseSql.Append($" AND {col} LIKE @Search");
                else
                    baseSql.Append(@" AND (sit.IssueStatus LIKE @Search OR uC.FirstName LIKE @Search OR uC.LastName LIKE @Search OR uU.FirstName LIKE @Search OR uU.LastName LIKE @Search OR uD.FirstName LIKE @Search OR uD.LastName LIKE @Search)");
                parameters.Add("Search", $"%{filter.SearchText}%");
            }

            var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["IssueStatusId"] = "sit.SysIssueStatusId", ["IssueStatus"] = "sit.IssueStatus",
                ["Active"] = "sit.IsActive", ["Deleted"] = "sit.IsDeleted",
                ["CreatedDate"] = "sit.CreatedAt", ["ModifiedDate"] = "sit.UpdatedAt", ["DeletedDate"] = "sit.DeletedAt",
            };
            var sortOrder = filter.SortOrder == SortOrder.Desc ? "DESC" : "ASC";
            var sortKey = string.IsNullOrWhiteSpace(filter.SortBy) ? "IssueStatus" : filter.SortBy;
            if (!sortMap.TryGetValue(sortKey, out var sortColumn)) sortColumn = sortMap["IssueStatusId"];

            var countSql = "SELECT COUNT(1) " + baseSql;
            var itemsSql = $@"SELECT sit.SysIssueStatusId AS IssueStatusId, sit.IssueStatus, sit.IsActive AS Active, sit.IsDeleted AS Deleted,
                sit.CreatedAt AS CreatedDate, sit.UpdatedAt AS ModifiedDate, sit.DeletedAt AS DeletedDate,
                CONCAT(uC.FirstName,' ',ISNULL(uC.LastName,'')) AS CreatedUser,
                ISNULL(CONCAT(uU.FirstName,' ',ISNULL(uU.LastName,'')),'') AS ModifiedUser,
                ISNULL(CONCAT(uD.FirstName,' ',ISNULL(uD.LastName,'')),'') AS DeletedUser
                {baseSql} ORDER BY {sortColumn} {sortOrder} OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;";

            using var connection = _context.CreateConnection();
            var totalCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));
            var items = await connection.QueryAsync<SysIssueStatusDTO>(new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken));
            return new PagedResultDto<SysIssueStatusDTO> { Items = items.ToList(), TotalCount = totalCount, PageNumber = page, PageSize = limit };
        }

        public async Task<List<SysIssueStatusActiveDTO?>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            const string sql = "SELECT SysIssueStatusId As IssueStatusId, IssueStatus FROM dbo.SysIssueStatus WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY IssueStatus ASC;";
            using var connection = _context.CreateConnection();
            var entity = await connection.QueryAsync<SysIssueStatusActiveDTO?>(new CommandDefinition(sql, cancellationToken: cancellationToken));
            return entity.ToList();
        }

        public async Task<bool> UpdateAsync(SysIssueStatus entity, CancellationToken cancellationToken = default)
        {
            var sets = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("IssueStatus", entity.IssueStatus);
            parameters.Add("Active", entity.IsActive.HasValue ? (entity.IsActive.Value ? 1 : 0) : (int?)null);
            if (!string.IsNullOrWhiteSpace(entity.IssueStatus)) sets.Add("IssueStatus = @IssueStatus");
            if (entity.IsActive.HasValue) sets.Add("IsActive = @Active");
            if (sets.Count == 0) return false;
            sets.Add("UpdatedBy = @UpdatedBy"); sets.Add("UpdatedAt = SYSDATETIME()");
            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("Id", entity.SysIssueStatusId);
            var sql = $@"UPDATE SysIssueStatus SET {string.Join(", ", sets)} WHERE SysIssueStatusId = @Id AND IsDeleted = 0
                AND ((@IssueStatus IS NOT NULL AND IssueStatus <> @IssueStatus) OR (@Active IS NOT NULL AND IsActive <> @Active));";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken)) > 0;
        }

        public async Task<bool> DeleteAsync(int id, int deletedBy, CancellationToken cancellationToken = default)
        {
            const string sql = "UPDATE SysIssueStatus SET IsDeleted = 1, DeletedAt = SYSDATETIME(), DeletedBy = @deletedBy WHERE SysIssueStatusId = @Id AND IsDeleted = 0;";
            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open) connection.Open();
            return await connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id, deletedBy }, cancellationToken: cancellationToken)) > 0;
        }

        public async Task<bool> RecoverIssueStatusAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {
            const string sql = "UPDATE SysIssueStatus SET IsDeleted = 0, DeletedBy = NULL, DeletedAt = NULL, UpdatedBy = @LoginId, UpdatedAt = SYSDATETIME() WHERE SysIssueStatusId = @Id AND IsDeleted = 1";
            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open) connection.Open();
            return await connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id, LoginId = loginId }, cancellationToken: cancellationToken)) > 0;
        }
    }
}
