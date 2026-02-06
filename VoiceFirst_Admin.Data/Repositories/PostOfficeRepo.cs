using Azure.Core;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;
using static Dapper.SqlMapper;

namespace VoiceFirst_Admin.Data.Repositories;

public class PostOfficeRepo : IPostOfficeRepo
{
    private readonly IDapperContext _context;

    public PostOfficeRepo(IDapperContext context)
    {
        _context = context;
    }





  




    public async Task<Dictionary<string, bool>> IsBulkIdsExistAsync(
    List<int> postOfficeIds,
    CancellationToken cancellationToken = default)
        {
            var result = new Dictionary<string, bool>
        {
            { "idNotFound", false },
            { "deletedOrInactive", false }
        };



        if (postOfficeIds == null || postOfficeIds.Count == 0)
            return result;

        const string sql = @"
        SELECT 
            PostOfficeId,
            IsActive,
            IsDeleted
        FROM PostOffice
        WHERE PostOfficeId IN @Ids;
        ";

        using var connection = _context.CreateConnection();

        var entities = (await connection.QueryAsync<SysProgramActions>(
            new CommandDefinition(
                sql,
                new { Ids = postOfficeIds },
                cancellationToken: cancellationToken)))
            .ToList();

        // 1️⃣ Check NOT FOUND
        if (entities.Count != postOfficeIds.Distinct().Count())
        {
            result["idNotFound"] = true;
        }

        // 2️⃣ Check Deleted or Inactive
        if (entities.Any(x => x.IsDeleted == true || x.IsActive == false))
        {
            result["deletedOrInactive"] = true;
        }

        return result;
    }





    public async Task<PostOffice> CreateAsync(PostOffice entity, List<string> zipCodes, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();
        connection.Open();
        using var tx = connection.BeginTransaction();
        try
        {
            const string sql = @"
                INSERT INTO PostOffice (PostOfficeName, CountryId, DivisionOneId, DivisionTwoId, DivisionThreeId, CreatedBy)
                VALUES (@PostOfficeName, @CountryId, @DivisionOneId, @DivisionTwoId, @DivisionThreeId, @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            var cmd = new CommandDefinition(sql, new
            {
                entity.PostOfficeName,
                entity.CountryId,
                entity.DivisionOneId,
                entity.DivisionTwoId,
                entity.DivisionThreeId,
                entity.CreatedBy,
            }, transaction:tx, cancellationToken: cancellationToken);
            var id = await connection.ExecuteScalarAsync<int>(cmd);
            if (id > 0)
            {
                entity.PostOfficeId = id;

                if (zipCodes.Count() > 0)
                {
                    await BulkCreateZipCodesAsync(connection, tx,id, zipCodes, entity.CreatedBy, cancellationToken);
                }



                tx.Commit();
                connection.Close();
                return entity;
            }
            return null;
        }
        catch
        {
            tx.Rollback();
            connection.Close();
            throw;
        }
        
    }

    public async Task<PostOffice?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT
                po.PostOfficeId,
                po.PostOfficeName,
                po.CountryId,
                po.DivisionOneId,
                po.DivisionTwoId,
                po.DivisionThreeId,
                po.CreatedAt,
                po.IsActive,
                po.UpdatedAt,
                po.IsDeleted,
                po.DeletedAt,
                Country.CountryName,
                DivisionOne.DivisionOneId,
                DivisionTwo.DivisionTwoId,
                DivisionThree.DivisionThreeId,
                DivisionOne.DivisionOneName,
                DivisionTwo.DivisionTwoName,
                DivisionThree.DivisionThreeName,
                uC.UserId   AS CreatedById,
                CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
                uU.UserId   AS UpdatedById,
                CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName,
                uD.UserId   AS DeletedById,
                CONCAT(uD.FirstName, ' ', uD.LastName) AS DeletedUserName
            FROM PostOffice po
            INNER JOIN Users uC ON uC.UserId = po.CreatedBy
            INNER JOIN Country ON Country.CountryId = po.CountryId
            LEFT JOIN DivisionTwo ON DivisionTwo.DivisionTwoId = po.DivisionTwoId
            LEFT JOIN DivisionOne ON DivisionOne.DivisionOneId = po.DivisionOneId
            LEFT JOIN DivisionThree ON DivisionThree.DivisionThreeId = po.DivisionThreeId
            LEFT JOIN Users uU ON uU.UserId = po.UpdatedBy
            LEFT JOIN Users uD ON uD.UserId = po.DeletedBy
            WHERE po.PostOfficeId = @Id;";

        var cmd = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<PostOffice>(cmd);
    }
    public async Task<PostOfficeZipCode?> GetZipCodeByIdAsync(int id, CancellationToken cancellationToken = default)
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
                CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName,
                uD.UserId   AS DeletedById,
                CONCAT(uD.FirstName, ' ', uD.LastName) AS DeletedUserName FROM PostOfficeZipCode po
                INNER JOIN Users uC ON uC.UserId = po.CreatedBy
                LEFT JOIN Users uU ON uU.UserId = po.UpdatedBy
                LEFT JOIN Users uD ON uD.UserId = po.DeletedBy
                 WHERE po.PostOfficeZipCodeId = @Id;";

