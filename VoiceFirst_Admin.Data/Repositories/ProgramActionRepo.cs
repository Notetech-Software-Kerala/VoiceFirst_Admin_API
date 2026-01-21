using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using VoiceFirst_Admin.Data.Context;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction;
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
    public async Task<IEnumerable<SysProgramActions>> GetLookupAsync( CancellationToken cancellationToken = default)
    {
        var sql = new StringBuilder(@"
        SELECT
            SysProgramActionId,
            ProgramActionName 
        FROM SysProgramActions
        WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY ProgramActionName ASC
        ");

        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<SysProgramActions>(
            new CommandDefinition(sql.ToString(), cancellationToken: cancellationToken));
    }

    public async Task<SysProgramActions?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT
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
            baseSql.Append(" AND spa.IsActive = @IsActive");
            parameters.Add("IsActive", filter.Active.Value);
        }
        // CreatedAt
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

        // UpdatedAt
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

        // DeletedAt
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

        var searchByMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["ActionName"] = "spa.ProgramActionName",
            ["CreatedUser"] = "CONCAT(uC.FirstName, ' ', uC.LastName)",
            ["ModifiedUser"] = "CONCAT(uU.FirstName, ' ', uU.LastName)",
            ["DeletedUser"] = "CONCAT(uD.FirstName, ' ', uD.LastName)",
        };

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            if (!string.IsNullOrWhiteSpace(filter.SearchBy) &&
                searchByMap.TryGetValue(filter.SearchBy, out var col))
            {
                baseSql.Append($" AND {col} LIKE @Search");
                parameters.Add("Search", $"%{filter.SearchText}%");
            }
            else
            {
                // default: search across multiple columns
                baseSql.Append(@"
            AND (
                spa.ProgramActionName LIKE @Search
             OR uC.FirstName LIKE @Search OR uC.LastName LIKE @Search
             OR uU.FirstName LIKE @Search OR uU.LastName LIKE @Search
             OR uD.FirstName LIKE @Search OR uD.LastName LIKE @Search
            )");
                parameters.Add("Search", $"%{filter.SearchText}%");
            }
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

        var sortOrder = filter.SortOrder == SortOrder.Desc ? "DESC" : "ASC";
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
            sets.Add("IsActive = @IsActive");
            parameters.Add("IsActive", entity.IsActive.Value ? 1 : 0);
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
        const string sql = @"UPDATE SysProgramActions SET IsDeleted = 1, DeletedAt = SYSDATETIME(),DeletedBy=@DeletedBy  WHERE SysProgramActionId = @Id AND IsDeleted = 0;";
        var parameters = new DynamicParameters();
        parameters.Add("Id", entity.SysProgramActionId);
        parameters.Add("DeletedBy", entity.DeletedBy);
        var cmd = new CommandDefinition(sql.ToString(), parameters, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        var affected = await connection.ExecuteAsync(cmd);
        return affected > 0;
    }
    public async Task<bool> RestoreAsync(SysProgramActions entity, CancellationToken cancellationToken = default)
    {
        const string sql = @"UPDATE SysProgramActions SET IsDeleted = 0, DeletedAt = NULL,DeletedBy=NULL,UpdatedBy = @UpdatedBy,UpdatedAt = SYSDATETIME()  WHERE SysProgramActionId = @Id AND IsDeleted = 1;";
        var parameters = new DynamicParameters();
        parameters.Add("Id", entity.SysProgramActionId);
        parameters.Add("UpdatedBy", entity.UpdatedBy);
        var cmd = new CommandDefinition(sql.ToString(), parameters, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        var affected = await connection.ExecuteAsync(cmd);
        return affected > 0;
    }
    public async Task<SysProgramActions> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM SysProgramActions WHERE ProgramActionName = @Name";
        if (excludeId.HasValue)
            sql += " AND SysProgramActionId <> @ExcludeId";

        var cmd = new CommandDefinition(sql, new { Name = name, ExcludeId = excludeId }, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        return await connection.ExecuteScalarAsync<SysProgramActions>(cmd);
     
    }
   
}