using Dapper;
using System.Data;
using System.Text;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class SysIssueTypeRepo : ISysIssueTypeRepo
    {
        private readonly IDapperContext _context;

        public SysIssueTypeRepo(IDapperContext context)
        {
            _context = context;
        }


        public async Task<SysIssueTypeDTO> IssueTypeExistsAsync
            (string name, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var sql = "SELECT IsDeleted As Deleted, SysIssueTypeId As IssueTypeId FROM SysIssueType WHERE IssueType = @IssueTypeName";
            if (excludeId.HasValue)
                sql += " AND SysIssueTypeId <> @ExcludeId";

            var cmd = new CommandDefinition(sql, new { IssueTypeName = name, ExcludeId = excludeId }, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            var entity = await connection.QueryFirstOrDefaultAsync<SysIssueTypeDTO>(cmd);
            return entity;
        }


        public async Task<int> CreateAsync
            (SysIssueType entity,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
                INSERT INTO SysIssueType (IssueType, CreatedBy)
                VALUES (@IssueType, @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            var cmd = new CommandDefinition(sql, new
            {
                entity.IssueType,
                entity.CreatedBy,
            }, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            int id = await connection.ExecuteScalarAsync<int>(cmd);
            return id;
        }


        public async Task<SysIssueTypeDTO?> GetByIdAsync(
          int issueTypeId,
          CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT 
                    s.SysIssueTypeId       As IssueTypeId,
                    s.IssueType            As IssueType,
                    s.IsActive             As Active,
                    s.IsDeleted            As Deleted,
                    s.CreatedAt            As CreatedDate,
                    s.UpdatedAt            As ModifiedDate,
                    s.DeletedAt            As DeletedDate,

                    -- Created User
                    CONCAT(cu.FirstName, ' ', ISNULL(cu.LastName, '')) AS CreatedUser,

                    -- Updated User
                    CONCAT(uu.FirstName, ' ', ISNULL(uu.LastName, '')) AS ModifiedUser,

                    -- Deleted User
                    CONCAT(du.FirstName, ' ', ISNULL(du.LastName, '')) AS DeletedUser

                FROM dbo.SysIssueType s
                INNER JOIN dbo.Users cu ON cu.UserId = s.CreatedBy
                LEFT JOIN dbo.Users uu ON uu.UserId = s.UpdatedBy
                LEFT JOIN dbo.Users du ON du.UserId = s.DeletedBy
                WHERE s.SysIssueTypeId = @IssueTypeId;
                ";

            using var connection = _context.CreateConnection();
            var entity = await connection.QuerySingleOrDefaultAsync<SysIssueTypeDTO>(
                new CommandDefinition(sql, new { IssueTypeId = issueTypeId }, cancellationToken: cancellationToken)
            );

            if (entity == null)
                return null;

            // load media rules
            const string rulesSql = @"
                SELECT 
                    r.SysIssueMediaRuleId AS IssueMediaRuleId,
                    r.IssueMediaFormatId AS IssueMediaFormatId,
                    f.IssueMediaFormat AS IssueMediaFormat,
                    r.[Min] AS Min,
                    r.[Max] AS Max,
                    r.MaxSizeMB AS MaxSizeMB,
                    CONCAT(cu.FirstName, ' ', ISNULL(cu.LastName, '')) AS CreatedUser,
                    r.CreatedAt AS CreatedDate,
                    r.IsActive AS Active,
                    CONCAT(uu.FirstName, ' ', ISNULL(uu.LastName, '')) AS ModifiedUser,
                    r.UpdatedAt AS ModifiedDate
                FROM dbo.SysIssueMediaRule r
                INNER JOIN dbo.SysIssueMediaFormat f ON f.SysIssueMediaFormatId = r.IssueMediaFormatId
                LEFT JOIN dbo.Users cu ON cu.UserId = r.CreatedBy
                LEFT JOIN dbo.Users uu ON uu.UserId = r.UpdatedBy
                WHERE r.IssueTypeId = @IssueTypeId ;";

            var rules = (await connection.QueryAsync<IssueMediaRuleDTO>(new CommandDefinition(rulesSql, new { IssueTypeId = issueTypeId }, cancellationToken: cancellationToken))).ToList();

            if (rules.Any())
            {
                var ruleIds = rules.Select(r => r.IssueMediaRuleId).ToArray();

                const string typesSql = @"
                    SELECT 
                        t.SysIssueMediaRuleTypeId AS IssueMediaRuleTypeId,
                        t.IssueMediaRuleId AS IssueMediaRuleId,
                        t.IssueMediaTypeId AS IssueMediaTypeId,
                        mt.IssueMediaType AS IssueMediaType,
                        t.IsMandatory AS IsMandatory,
                        CONCAT(cu.FirstName, ' ', ISNULL(cu.LastName, '')) AS CreatedUser,
                        t.CreatedAt AS CreatedDate,
                        t.IsActive AS Active,
                        CONCAT(uu.FirstName, ' ', ISNULL(uu.LastName, '')) AS ModifiedUser,
                        t.UpdatedAt AS ModifiedDate
                    FROM dbo.SysIssueMediaRuleType t
                    INNER JOIN dbo.SysIssueMediaType mt ON mt.SysIssueMediaTypeId = t.IssueMediaTypeId
                    LEFT JOIN dbo.Users cu ON cu.UserId = t.CreatedBy
                    LEFT JOIN dbo.Users uu ON uu.UserId = t.UpdatedBy
                    WHERE t.IssueMediaRuleId IN @RuleIds ;";

                var types = (await connection.QueryAsync<IssueMediaRuleTypeDTO>(new CommandDefinition(typesSql, new { RuleIds = ruleIds }, cancellationToken: cancellationToken))).ToList();

                // group types into their rules
                foreach (var rule in rules)
                {
                    var mts = types.Where(t => t.IssueMediaRuleId == rule.IssueMediaRuleId).ToList();
                    rule.MediaTypes = mts;
                }
            }

            entity.MediaRules = rules;

            return entity;
        }




        public async Task<SysIssueTypeDTO> IsIdExistAsync(
          int issueTypeId,
          CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT  s.SysIssueTypeId   As IssueTypeId,
                        s.IsDeleted        As Deleted
                FROM dbo.SysIssueType s
                WHERE SysIssueTypeId = @IssueTypeId;
                ";

            using var connection = _context.CreateConnection();

            var dto = await connection.QuerySingleOrDefaultAsync<SysIssueTypeDTO>(
                new CommandDefinition(
                    sql,
                    new { IssueTypeId = issueTypeId },
                    cancellationToken: cancellationToken
                )
            );
            return dto;
        }



        public async Task<PagedResultDto<SysIssueTypeDTO>>
         GetAllAsync(IssueTypeFilterDTO filter, CancellationToken cancellationToken = default)
        {
            var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var limit = filter.Limit <= 0 ? 10 : filter.Limit;
            var offset = (page - 1) * limit;

            var parameters = new DynamicParameters();
            parameters.Add("Offset", offset);
            parameters.Add("Limit", limit);


            var baseSql = new StringBuilder(@"
            FROM SysIssueType sit
            INNER JOIN Users uC ON uC.UserId = sit.CreatedBy
            LEFT JOIN Users uU ON uU.UserId = sit.UpdatedBy
            LEFT JOIN Users uD ON uD.UserId = sit.DeletedBy WHERE 1=1
            ");

            if (filter.Deleted.HasValue)
            {
                baseSql.Append(" AND sit.IsDeleted = @IsDeleted");
                parameters.Add("IsDeleted", filter.Deleted.Value);
            }

            if (filter.Active.HasValue)
            {
                baseSql.Append(" AND sit.IsActive = @IsActive");
                parameters.Add("IsActive", filter.Active.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.CreatedFromDate) &&
                DateTime.TryParse(filter.CreatedFromDate, out var createdFrom))
            {
                baseSql.Append(" AND sit.CreatedAt >= @CreatedFrom");
                parameters.Add("CreatedFrom", createdFrom);
            }
            if (!string.IsNullOrWhiteSpace(filter.CreatedToDate) &&
                DateTime.TryParse(filter.CreatedToDate, out var createdTo))
            {
                baseSql.Append(" AND sit.CreatedAt < DATEADD(day, 1, @CreatedTo)");
                parameters.Add("CreatedTo", createdTo.Date);
            }


            if (!string.IsNullOrWhiteSpace(filter.UpdatedFromDate) &&
                DateTime.TryParse(filter.UpdatedFromDate, out var updatedFrom))
            {
                baseSql.Append(" AND sit.UpdatedAt >= @UpdatedFrom");
                parameters.Add("UpdatedFrom", updatedFrom);
            }
            if (!string.IsNullOrWhiteSpace(filter.UpdatedToDate) &&
                DateTime.TryParse(filter.UpdatedToDate, out var updatedTo))
            {
                baseSql.Append(" AND sit.UpdatedAt < DATEADD(day, 1, @UpdatedTo)");
                parameters.Add("UpdatedTo", updatedTo.Date);
            }


            if (!string.IsNullOrWhiteSpace(filter.DeletedFromDate) &&
                DateTime.TryParse(filter.DeletedFromDate, out var deletedFrom))
            {
                baseSql.Append(" AND sit.DeletedAt >= @DeletedFrom");
                parameters.Add("DeletedFrom", deletedFrom);
            }
            if (!string.IsNullOrWhiteSpace(filter.DeletedToDate) &&
                DateTime.TryParse(filter.DeletedToDate, out var deletedTo))
            {
                baseSql.Append(" AND sit.DeletedAt < DATEADD(day, 1, @DeletedTo)");
                parameters.Add("DeletedTo", deletedTo.Date);
            }


            var searchByMap = new Dictionary<IssueTypeSearchBy, string>
            {
                [IssueTypeSearchBy.IssueType] = "sit.IssueType",
                [IssueTypeSearchBy.CreatedUser] = "CONCAT(uC.FirstName,' ',uC.LastName)",
                [IssueTypeSearchBy.ModifiedUser] = "CONCAT(uU.FirstName,' ',uU.LastName)",
                [IssueTypeSearchBy.DeletedUser] = "CONCAT(uD.FirstName,' ',uD.LastName)"
            };

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                if (filter.SearchBy.HasValue && searchByMap.TryGetValue(filter.SearchBy.Value, out var col))
                    baseSql.Append($" AND {col} LIKE @Search");
                else
                    baseSql.Append(@"
            AND (
                sit.IssueType LIKE @Search
             OR uC.FirstName LIKE @Search OR uC.LastName LIKE @Search
             OR uU.FirstName LIKE @Search OR uU.LastName LIKE @Search
             OR uD.FirstName LIKE @Search OR uD.LastName LIKE @Search
            )");

                parameters.Add("Search", $"%{filter.SearchText}%");
            }



            var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["IssueTypeId"] = "sit.SysIssueTypeId",
                ["IssueTypeName"] = "sit.IssueType",
                ["Active"] = "sit.IsActive",
                ["Deleted"] = "sit.IsDeleted",
                ["CreatedDate"] = "sit.CreatedAt",
                ["ModifiedDate"] = "sit.UpdatedAt",
                ["DeletedDate"] = "sit.DeletedAt",
            };

            var sortOrder = filter.SortOrder == SortOrder.Desc ? "DESC" : "ASC";
            var sortKey = string.IsNullOrWhiteSpace(filter.SortBy)
                ? "IssueTypeName"
                : filter.SortBy;

            if (!sortMap.TryGetValue(sortKey, out var sortColumn))
                sortColumn = sortMap["IssueTypeId"];



            var countSql = "SELECT COUNT(1) " + baseSql;


            var itemsSql = $@"
            SELECT
                sit.SysIssueTypeId AS IssueTypeId,
                sit.IssueType AS IssueType,
                sit.IsActive AS Active,
                sit.IsDeleted AS Deleted,
                sit.CreatedAt AS CreatedDate,
                sit.UpdatedAt AS ModifiedDate,
                sit.DeletedAt AS DeletedDate,
                CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
                ISNULL(CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')), '') AS ModifiedUser,
                ISNULL(CONCAT(uD.FirstName, ' ', ISNULL(uD.LastName, '')), '') AS DeletedUser
            {baseSql}
            ORDER BY {sortColumn} {sortOrder}
            OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;
            ";

            using var connection = _context.CreateConnection();

            var totalCount = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

            var items = await connection.QueryAsync<SysIssueTypeDTO>(
                new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken));

            return new PagedResultDto<SysIssueTypeDTO>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = limit
            };
        }



        public async Task<List<SysIssueTypeActiveDTO?>> GetActiveAsync(
         CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT 
                SysIssueTypeId As IssueTypeId,
                IssueType      As IssueType
                FROM dbo.SysIssueType 
                WHERE IsDeleted = 0 And IsActive = 1 ORDER BY IssueType ASC;
                ";

            using var connection = _context.CreateConnection();
            var entity = await connection.QueryAsync<SysIssueTypeActiveDTO?>(
                new CommandDefinition(sql, cancellationToken: cancellationToken)
            );
            return entity.ToList();
        }



        public async Task<bool> UpdateAsync(
           SysIssueType entity,
           CancellationToken cancellationToken = default)
        {
            var sets = new List<string>();
            var parameters = new DynamicParameters();

            parameters.Add("IssueType", entity.IssueType);
            parameters.Add("Active", entity.IsActive.HasValue
                ? (entity.IsActive.Value ? 1 : 0)
                : (int?)null);

            if (!string.IsNullOrWhiteSpace(entity.IssueType))
                sets.Add("IssueType = @IssueType");

            if (entity.IsActive.HasValue)
                sets.Add("IsActive = @Active");

            if (sets.Count == 0)
                return false;

            sets.Add("UpdatedBy = @UpdatedBy");
            sets.Add("UpdatedAt = SYSDATETIME()");

            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("IssueTypeId", entity.SysIssueTypeId);

            var sql = $@"
                UPDATE SysIssueType
                SET {string.Join(", ", sets)}
                WHERE SysIssueTypeId = @IssueTypeId
                  AND IsDeleted = 0
                  AND (
                        (@IssueType IS NOT NULL 
                            AND IssueType <> @IssueType)
                     OR (@Active IS NOT NULL 
                            AND IsActive <> @Active)
                  );";

            using var connection = _context.CreateConnection();
            var affected = await connection.ExecuteAsync(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return affected > 0;
        }

        public async Task<bool> UpdateAsync(
           SysIssueType entity,
           IDbConnection connection,
           IDbTransaction transaction,
           CancellationToken cancellationToken = default)
        {
            var sets = new List<string>();
            var parameters = new DynamicParameters();

            parameters.Add("IssueType", entity.IssueType);
            parameters.Add("Active", entity.IsActive.HasValue
                ? (entity.IsActive.Value ? 1 : 0)
                : (int?)null);

            if (!string.IsNullOrWhiteSpace(entity.IssueType))
                sets.Add("IssueType = @IssueType");

            if (entity.IsActive.HasValue)
                sets.Add("IsActive = @Active");

            if (sets.Count == 0)
                return false;

            sets.Add("UpdatedBy = @UpdatedBy");
            sets.Add("UpdatedAt = SYSDATETIME()");

            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("IssueTypeId", entity.SysIssueTypeId);

            var sql = $@"
                UPDATE SysIssueType
                SET {string.Join(", ", sets)}
                WHERE SysIssueTypeId = @IssueTypeId
                  AND IsDeleted = 0
                  AND (
                        (@IssueType IS NOT NULL 
                            AND IssueType <> @IssueType)
                     OR (@Active IS NOT NULL 
                            AND IsActive <> @Active)
                  );";

            var affected = await connection.ExecuteAsync(
                new CommandDefinition(sql, parameters, transaction: transaction, cancellationToken: cancellationToken));

            return affected > 0;
        }



        public async Task<bool> DeleteAsync
            (int id, int deletedBy,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE SysIssueType SET IsDeleted = 1, DeletedAt = SYSDATETIME(), DeletedBy = @deletedBy WHERE SysIssueTypeId = @IssueTypeId And IsDeleted = 0;";

            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(sql, new { IssueTypeId = id, deletedBy }, cancellationToken: cancellationToken));
            return affectedRows > 0;
        }



        public async Task<bool> RecoverIssueTypeAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE SysIssueType SET IsDeleted = 0, DeletedBy = NULL, DeletedAt = NULL, UpdatedBy = @LoginId, UpdatedAt = SYSDATETIME() WHERE SysIssueTypeId = @IssueTypeId And IsDeleted = 1";
            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open) connection.Open();
            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(sql, new { IssueTypeId = id, LoginId = loginId }, cancellationToken: cancellationToken));
            return affectedRows > 0;
        }


        public async Task<int> CreateAsync(
            SysIssueType entity,
            IDbConnection connection,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
                INSERT INTO SysIssueType (IssueType, CreatedBy)
                VALUES (@IssueType, @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            var cmd = new CommandDefinition(sql, new
            {
                entity.IssueType,
                entity.CreatedBy,
            }, transaction: transaction, cancellationToken: cancellationToken);
            return await connection.ExecuteScalarAsync<int>(cmd);
        }

    }
}
