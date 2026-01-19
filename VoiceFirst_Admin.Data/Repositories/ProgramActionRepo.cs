using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using VoiceFirst_Admin.Data.Context;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;
using static Dapper.SqlMapper;

namespace VoiceFirst_Admin.Data.Repositories;

public class ProgramActionRepo : IProgramActionRepo
{
    private readonly IDapperContext _context;

    public ProgramActionRepo(IDapperContext context)
    {
        _context = context;
    }

    public async Task<SysProgramActions> CreateAsync(SysProgramActions entity, CancellationToken cancellationToken = default)
    {
        const string sql = @"
                INSERT INTO SysProgramActions (ProgramActionName, CreatedBy)
                VALUES (@ProgramActionName, @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

        var cmd = new CommandDefinition(sql, new
        {
            entity.ProgramActionName,
            entity.CreatedBy,
        }, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        var id = await connection.ExecuteScalarAsync<int>(cmd);
        entity.SysProgramActionId = id;
        return entity;
    }

    public async Task<SysProgramActions?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT
            spa.SysProgramActionId,
            spa.ProgramActionName,
            spa.CreatedAt,
            spa.isActive,
            spa.UpdatedAt,
            spa.IsDeleted,
            spa.DeletedAt,

            uC.UserId   AS CreatedById,
            CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,

            uU.UserId   AS UpdatedById,
            CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName,

            uD.UserId   AS DeletedById,
            CONCAT(uD.FirstName, ' ', uD.LastName) AS DeletedUserName
        FROM SysProgramActions spa
        INNER JOIN Users uC ON uC.UserId = spa.CreatedBy
        LEFT JOIN Users uU ON uU.UserId = spa.UpdatedBy
        LEFT JOIN Users uD ON uD.UserId = spa.DeletedBy
         WHERE SysProgramActionId = @Id;";

        var cmd = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SysProgramActions>(cmd);
    }

    public async Task<PagedResultDto<SysProgramActions>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default)
    {
        var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
        var limit = filter.Limit <= 0 ? 10 : filter.Limit;
        var offset = (page - 1) * limit;

        var parameters = new DynamicParameters();
        parameters.Add("Offset", offset);
        parameters.Add("Limit", limit);

        // FROM + WHERE (shared)
        var baseSql = new StringBuilder(@"
FROM SysProgramActions spa
INNER JOIN Users uC ON uC.UserId = spa.CreatedBy
LEFT JOIN Users uU ON uU.UserId = spa.UpdatedBy
LEFT JOIN Users uD ON uD.UserId = spa.DeletedBy WHERE 1=1
            ");

        // filters (apply ONLY here so both count + items match)
        if (filter.Deleted.HasValue)
        {
            baseSql.Append(" AND spa.IsDeleted = @IsDeleted");
            parameters.Add("IsDeleted", filter.Deleted.Value);
        }

        if (filter.Active.HasValue)
        {
            baseSql.Append(" AND spa.IsActive = @Active");
            parameters.Add("Active", filter.Active.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            baseSql.Append(@"
             AND (
                    spa.ProgramActionName LIKE @Search
                 OR uC.FirstName LIKE @Search OR uC.LastName LIKE @Search
                 OR uU.FirstName LIKE @Search OR uU.LastName LIKE @Search
                 OR uD.FirstName LIKE @Search OR uD.LastName LIKE @Search
             )
            ");
            parameters.Add("Search", $"%{filter.SearchText}%");
        }

        // sorting (items only)
        var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["ActionId"] = "spa.SysProgramActionId",
            ["ActionName"] = "spa.ProgramActionName",
            ["Active"] = "spa.IsActive",
            ["Deleted"] = "spa.IsDeleted",
            ["CreatedDate"] = "spa.CreatedAt",
            ["ModifiedDate"] = "spa.UpdatedAt",
            ["DeletedDate"] = "spa.DeletedAt",
        };

        var sortOrder = string.Equals(filter.SortOrder, "desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
        var sortKey = string.IsNullOrWhiteSpace(filter.SortBy) ? "ActionId" : filter.SortBy;

        if (!sortMap.TryGetValue(sortKey, out var sortColumn))
            sortColumn = sortMap["ActionId"];
        
        // COUNT (no ORDER BY / paging)
        var countSql = "SELECT COUNT(1) " + baseSql;

        // ITEMS (select + order + paging)
        var itemsSql = $@"
            SELECT
                spa.SysProgramActionId, spa.ProgramActionName, spa.CreatedAt, spa.IsActive, spa.UpdatedAt, spa.IsDeleted, spa.DeletedAt, uC.UserId AS CreatedById, CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName, uU.UserId AS UpdatedById, CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName, uD.UserId AS DeletedById, CONCAT(uD.FirstName, ' ', uD.LastName) AS DeletedUserName
            {baseSql}
            ORDER BY {sortColumn} {sortOrder}
            OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;
            ";

        using var connection = _context.CreateConnection();

        var totalCount = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

        var items = await connection.QueryAsync<SysProgramActions>(
            new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken));

        return new PagedResultDto<SysProgramActions>
        {
            Items = items.ToList(),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = limit
        };
    }


    public async Task<bool> UpdateAsync(SysProgramActions entity, CancellationToken cancellationToken = default)
    {
        var sets = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(entity.ProgramActionName))
        {
            sets.Add("ProgramActionName = @ProgramActionName");
            parameters.Add("ProgramActionName", entity.ProgramActionName);
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
        parameters.Add("Id", entity.SysProgramActionId);

        var sql = new StringBuilder();
        sql.Append("UPDATE SysProgramActions SET ");
        sql.Append(string.Join(", ", sets));
        sql.Append(" WHERE SysProgramActionId = @Id AND IsDeleted = 0;");

        var cmd = new CommandDefinition(sql.ToString(), parameters, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        var affected = await connection.ExecuteAsync(cmd);
        return affected > 0;
    }
    public async Task<bool> DeleteAsync(SysProgramActions entity, CancellationToken cancellationToken = default)
    {
        const string sql = @"UPDATE SysProgramActions SET IsDeleted = @IsDeleted, DeletedAt = SYSDATETIME(),DeletedBy=@DeletedBy  WHERE SysProgramActionId = @Id";
        var parameters = new DynamicParameters();
        parameters.Add("Id", entity.SysProgramActionId);
        parameters.Add("DeletedBy", entity.DeletedBy);
        parameters.Add("IsDeleted", entity.IsDeleted);
        var cmd = new CommandDefinition(sql.ToString(), parameters, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        var affected = await connection.ExecuteAsync(cmd);
        return affected > 0;
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT COUNT(1) FROM SysProgramActions WHERE ProgramActionName = @Name AND IsDeleted=0";
        if (excludeId.HasValue)
            sql += " AND SysProgramActionId <> @ExcludeId";

        var cmd = new CommandDefinition(sql, new { Name = name, ExcludeId = excludeId }, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(cmd);
        return count > 0;
    }
}