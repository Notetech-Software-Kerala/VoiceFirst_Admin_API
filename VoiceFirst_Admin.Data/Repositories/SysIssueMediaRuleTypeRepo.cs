using Dapper;
using System.Data;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories;

public class SysIssueMediaRuleTypeRepo : ISysIssueMediaRuleTypeRepo
{
    public async Task<int> CreateAsync(
        SysIssueMediaRuleType entity,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO SysIssueMediaRuleType (IssueMediaRuleId, IssueMediaTypeId, IsMandatory, CreatedBy)
            VALUES (@IssueMediaRuleId, @IssueMediaTypeId, @IsMandatory, @CreatedBy);
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new
            {
                entity.IssueMediaRuleId,
                entity.IssueMediaTypeId,
                entity.IsMandatory,
                entity.CreatedBy
            }, transaction: transaction, cancellationToken: ct));
    }

    public async Task<bool> BulkInsertAsync(
        int ruleId,
        IEnumerable<IssueMediaRuleTypeCreateDTO> mediaTypes,
        int createdBy,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct = default)
    {
        if (mediaTypes == null || !mediaTypes.Any()) return false;

        const string sql = @"
            INSERT INTO SysIssueMediaRuleType (IssueMediaRuleId, IssueMediaTypeId, IsMandatory, CreatedBy)
            VALUES (@IssueMediaRuleId, @IssueMediaTypeId, @IsMandatory, @CreatedBy);";

        var parameters = mediaTypes.Select(mt => new
        {
            IssueMediaRuleId = ruleId,
            mt.IssueMediaTypeId,
            mt.IsMandatory,
            CreatedBy = createdBy
        });

        var rows = await connection.ExecuteAsync(
            new CommandDefinition(sql, parameters, transaction: transaction, cancellationToken: ct));
        return rows > 0;
    }

    public async Task<bool> BulkInsertAsync(
        IEnumerable<SysIssueMediaRuleType> entities,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct = default)
    {
        if (entities == null || !entities.Any()) return false;

        const string sql = @"
            INSERT INTO SysIssueMediaRuleType (IssueMediaRuleId, IssueMediaTypeId, IsMandatory, CreatedBy)
            VALUES (@IssueMediaRuleId, @IssueMediaTypeId, @IsMandatory, @CreatedBy);";

        var parameters = entities.Select(d => new
        {
            d.IssueMediaRuleId,
            d.IssueMediaTypeId,
            d.IsMandatory,
            d.CreatedBy
        });

        var rows = await connection.ExecuteAsync(
            new CommandDefinition(sql, parameters, transaction: transaction, cancellationToken: ct));
        return rows > 0;
    }

   
    public async Task<int> BulkUpdateAsync(
    IEnumerable<SysIssueMediaRuleType> entities,
    IDbConnection connection,
    IDbTransaction transaction,
    CancellationToken ct = default)
    {
        if (entities == null || !entities.Any())
            return 0;

        int affectedRows = 0;

        foreach (var entity in entities)
        {
            var setClauses = new List<string>();

            if (entity.IsMandatory.HasValue)
                setClauses.Add("IsMandatory = @IsMandatory");

            if (entity.IsActive.HasValue)
                setClauses.Add("IsActive = @IsActive");

            setClauses.Add("UpdatedBy = @UpdatedBy");
            setClauses.Add("UpdatedAt = SYSDATETIME()");

            var sql = $@"
            UPDATE SysIssueMediaRuleType
            SET {string.Join(", ", setClauses)}
            WHERE IssueMediaRuleId = @IssueMediaRuleId
            AND IssueMediaTypeId = @IssueMediaTypeId";

            affectedRows += await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    entity,
                    transaction: transaction,
                    cancellationToken: ct
                ));
        }

        return affectedRows;
    }




    public async Task<SysIssueMediaRuleType?> GetAsync(
        int ruleId,
        int mediaTypeId,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct = default)
    {
        const string sql = @"
            SELECT TOP 1 * FROM SysIssueMediaRuleType
            WHERE IssueMediaRuleId = @RuleId AND IssueMediaTypeId = @IssueMediaTypeId;";

        return await connection.QuerySingleOrDefaultAsync<SysIssueMediaRuleType>(
            new CommandDefinition(sql, new { RuleId = ruleId, IssueMediaTypeId = mediaTypeId },
            transaction: transaction, cancellationToken: ct));
    }

    public async Task<bool> UpdateAsync(
        SysIssueMediaRuleType entity,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct = default)
    {
        const string sql = @"
            UPDATE SysIssueMediaRuleType
            SET IsMandatory = @IsMandatory,
                IsActive = @IsActive,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = SYSDATETIME()
            WHERE SysIssueMediaRuleTypeId = @Id;";

        var affected = await connection.ExecuteAsync(
            new CommandDefinition(sql, new
            {
                entity.IsMandatory,
                entity.IsActive,
                entity.UpdatedBy,
                Id = entity.SysIssueMediaRuleTypeId
            }, transaction: transaction, cancellationToken: ct));
        return affected > 0;
    }

    public async Task<IEnumerable<SysIssueMediaRuleType>> GetByRuleIdsAsync(
        IEnumerable<int> ruleIds,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct = default)
    {
        if (ruleIds == null || !ruleIds.Any())
            return Enumerable.Empty<SysIssueMediaRuleType>();

        const string sql = @"
            SELECT SysIssueMediaRuleTypeId, IssueMediaRuleId, IssueMediaTypeId, IsMandatory, IsActive, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
            FROM SysIssueMediaRuleType
            WHERE IssueMediaRuleId IN @RuleIds;";

        return await connection.QueryAsync<SysIssueMediaRuleType>(
            new CommandDefinition(sql, new { RuleIds = ruleIds },
            transaction: transaction, cancellationToken: ct));
    }
}
