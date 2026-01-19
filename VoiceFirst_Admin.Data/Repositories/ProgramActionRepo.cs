using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Data.Context;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

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

    public async Task<IEnumerable<SysProgramActions>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default)
    {
        var sql = new StringBuilder(@"SELECT
    spa.SysProgramActionId,
    spa.ProgramActionName,
    spa.CreatedAt,
    spa.IsActive,
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
LEFT JOIN Users uD ON uD.UserId = spa.DeletedBy WHERE 1=1");
        var parameters = new DynamicParameters();
        var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
        var limit = filter.Limit <= 0 ? 10 : filter.Limit;
        var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["ActionId"] = "SysProgramActionId",
            ["ActionName"] = "ProgramActionName",
            ["Active"] = "IsActive",
            ["Deleted"] = "IsDeleted",
            ["CreatedDate"] = "CreatedAt",
            ["ModifiedDate"] = "UpdatedAt",
            ["DeletedDate"] = "DeletedAt",
            ["CreatedUser"] = "CreatedBy",
            ["ModifiedUser"] = "UpdatedBy",
            ["DeletedUser"] = "DeletedBy",
        };
        // deleted filter (default exclude deleted)
        if (filter.Deleted.HasValue)
        {
            sql.Append(" AND spa.IsDeleted = @IsDeleted");
            parameters.Add("IsDeleted", filter.Deleted.Value ? 1: 0);
        }
        // deleted filter (default exclude deleted)
        if (filter.Active.HasValue)
        {
            sql.Append(" AND spa.IsActive = @IsActive");
            parameters.Add("IsActive", filter.Active.Value ? 1 : 0);
        }
        

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            sql.Append(" AND spa.ProgramActionName LIKE @Search");
            parameters.Add("Search", $"%{filter.SearchText}%");
        }

        // search by name (use SortBy field for generic use? keep simple)
        // If caller uses SortBy as a search term, prefer a separate search param — here we support SortBy as column.


        // normalize sort order
        var sortOrder = string.Equals(filter.SortOrder, "desc", StringComparison.OrdinalIgnoreCase)
            ? "DESC"
            : "ASC";

        // default sort key (client name)
        var sortKey = string.IsNullOrWhiteSpace(filter.SortBy) ? "ActionId" : filter.SortBy;

        // convert to db column (fallback to default)
        if (!sortMap.TryGetValue(sortKey, out var sortColumn))
            sortColumn = sortMap["ActionId"];

        sql.Append($" ORDER BY spa.{sortColumn} {sortOrder}");


        // paging
        var offset = (page - 1) * limit;
        sql.Append(" OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY");
        parameters.Add("Offset", offset);
        parameters.Add("Limit", limit);

        var cmd = new CommandDefinition(sql.ToString(), parameters, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<SysProgramActions>(cmd);
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
        sql.Append(" WHERE SysProgramActionId = @Id AND Deleted = 0;");

        var cmd = new CommandDefinition(sql.ToString(), parameters, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        var affected = await connection.ExecuteAsync(cmd);
        return affected > 0;
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT COUNT(1) FROM SysProgramActions WHERE ProgramActionName = @Name";
        if (excludeId.HasValue)
            sql += " AND ActionId <> @ExcludeId";

        var cmd = new CommandDefinition(sql, new { Name = name, ExcludeId = excludeId }, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(cmd);
        return count > 0;
    }
}