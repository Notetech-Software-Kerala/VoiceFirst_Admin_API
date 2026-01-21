using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories;

public class PostOfficeRepo : IPostOfficeRepo
{
    private readonly IDapperContext _context;

    public PostOfficeRepo(IDapperContext context)
    {
        _context = context;
    }

    public async Task<PostOffice> CreateAsync(PostOffice entity, CancellationToken cancellationToken = default)
    {
        const string sql = @"
                INSERT INTO PostOffice (PostOfficeName, CountryId, CreatedBy)
                VALUES (@PostOfficeName, @CountryId, @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

        var cmd = new CommandDefinition(sql, new
        {
            entity.PostOfficeName,
            entity.CountryId,
            entity.CreatedBy,
        }, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        var id = await connection.ExecuteScalarAsync<int>(cmd);
        entity.PostOfficeId = id;
        return entity;
    }

    public async Task<PostOffice?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT
                po.PostOfficeId,
                po.PostOfficeName,
                po.CountryId,
                po.CreatedAt,
                po.IsActive,
                po.UpdatedAt,
                po.IsDeleted,
                po.DeletedAt,
                uC.UserId   AS CreatedById,
                CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
                uU.UserId   AS UpdatedById,
                CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName,
                uD.UserId   AS DeletedById,
                CONCAT(uD.FirstName, ' ', uD.LastName) AS DeletedUserName
            FROM PostOffice po
            INNER JOIN Users uC ON uC.UserId = po.CreatedBy
            LEFT JOIN Users uU ON uU.UserId = po.UpdatedBy
            LEFT JOIN Users uD ON uD.UserId = po.DeletedBy
            WHERE po.PostOfficeId = @Id;";

        var cmd = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<PostOffice>(cmd);
    }

    public async Task<PagedResultDto<PostOffice>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default)
    {
        var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
        var limit = filter.Limit <= 0 ? 10 : filter.Limit;
        var offset = (page - 1) * limit;

        var parameters = new DynamicParameters();
        parameters.Add("Offset", offset);
        parameters.Add("Limit", limit);

        var baseSql = new StringBuilder(@"
FROM PostOffice po
INNER JOIN Users uC ON uC.UserId = po.CreatedBy
LEFT JOIN Users uU ON uU.UserId = po.UpdatedBy
LEFT JOIN Users uD ON uD.UserId = po.DeletedBy WHERE 1=1
            ");

        if (filter.Deleted.HasValue)
        {
            baseSql.Append(" AND po.IsDeleted = @IsDeleted");
            parameters.Add("IsDeleted", filter.Deleted.Value);
        }

        if (filter.Active.HasValue)
        {
            baseSql.Append(" AND po.IsActive = @IsActive");
            parameters.Add("IsActive", filter.Active.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.CreatedFromDate) &&
            DateTime.TryParse(filter.CreatedFromDate, out var createdFrom))
        {
            baseSql.Append(" AND po.CreatedAt >= @CreatedFrom");
            parameters.Add("CreatedFrom", createdFrom);
        }
        if (!string.IsNullOrWhiteSpace(filter.CreatedToDate) &&
            DateTime.TryParse(filter.CreatedToDate, out var createdTo))
        {
            baseSql.Append(" AND po.CreatedAt < DATEADD(day, 1, @CreatedTo)");
            parameters.Add("CreatedTo", createdTo.Date);
        }

        if (!string.IsNullOrWhiteSpace(filter.UpdatedFromDate) &&
            DateTime.TryParse(filter.UpdatedFromDate, out var updatedFrom))
        {
            baseSql.Append(" AND po.UpdatedAt >= @UpdatedFrom");
            parameters.Add("UpdatedFrom", updatedFrom);
        }
        if (!string.IsNullOrWhiteSpace(filter.UpdatedToDate) &&
            DateTime.TryParse(filter.UpdatedToDate, out var updatedTo))
        {
            baseSql.Append(" AND po.UpdatedAt < DATEADD(day, 1, @UpdatedTo)");
            parameters.Add("UpdatedTo", updatedTo.Date);
        }

        if (!string.IsNullOrWhiteSpace(filter.DeletedFromDate) &&
            DateTime.TryParse(filter.DeletedFromDate, out var deletedFrom))
        {
            baseSql.Append(" AND po.DeletedAt >= @DeletedFrom");
            parameters.Add("DeletedFrom", deletedFrom);
        }
        if (!string.IsNullOrWhiteSpace(filter.DeletedToDate) &&
            DateTime.TryParse(filter.DeletedToDate, out var deletedTo))
        {
            baseSql.Append(" AND po.DeletedAt < DATEADD(day, 1, @DeletedTo)");
            parameters.Add("DeletedTo", deletedTo.Date);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            baseSql.Append(@"
             AND (
                    po.PostOfficeName LIKE @Search
                 OR uC.FirstName LIKE @Search OR uC.LastName LIKE @Search
                 OR uU.FirstName LIKE @Search OR uU.LastName LIKE @Search
                 OR uD.FirstName LIKE @Search OR uD.LastName LIKE @Search
             )
            ");
            parameters.Add("Search", $"%{filter.SearchText}%");
        }

        var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["PostOfficeId"] = "po.PostOfficeId",
            ["PostOfficeName"] = "po.PostOfficeName",
            ["Active"] = "po.IsActive",
            ["Deleted"] = "po.IsDeleted",
            ["CreatedDate"] = "po.CreatedAt",
            ["ModifiedDate"] = "po.UpdatedAt",
            ["DeletedDate"] = "po.DeletedAt",
        };

        var sortOrder = filter.SortOrder == SortOrder.Desc ? "DESC" : "ASC";
        var sortKey = string.IsNullOrWhiteSpace(filter.SortBy) ? "PostOfficeId" : filter.SortBy;

        if (!sortMap.TryGetValue(sortKey, out var sortColumn))
            sortColumn = sortMap["PostOfficeId"];

        var countSql = "SELECT COUNT(1) " + baseSql;

        var itemsSql = $@"
            SELECT
                po.PostOfficeId,
                po.PostOfficeName,
                po.CountryId,
                po.CreatedAt,
                po.IsActive,
                po.UpdatedAt,
                po.IsDeleted,
                po.DeletedAt,
                uC.UserId AS CreatedById,
                CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
                uU.UserId AS UpdatedById,
                CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName,
                uD.UserId AS DeletedById,
                CONCAT(uD.FirstName, ' ', uD.LastName) AS DeletedUserName
            {baseSql}
            ORDER BY {sortColumn} {sortOrder}
            OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;
            ";

        using var connection = _context.CreateConnection();

        var totalCount = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

        var items = await connection.QueryAsync<PostOffice>(
            new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken));

        return new PagedResultDto<PostOffice>
        {
            Items = items.ToList(),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = limit
        };
    }

