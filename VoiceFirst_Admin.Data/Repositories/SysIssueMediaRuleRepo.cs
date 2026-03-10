using Dapper;
using System.Data;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories;

public class SysIssueMediaRuleRepo : ISysIssueMediaRuleRepo
{
    public async Task<int> CreateAsync(
        SysIssueMediaRule rule,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO SysIssueMediaRule (IssueTypeId, IssueMediaFormatId, [Min], [Max], MaxSizeMB, CreatedBy)
            VALUES (@IssueTypeId, @IssueMediaFormatId, @Min, @Max, @MaxSizeMB, @CreatedBy);
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new
            {
                rule.IssueTypeId,
                rule.IssueMediaFormatId,
                rule.Min,
                rule.Max,
                rule.MaxSizeMB,
                rule.CreatedBy
            }, transaction: transaction, cancellationToken: ct));
    }

    public async Task<bool> BulkInsertAsync(
        int issueTypeId,
        IEnumerable<IssueMediaRuleCreateDTO> dtos,
        int createdBy,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct = default)
    {
        if (dtos == null || !dtos.Any()) return false;

        const string sql = @"
            INSERT INTO SysIssueMediaRule (IssueTypeId, IssueMediaFormatId, [Min], [Max], MaxSizeMB, CreatedBy)
            VALUES (@IssueTypeId, @IssueMediaFormatId, @Min, @Max, @MaxSizeMB, @CreatedBy);";

        var parameters = dtos.Select(d => new
        {
            IssueTypeId = issueTypeId,
            IssueMediaFormatId = d.IssueMediaFormatId,
            Min = d.Min,
            Max = d.Max,
            MaxSizeMB = d.MaxSizeMB,
            CreatedBy = createdBy
        });

        var rows = await connection.ExecuteAsync(
            new CommandDefinition(sql, parameters, transaction: transaction, cancellationToken: ct));
        return rows > 0;
    }

    public async Task<bool> BulkUpdateAsync(
        IEnumerable<SysIssueMediaRule> entities,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct = default)
    {
        if (entities == null || !entities.Any()) return false;

        const string sql = @"
            UPDATE SysIssueMediaRule
            SET 
                [Min] = CASE WHEN @Min IS NOT NULL THEN @Min ELSE [Min] END,
                [Max] = CASE WHEN @Max IS NOT NULL THEN @Max ELSE [Max] END,
                MaxSizeMB = CASE WHEN @MaxSizeMB IS NOT NULL THEN @MaxSizeMB ELSE MaxSizeMB END,
                IsActive = CASE WHEN @IsActive IS NOT NULL THEN @IsActive ELSE IsActive END,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = SYSDATETIME()
            WHERE IssueTypeId = @IssueTypeId 
              AND IssueMediaFormatId = @IssueMediaFormatId
              AND (
                    (@Min IS NOT NULL AND ISNULL([Min], -1) <> ISNULL(@Min, -1))
                 OR (@Max IS NOT NULL AND ISNULL([Max], -1) <> ISNULL(@Max, -1))
                 OR (@MaxSizeMB IS NOT NULL AND ISNULL(MaxSizeMB, -1) <> ISNULL(@MaxSizeMB, -1))
                 OR (@IsActive IS NOT NULL AND ISNULL(IsActive, 0) <> ISNULL(@IsActive, 0))
              );";

        var rows = await connection.ExecuteAsync(
            new CommandDefinition(sql, entities, transaction: transaction, cancellationToken: ct));
        return rows > 0;
    }

    public async Task<bool> UpdateAsync(
        SysIssueMediaRule rule,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct = default)
    {
        const string sql = @"
            UPDATE SysIssueMediaRule
            SET [Min] = @Min,
                [Max] = @Max,
                MaxSizeMB = @MaxSizeMB,
                IsActive = @IsActive,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = SYSDATETIME()
            WHERE SysIssueMediaRuleId = @Id;";

        var affected = await connection.ExecuteAsync(
            new CommandDefinition(sql, new
            {
                Min = rule.Min,
                Max = rule.Max,
                MaxSizeMB = rule.MaxSizeMB,
                IsActive = rule.IsActive,
                UpdatedBy = rule.UpdatedBy,
                Id = rule.SysIssueMediaRuleId
            }, transaction: transaction, cancellationToken: ct));
        return affected > 0;
    }



        public async Task<bool>
              CheckMediaFormatLinksExistAsync(
                          int issueTypeId,
                  IEnumerable<int> formatIds,
                  bool update,
                  IDbConnection connection,
                  IDbTransaction transaction,
                  CancellationToken cancellationToken = default)
        {
            // If no IDs are sent, treat as invalid
            if (issueTypeId == null || !formatIds.Any())
                return false;

            const string sql = @"
                        SELECT COUNT(1)
                        FROM SysIssueMediaRule
                        WHERE IssueTypeId = @IssueTypeId AND IssueMediaFormatId IN @Ids
                    ";

            var exists = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    sql,
                    new { IssueTypeId = issueTypeId, Ids = formatIds },
                    transaction,
                    cancellationToken: cancellationToken
                ));
            if (!update)
            {
                // INSERT case
                // true  → already exists (block insert)
                // false → safe to insert
                return exists > 0;
            }

            // UPDATE case
            // true  → all records exist
            // false → some records missing
            return exists == formatIds.Count();
        }



    public async Task<BulkValidationResult> IsBulkExistAsync(
        int issueTypeId,
        IEnumerable<int> formatIds,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct = default)
    {
        var result = new BulkValidationResult();

        if (formatIds == null || !formatIds.Any())
            return result;

        const string sql = @"
            SELECT IssueMediaFormatId, IsActive FROM SysIssueMediaRule
            WHERE IssueTypeId = @IssueTypeId AND IssueMediaFormatId IN @Ids ;";

        var entities = (await connection.QueryAsync<dynamic>(
            new CommandDefinition(sql, new { IssueTypeId = issueTypeId, Ids = formatIds },
            transaction: transaction, cancellationToken: ct))).ToList();

        return new BulkValidationResult
        {
            IdNotFound = entities.Count != formatIds.Distinct().Count(),
            IsInactive = entities.Any(e => e.IsActive == false)
        };
    }

    public async Task<IEnumerable<SysIssueMediaRule>> GetByIssueTypeAndFormatsAsync(
        int issueTypeId,
        IEnumerable<int> formatIds,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct = default)
    {
        if (formatIds == null || !formatIds.Any())
            return Enumerable.Empty<SysIssueMediaRule>();

        const string sql = @"SELECT * FROM SysIssueMediaRule WHERE IssueTypeId = @IssueTypeId AND IssueMediaFormatId IN @Ids;";
        return await connection.QueryAsync<SysIssueMediaRule>(
            new CommandDefinition(sql, new { IssueTypeId = issueTypeId, Ids = formatIds },
            transaction: transaction, cancellationToken: ct));
    }

    public async Task<SysIssueMediaRule?> GetByIssueTypeAndFormatAsync(
        int issueTypeId,
        int formatId,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct = default)
    {
        const string sql = @"
            SELECT TOP 1 * FROM SysIssueMediaRule
            WHERE IssueTypeId = @IssueTypeId AND IssueMediaFormatId = @IssueMediaFormatId;";

        return await connection.QuerySingleOrDefaultAsync<SysIssueMediaRule>(
            new CommandDefinition(sql, new { IssueTypeId = issueTypeId, IssueMediaFormatId = formatId },
            transaction: transaction, cancellationToken: ct));
    }
}