        var cmd = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<PostOfficeZipCode>(cmd);
    }

    public async Task<PagedResultDto<PostOffice>> GetAllAsync(PostOfficeFilterDto filter, CancellationToken cancellationToken = default)
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
            INNER JOIN Country ON Country.CountryId = po.CountryId
            LEFT JOIN DivisionTwo ON DivisionTwo.DivisionTwoId = po.DivisionTwoId
            LEFT JOIN DivisionOne ON DivisionOne.DivisionOneId = po.DivisionOneId
            LEFT JOIN DivisionThree ON DivisionThree.DivisionThreeId = po.DivisionThreeId
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
        var searchByMap = new Dictionary<PostOfficeSearchBy, string>
        {
            [PostOfficeSearchBy.PostOfficeName] = "po.PostOfficeName",
            [PostOfficeSearchBy.CountryName] = "Country.CountryName",
            [PostOfficeSearchBy.DivOneName] = "DivisionOne.DivisionOneName",
            [PostOfficeSearchBy.DivTwoName] = "DivisionTwo.DivisionTwoName",
            [PostOfficeSearchBy.DivThreeName] = "DivisionThree.DivisionThreeName",
            [PostOfficeSearchBy.CreatedUser] = "CONCAT(uC.FirstName,' ',uC.LastName)",
            [PostOfficeSearchBy.UpdatedUser] = "CONCAT(uU.FirstName,' ',uU.LastName)",
            [PostOfficeSearchBy.DeletedUser] = "CONCAT(uD.FirstName,' ',uD.LastName)",

            // ZipCode cannot be a single column on po; handle separately (see below)
        };

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            // If SearchBy = ZipCode, use EXISTS on PostOfficeZipCode
            if (filter.SearchBy == PostOfficeSearchBy.ZipCode)
            {
                // Zip code moved to master 'ZipCode' table and linked via 'PostOfficeZipCodeLink'
                baseSql.Append(@"
            AND EXISTS (
                SELECT 1
                FROM PostOfficeZipCodeLink l
                INNER JOIN ZipCode z ON z.ZipCodeId = l.ZipCodeId
                WHERE l.PostOfficeId = po.PostOfficeId
                  AND l.IsActive = 1
                  AND z.ZipCode LIKE @Search
            )");
            }
            else if (filter.SearchBy.HasValue && searchByMap.TryGetValue(filter.SearchBy.Value, out var col))
            {
                baseSql.Append($" AND {col} LIKE @Search");
            }
            else
            {
                // Default: search across everything (name + users + zipcode)
                baseSql.Append(@"
            AND (
                   po.PostOfficeName LIKE @Search
                OR uC.FirstName LIKE @Search OR uC.LastName LIKE @Search
                OR uU.FirstName LIKE @Search OR uU.LastName LIKE @Search
                OR uD.FirstName LIKE @Search OR uD.LastName LIKE @Search
                OR Country.CountryName LIKE @Search 
                OR DivisionOne.DivisionOneName LIKE @Search 
                OR DivisionTwo.DivisionTwoName LIKE @Search 
                OR DivisionThree.DivisionThreeName LIKE @Search 
                OR EXISTS (
                    SELECT 1
                FROM PostOfficeZipCodeLink l
                INNER JOIN ZipCode z ON z.ZipCodeId = l.ZipCodeId
                WHERE l.PostOfficeId = po.PostOfficeId
                  AND l.IsActive = 1
                  AND z.ZipCode LIKE @Search
                )
            )");
            }

            parameters.Add("Search", $"%{filter.SearchText}%");
        }



        var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["PostOfficeId"] = "po.PostOfficeId",
            ["CountryName"] = "Country.CountryName",
            ["DivOneName"] = "DivisionOne.DivisionOneName",
            ["DivThreeName"] = "DivisionThree.DivisionThreeName",
            ["DivTwoName"] = "DivisionTwo.DivisionTwoName",
            ["PostOfficeName"] = "po.PostOfficeName",
            ["Active"] = "po.IsActive",
            ["Deleted"] = "po.IsDeleted",
            ["CreatedDate"] = "po.CreatedAt",
            ["ModifiedDate"] = "po.UpdatedAt",
            ["DeletedDate"] = "po.DeletedAt",
        };

        var sortOrder = filter.SortOrder == Utilities.DTOs.Shared.SortOrder.Desc ? "DESC" : "ASC";
        var sortKey = string.IsNullOrWhiteSpace(filter.SortBy) ? "PostOfficeId" : filter.SortBy;

        if (!sortMap.TryGetValue(sortKey, out var sortColumn))
            sortColumn = sortMap["PostOfficeId"];

        var countSql = "SELECT COUNT(1) " + baseSql;

        var itemsSql = $@"
            SELECT
                po.PostOfficeId,
                po.PostOfficeName,
                po.CountryId,
                DivisionOne.DivisionOneId,
                DivisionTwo.DivisionTwoId,
                DivisionThree.DivisionThreeId,
                DivisionOne.DivisionOneName,
                DivisionTwo.DivisionTwoName,
                DivisionThree.DivisionThreeName,
                po.CreatedAt,
                po.IsActive,
                po.UpdatedAt,
                po.IsDeleted,
                po.DeletedAt,
                uC.UserId AS CreatedById,
                Country.CountryName,
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



    public async Task<IEnumerable<PostOffice>> GetLookupAsync(PostOfficeLookUpWithZipCodeFilterDto filter, CancellationToken cancellationToken = default)
    {
        var sql = new StringBuilder(@"
SELECT
    po.PostOfficeId,
    po.PostOfficeName,
    po.CountryId,
    po.DivisionOneId,
    po.DivisionThreeId,
DivisionOne.DivisionOneName,
                DivisionTwo.DivisionTwoName,
                DivisionThree.DivisionThreeName,
po.DivisionTwoId
FROM PostOffice po
INNER JOIN Country ON Country.CountryId = po.CountryId
LEFT JOIN DivisionTwo ON DivisionTwo.DivisionTwoId = po.DivisionTwoId
LEFT JOIN DivisionOne ON DivisionOne.DivisionOneId = po.DivisionOneId
LEFT JOIN DivisionThree ON DivisionThree.DivisionThreeId = po.DivisionThreeId
WHERE po.IsDeleted = 0
  AND po.IsActive = 1
");

        // Optional filters (only appended if provided)
        if (filter?.CountryId is not null)
            sql.AppendLine("  AND po.CountryId = @CountryId");

        if (filter?.DivOneId is not null)
            sql.AppendLine("  AND po.DivisionOneId = @DivOneId");

        if (filter?.DivTwoId is not null)
            sql.AppendLine("  AND po.DivisionTwoId = @DivTwoId");

        if (filter?.DivThreeId is not null)
            sql.AppendLine("  AND po.DivisionThreeId = @DivThreeId");

        // ZipCode filter via EXISTS (your requirement)
        if (!string.IsNullOrWhiteSpace(filter?.ZipCode))
        {
            sql.AppendLine(@"
  AND EXISTS (
      SELECT 1
      FROM PostOfficeZipCodeLink l
      INNER JOIN ZipCode z ON z.ZipCodeId = l.ZipCodeId
      WHERE l.PostOfficeId = po.PostOfficeId
        AND l.IsActive = 1
        AND z.ZipCode LIKE @ZipCodeSearch
  )
");
        }

        sql.AppendLine("ORDER BY po.PostOfficeName ASC;");

        var param = new
        {
            CountryId = filter?.CountryId,
            DivOneId = filter?.DivOneId,
            DivTwoId = filter?.DivTwoId,
            DivThreeId = filter?.DivThreeId,
            ZipCodeSearch = !string.IsNullOrWhiteSpace(filter?.ZipCode)
                ? $"%{filter!.ZipCode.Trim()}%"
                : null
        };

        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<PostOffice>(
            new CommandDefinition(sql.ToString(), param, cancellationToken: cancellationToken));
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
        if (entity.DivisionOneId.HasValue)
        {
            sets.Add("DivisionOneId = @DivisionOneId");
            parameters.Add("DivisionOneId", entity.DivisionOneId);
        }
        if (entity.DivisionTwoId.HasValue)
        {
            sets.Add("DivisionTwoId = @DivisionTwoId");
            parameters.Add("DivisionTwoId", entity.CountryId);
        }
        if (entity.DivisionThreeId.HasValue)
        {
            sets.Add("DivisionThreeId = @DivisionThreeId");
            parameters.Add("DivisionThreeId", entity.CountryId);
        }

        if (entity.IsActive.HasValue)
        {
            sets.Add("IsActive = @IsActive");
            parameters.Add("IsActive", entity.IsActive.Value ? 1 : 0);
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
                l.PostOfficeZipCodeLinkId,
                l.PostOfficeId,
                z.ZipCode,
                l.CreatedAt,
                l.IsActive,
                l.UpdatedAt,
                uC.UserId AS CreatedById,
                CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
                uU.UserId AS UpdatedById,
                CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName
            FROM PostOfficeZipCodeLink l
            INNER JOIN ZipCode z ON z.ZipCodeId = l.ZipCodeId
            INNER JOIN Users uC ON uC.UserId = l.CreatedBy
            LEFT JOIN Users uU ON uU.UserId = l.UpdatedBy
            WHERE l.PostOfficeId = @PostOfficeId ";

        var cmd = new CommandDefinition(sql, new { PostOfficeId = postOfficeId }, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<PostOfficeZipCode>(cmd);
    }
    public async Task<IEnumerable<PostOfficeZipCode>> GetAllZipCodesAsync(string SearchText, CancellationToken cancellationToken = default)
    {
        // After schema change ZipCode is moved to master table 'ZipCode' and links are in 'PostOfficeZipCodeLink'
        const string sql = @"SELECT
                l.PostOfficeZipCodeLinkId,
                l.PostOfficeId,
                z.ZipCode,
                l.CreatedAt,
                l.IsActive,
                l.UpdatedAt,
                uC.UserId AS CreatedById,
                CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
                uU.UserId AS UpdatedById,
                CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName
            FROM PostOfficeZipCodeLink l
            INNER JOIN ZipCode z ON z.ZipCodeId = l.ZipCodeId
            INNER JOIN Users uC ON uC.UserId = l.CreatedBy
            LEFT JOIN Users uU ON uU.UserId = l.UpdatedBy
            WHERE z.ZipCode LIKE @ZipCode";

        var param = new { ZipCode = $"%{SearchText}%" };
        var cmd = new CommandDefinition(sql, param, cancellationToken: cancellationToken);
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<PostOfficeZipCode>(cmd);
    }
    public async Task<BulkUpsertError?> BulkUpdateZipCodesAsync(int postOfficeId, IEnumerable<PostOfficeZipCode> zipCodes, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        string? currentZip = null;
        try
        {
            // load existing post-office -> zip links
           

            var incomingList = zipCodes.ToList();

            // update or insert
            foreach (var z in incomingList)
            {
                currentZip = z.ZipCode?.Trim();

                if (z.PostOfficeZipCodeLinkId > 0)
                {
                    // update existing link (PostOfficeZipCodeLink)
                    var sets = new List<string>();
                    var parameters = new DynamicParameters();

                    if (z.IsActive.HasValue)
                    {
                        sets.Add("IsActive = @IsActive");
                        parameters.Add("IsActive", z.IsActive.Value ? 1 : 0);
                    }

                    // if zip value changed, ensure master exists and update link's ZipCodeId
                    if (!string.IsNullOrWhiteSpace(z.ZipCode))
                    {
                        // find or insert master ZipCode
                        var zip = await connection.QueryFirstOrDefaultAsync<PostOfficeZipCode>(new CommandDefinition("SELECT ZipCodeId, ZipCode  FROM ZipCode WHERE ZipCode = @Zip", new { Zip = currentZip }, transaction, cancellationToken: cancellationToken));
                        int zipId;
                        if (zip == null)
                        {
                            var insertZipSql = "INSERT INTO ZipCode (ZipCode, CreatedBy) VALUES (@ZipCode, @CreatedBy); SELECT CAST(SCOPE_IDENTITY() AS int);";
                            zipId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(insertZipSql, new { ZipCode = currentZip, CreatedBy = z.CreatedBy }, transaction, cancellationToken: cancellationToken));
                        }
                        else
                        {
                            
                            // map to existing id
                            zipId = zip.PostOfficeZipCodeLinkId;
                        }

                        sets.Add("ZipCodeId = @ZipCodeId");
                        parameters.Add("ZipCodeId", zipId);
                    }

                    if (sets.Count > 0)
                    {
                        parameters.Add("UpdatedBy", z.UpdatedBy);
                        parameters.Add("Id", z.PostOfficeZipCodeLinkId);
                        var sql = new StringBuilder();
                        sql.Append("UPDATE PostOfficeZipCodeLink SET UpdatedBy = @UpdatedBy, UpdatedAt = SYSDATETIME(), ");
                        sql.Append(string.Join(", ", sets));
                        sql.Append(" WHERE PostOfficeZipCodeLinkId = @Id");

                        var cmd = new CommandDefinition(sql.ToString(), parameters, transaction, cancellationToken: cancellationToken);
                        var affected = await connection.ExecuteAsync(cmd);
                    }
                }
                else
                {
                    return new BulkUpsertError
                    {
                        Message = Messages.BadRequest,
                        StatuaCode = StatusCodes.Status400BadRequest
                    };
                }
            }

            

            transaction.Commit();
            return null;
        }
        catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
        {
            // ? SQL unique constraint error
            transaction.Rollback();
            var sql = "SELECT * FROM PostOfficeZipCode WHERE ZipCode = @ZipCode";
         

            var cmd = new CommandDefinition(sql, new { ZipCode = currentZip }, cancellationToken: cancellationToken);
            var item= await connection.QueryFirstOrDefaultAsync<SysProgramActions>(cmd);
            if(item==null)
                return new BulkUpsertError
                {
                    Message = Messages.NotFound,
                    StatuaCode = StatusCodes.Status404NotFound
                };
            else if (item.IsDeleted == true)
            {
                return new BulkUpsertError
                {
                    Message = $"ZipCode '{currentZip ?? "(unknown)"}' exists in trash. Restore it to use again. ",
                    StatuaCode = StatusCodes.Status422UnprocessableEntity
                };
            }
            else
            {
                return new BulkUpsertError
                {
                    Message = $"ZipCode '{currentZip ?? "(unknown)"}' already exists.",
                    StatuaCode = StatusCodes.Status409Conflict
                };
            }
                
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new InvalidOperationException("Bulk upsert failed: " + ex.Message, ex);
        }
    }
    public async Task<BulkUpsertError?> BulkCreateZipCodesAsync(IDbConnection connection, IDbTransaction transaction, int postOfficeId, List<string> zipCodes, int loginId, CancellationToken cancellationToken = default)
    {
        
        string? currentZip = null;
        try
        {
            // load existing post-office -> zip links


            var incomingList = zipCodes.ToList();

            // update or insert
            foreach (var z in incomingList)
            {

                currentZip = (z ?? "").Trim();

                if (currentZip.Length > 0)
                {
                    // ensure master ZipCode exists
                    var zip = await connection.QueryFirstOrDefaultAsync<ZipCodes>(new CommandDefinition("SELECT ZipCodeId, ZipCode FROM ZipCode WHERE ZipCode = @Zip", new { Zip = currentZip }, transaction, cancellationToken: cancellationToken));
                    int zipId;
                    if (zip == null)
                    {
                        var insertZipSql = "INSERT INTO ZipCode (ZipCode, CreatedBy) VALUES (@ZipCode, @CreatedBy); SELECT CAST(SCOPE_IDENTITY() AS int);";
                        zipId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(insertZipSql, new { ZipCode = currentZip, CreatedBy = loginId }, transaction, cancellationToken: cancellationToken));
                    }
                    else
                    {

                        zipId = zip.ZipCodeId;
                    }

                    const string insertSql = @"INSERT INTO PostOfficeZipCodeLink (PostOfficeId, ZipCodeId, CreatedBy) VALUES (@PostOfficeId, @ZipCodeId, @CreatedBy);";
                    await connection.ExecuteAsync(new CommandDefinition(insertSql, new { PostOfficeId = postOfficeId, ZipCodeId = zipId, CreatedBy = loginId }, transaction, cancellationToken: cancellationToken));
                }
            }



            return null;
        }
        catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
        {
            // ? SQL unique constraint error
            transaction.Rollback();
            var sql = "SELECT * FROM PostOfficeZipCode WHERE ZipCode = @ZipCode";


            var cmd = new CommandDefinition(sql, new { ZipCode = currentZip }, cancellationToken: cancellationToken);
            var item = await connection.QueryFirstOrDefaultAsync<SysProgramActions>(cmd);
            if (item == null)
                return new BulkUpsertError
                {
                    Message = Messages.NotFound,
                    StatuaCode = StatusCodes.Status404NotFound
                };
            else if (item.IsDeleted == true)
            {
                return new BulkUpsertError
                {
                    Message = $"ZipCode '{currentZip ?? "(unknown)"}' exists in trash. Restore it to use again. ",
                    StatuaCode = StatusCodes.Status422UnprocessableEntity
                };
            }
            else
            {
                return new BulkUpsertError
                {
                    Message = $"ZipCode '{currentZip ?? "(unknown)"}' already exists.",
                    StatuaCode = StatusCodes.Status409Conflict
                };
            }

        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new InvalidOperationException("Bulk upsert failed: " + ex.Message, ex);
        }
    }
    public async Task<BulkUpsertError?> BulkInsertZipCodesAsync(int postOfficeId, List<string> zipCodes, int loginId, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        string? currentZip = null;
        try
        {
            // load existing post-office -> zip links


            var incomingList = zipCodes.ToList();

            // update or insert
            foreach (var z in incomingList)
            {
               
                currentZip = (z ?? "").Trim();

                if (currentZip.Length > 0)
                { 
                    // ensure master ZipCode exists
                    var zip = await connection.QueryFirstOrDefaultAsync<ZipCodes>(new CommandDefinition("SELECT ZipCodeId, ZipCode FROM ZipCode WHERE ZipCode = @Zip", new { Zip = currentZip }, transaction, cancellationToken: cancellationToken));
                    int zipId;
                    if (zip == null)
                    {
                        var insertZipSql = "INSERT INTO ZipCode (ZipCode, CreatedBy) VALUES (@ZipCode, @CreatedBy); SELECT CAST(SCOPE_IDENTITY() AS int);";
                        zipId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(insertZipSql, new { ZipCode = currentZip, CreatedBy = loginId }, transaction, cancellationToken: cancellationToken));
                    }
                    else
                    {

                        zipId = zip.ZipCodeId;
                    }

                    const string insertSql = @"INSERT INTO PostOfficeZipCodeLink (PostOfficeId, ZipCodeId, CreatedBy) VALUES (@PostOfficeId, @ZipCodeId, @CreatedBy);";
                    await connection.ExecuteAsync(new CommandDefinition(insertSql, new { PostOfficeId = postOfficeId, ZipCodeId = zipId, CreatedBy = loginId }, transaction, cancellationToken: cancellationToken));
                }
            }



            transaction.Commit();
            return null;
        }
        catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
        {
            // ? SQL unique constraint error
            transaction.Rollback();
            var sql = "SELECT * FROM PostOfficeZipCode WHERE ZipCode = @ZipCode";


            var cmd = new CommandDefinition(sql, new { ZipCode = currentZip }, cancellationToken: cancellationToken);
            var item = await connection.QueryFirstOrDefaultAsync<SysProgramActions>(cmd);
            if (item == null)
                return new BulkUpsertError
                {
                    Message = Messages.NotFound,
                    StatuaCode = StatusCodes.Status404NotFound
                };
            else if (item.IsDeleted == true)
            {
                return new BulkUpsertError
                {
                    Message = $"ZipCode '{currentZip ?? "(unknown)"}' exists in trash. Restore it to use again. ",
                    StatuaCode = StatusCodes.Status422UnprocessableEntity
                };
            }
            else
            {
                return new BulkUpsertError
                {
                    Message = $"ZipCode '{currentZip ?? "(unknown)"}' already exists.",
                    StatuaCode = StatusCodes.Status409Conflict
                };
            }

        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new InvalidOperationException("Bulk upsert failed: " + ex.Message, ex);
        }
    }
}