    public async Task<IEnumerable<PostOffice>> GetLookupAsync(CancellationToken cancellationToken = default)
    {
        var sql = new StringBuilder(@"
        SELECT
            PostOfficeId,
            PostOfficeName 
        FROM PostOffice
        WHERE IsDeleted = 0 AND Active = 1 ORDER BY PostOfficeName ASC
        ");

        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<PostOffice>(
            new CommandDefinition(sql.ToString(), cancellationToken: cancellationToken));
    }

    public async Task<PostOffice?> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM PostOffice WHERE PostOfficeName = @Name";
        if (excludeId.HasValue)
            sql += " AND PostOfficeId <> @ExcludeId";

        var cmd = new CommandDefinition(sql, new { Name = name, ExcludeId = excludeId }, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<PostOffice>(cmd);
    }

    public async Task<bool> UpdateAsync(PostOffice entity, CancellationToken cancellationToken = default)
    {
        var sets = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(entity.PostOfficeName))
        {
            sets.Add("PostOfficeName = @PostOfficeName");
            parameters.Add("PostOfficeName", entity.PostOfficeName);
        }

        if (entity.CountryId.HasValue)
        {
            sets.Add("CountryId = @CountryId");
            parameters.Add("CountryId", entity.CountryId);
        }

        if (entity.IsActive.HasValue)
        {
            sets.Add("Active = @Active");
            parameters.Add("Active", entity.IsActive.Value ? 1 : 0);
        }

        if (sets.Count == 0)
            return false;

        sets.Add("UpdatedBy = @UpdatedBy");
        sets.Add("UpdatedAt = SYSDATETIME()");
        parameters.Add("UpdatedBy", entity.UpdatedBy);
        parameters.Add("Id", entity.PostOfficeId);

        var sql = new StringBuilder();
        sql.Append("UPDATE PostOffice SET ");
        sql.Append(string.Join(", ", sets));
        sql.Append(" WHERE PostOfficeId = @Id AND IsDeleted = 0;");

        var cmd = new CommandDefinition(sql.ToString(), parameters, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        var affected = await connection.ExecuteAsync(cmd);
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(PostOffice entity, CancellationToken cancellationToken = default)
    {
        const string sql = @"UPDATE PostOffice SET IsDeleted = 1, DeletedAt = SYSDATETIME(), DeletedBy = @DeletedBy  WHERE PostOfficeId = @Id AND IsDeleted = 0;";
        var parameters = new DynamicParameters();
        parameters.Add("Id", entity.PostOfficeId);
        parameters.Add("DeletedBy", entity.DeletedBy);
        var cmd = new CommandDefinition(sql.ToString(), parameters, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        var affected = await connection.ExecuteAsync(cmd);
        return affected > 0;
    }

    public async Task<bool> RestoreAsync(PostOffice entity, CancellationToken cancellationToken = default)
    {
        const string sql = @"UPDATE PostOffice SET IsDeleted = 0, DeletedAt = NULL, DeletedBy = NULL, UpdatedBy = @UpdatedBy, UpdatedAt = SYSDATETIME()  WHERE PostOfficeId = @Id AND IsDeleted = 1;";
        var parameters = new DynamicParameters();
        parameters.Add("Id", entity.PostOfficeId);
        parameters.Add("UpdatedBy", entity.UpdatedBy);
        var cmd = new CommandDefinition(sql.ToString(), parameters, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        var affected = await connection.ExecuteAsync(cmd);
        return affected > 0;
    }

    public async Task<IEnumerable<PostOfficeZipCode>> GetZipCodesByPostOfficeIdAsync(int postOfficeId, CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT
                po.PostOfficeZipCodeId,
                po.PostOfficeId,
                po.ZipCode,
                po.CreatedAt,
                po.IsActive,
                po.UpdatedAt,
                po.IsDeleted,
                po.DeletedAt,
                uC.UserId AS CreatedById,
                CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
                uU.UserId AS UpdatedById,
                CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName FROM PostOfficeZipCode po
INNER JOIN Users uC ON uC.UserId = po.CreatedBy
LEFT JOIN Users uU ON uU.UserId = po.UpdatedBy
            WHERE PostOfficeId = @PostOfficeId AND po.IsDeleted = 0;";

        var cmd = new CommandDefinition(sql, new { PostOfficeId = postOfficeId }, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<PostOfficeZipCode>(cmd);
    }
    public async Task<IEnumerable<PostOfficeZipCode>> GetAllZipCodesAsync(string SearchText, CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT
                po.PostOfficeZipCodeId,
                po.PostOfficeId,
                po.ZipCode,
                po.CreatedAt,
                po.IsActive,
                po.UpdatedAt,
                po.IsDeleted,
                po.DeletedAt,
                uC.UserId AS CreatedById,
                CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
                uU.UserId AS UpdatedById,
                CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName FROM PostOfficeZipCode po
                INNER JOIN Users uC ON uC.UserId = po.CreatedBy
                LEFT JOIN Users uU ON uU.UserId = po.UpdatedBy
                            WHERE ZipCode lINK @ZipCode AND po.IsDeleted = 0;";
        var param = new { ZipCode = $"%{SearchText}%" };

        var cmd = new CommandDefinition(sql, param, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<PostOfficeZipCode>(cmd);
    }
    public async Task BulkUpsertZipCodesAsync(int postOfficeId, IEnumerable<PostOfficeZipCode> zipCodes, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // load existing
            const string selectSql = @"SELECT PostOfficeZipCodeId, PostOfficeId, ZipCode FROM PostOfficeZipCode WHERE PostOfficeId = @PostOfficeId AND IsDeleted = 0;";
            var existing = (await connection.QueryAsync<PostOfficeZipCode>(
                new CommandDefinition(selectSql, new { PostOfficeId = postOfficeId }, transaction, cancellationToken: cancellationToken))).ToList();

            var incomingList = zipCodes.ToList();

            // update or insert
            foreach (var z in incomingList)
            {
                if (z.PostOfficeZipCodeId > 0)
                {
                    const string updateSql = @"UPDATE PostOfficeZipCode SET ZipCode = @ZipCode, UpdatedBy = @UpdatedBy, UpdatedAt = SYSDATETIME() WHERE PostOfficeZipCodeId = @Id;";
                    await connection.ExecuteAsync(
                        new CommandDefinition(updateSql, new { Id = z.PostOfficeZipCodeId, z.ZipCode, z.UpdatedBy }, transaction, cancellationToken: cancellationToken));
                }
                else
                {
                    const string insertSql = @"INSERT INTO PostOfficeZipCode (PostOfficeId, ZipCode, CreatedBy) VALUES (@PostOfficeId, @ZipCode, @CreatedBy);";
                    await connection.ExecuteAsync(
                        new CommandDefinition(insertSql, new { PostOfficeId = postOfficeId, z.ZipCode, z.CreatedBy }, transaction, cancellationToken: cancellationToken));
                }
            }

            // delete missing
            var incomingIds = incomingList.Where(x => x.PostOfficeZipCodeId > 0).Select(x => x.PostOfficeZipCodeId).ToHashSet();
            var toDelete = existing.Where(x => !incomingIds.Contains(x.PostOfficeZipCodeId)).ToList();
            foreach (var del in toDelete)
            {
                const string deleteSql = @"UPDATE PostOfficeZipCode SET IsDeleted = 1, DeletedAt = SYSDATETIME() WHERE PostOfficeZipCodeId = @Id;";
                await connection.ExecuteAsync(
                    new CommandDefinition(deleteSql, new { Id = del.PostOfficeZipCodeId }, transaction, cancellationToken: cancellationToken));
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
