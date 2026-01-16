using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Data.Context;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Basic;
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
        const string sql = @"SELECT TOP 1 * FROM SysProgramActions WHERE SysProgramActionId = @Id;";

        var cmd = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SysProgramActions>(cmd);
    }

    public async Task<IEnumerable<SysProgramActions>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default)
    {
        var sql = new StringBuilder("SELECT * FROM SysProgramActions WHERE 1=1");
        var parameters = new DynamicParameters();

        // deleted filter (default exclude deleted)
        if (filter.IsDeleted.HasValue)
        {
            sql.Append(" AND IsActive = @IsActive");
            parameters.Add("IsActive", filter.IsDeleted.Value ? 1 : 0);
        }
        else
        {
            sql.Append(" AND IsDeleted = 0");
        }

        

        // search by name (use SortBy field for generic use? keep simple)
        // If caller uses SortBy as a search term, prefer a separate search param — here we support SortBy as column.
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            var sortOrder = string.Equals(filter.SortOrder, "desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
            var allowedSort = new[] { "SysProgramActionId", "ProgramActionName", "CreatedAt", "UpdatedAt", "IsActive" };
            if (allowedSort.Contains(filter.SortBy))
            {
                sql.Append($" ORDER BY {filter.SortBy} {sortOrder}");
            }
        }
        else
        {
            sql.Append(" ORDER BY SysProgramActionId DESC");
        }

        // paging
        if (filter.Limit > 0)
        {
            var offset = (Math.Max(filter.Page, 1) - 1) * filter.Limit;
            sql.Append(" OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY");
            parameters.Add("Offset", offset);
            parameters.Add("Limit", filter.Limit);
        }

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

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT COUNT(1) FROM SysProgramActions WHERE ProgramActionName = @Name";
        if (excludeId.HasValue)
            sql += " AND SysProgramActionId <> @ExcludeId";

        var cmd = new CommandDefinition(sql, new { Name = name, ExcludeId = excludeId }, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(cmd);
        return count > 0;
    }
}