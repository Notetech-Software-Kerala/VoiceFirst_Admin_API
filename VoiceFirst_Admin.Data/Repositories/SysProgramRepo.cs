using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class SysProgramRepo: ISysProgramRepo
    {
        private readonly IDapperContext _context;

     

        public SysProgramRepo(IDapperContext context)
        {
            _context = context;
        }

        public async Task<SysProgram?> ExistsByNameAsync(int applicationId, string name, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var sql = new StringBuilder("SELECT TOP 1 * FROM SysProgram WHERE ApplicationId = @ApplicationId AND ProgramName = @ProgramName");
            if (excludeId.HasValue)
                sql.Append(" AND SysProgramId <> @ExcludeId");

            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SysProgram>(
                new CommandDefinition(sql.ToString(), new { ApplicationId = applicationId, ProgramName = name, ExcludeId = excludeId }, cancellationToken: cancellationToken));
        }

        public async Task<SysProgram?> ExistsByLabelAsync(int applicationId, string label, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var sql = new StringBuilder("SELECT TOP 1 * FROM SysProgram WHERE ApplicationId = @ApplicationId AND LabelName = @Label");
            if (excludeId.HasValue)
                sql.Append(" AND SysProgramId <> @ExcludeId");

            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SysProgram>(
                new CommandDefinition(sql.ToString(), new { ApplicationId = applicationId, Label = label, ExcludeId = excludeId }, cancellationToken: cancellationToken));
        }

        public async Task<SysProgram?> ExistsByRouteAsync(int applicationId, string route, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var sql = new StringBuilder("SELECT TOP 1 * FROM SysProgram WHERE ApplicationId = @ApplicationId AND ProgramRoute = @Route");
            if (excludeId.HasValue)
                sql.Append(" AND SysProgramId <> @ExcludeId");

            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SysProgram>(
                new CommandDefinition(sql.ToString(), new { ApplicationId = applicationId, Route = route, ExcludeId = excludeId }, cancellationToken: cancellationToken));
        }

        public async Task<SysProgram?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT * FROM SysProgram WHERE SysProgramId = @ProgramId";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SysProgram>(
                new CommandDefinition(sql, new { ProgramId = id }, cancellationToken: cancellationToken));
        }

        public async Task<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramDto?> SysProgramGetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
        //    const string sql = @"
        //SELECT 
        //    p.SysProgramId AS ProgramId,
        //    p.ProgramName,
        //    p.LabelName AS Label,
        //    p.ProgramRoute AS Route,
        //    p.ApplicationId AS PlatformId,
        //    ISNULL(a.ApplicationName,'') AS PlatformName,
        //    p.CompanyId,
        //    ISNULL(c.CompanyName,'') AS CompanyName,
        //    ISNULL(p.IsActive,1) AS Active,
        //    ISNULL(p.IsDeleted,0) AS [Delete],
        //    CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
        //    p.CreatedAt AS CreatedDate,
        //    ISNULL(CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')), '') AS ModifiedUser,
        //    p.UpdatedAt AS ModifiedDate,
        //    ISNULL(CONCAT(uD.FirstName, ' ', ISNULL(uD.LastName, '')), '') AS DeletedUser,
        //    p.DeletedAt AS DeletedDate
        //FROM SysProgram p
        //LEFT JOIN Application a ON a.ApplicationId = p.ApplicationId
        //LEFT JOIN Company c ON c.CompanyId = p.CompanyId
        //LEFT JOIN Users uC ON uC.UserId = p.CreatedBy
        //LEFT JOIN Users uU ON uU.UserId = p.UpdatedBy
        //LEFT JOIN Users uD ON uD.UserId = p.DeletedBy
        //WHERE p.SysProgramId = @ProgramId;";

            const string sql = @"
        SELECT 
            p.SysProgramId AS ProgramId,
            p.ProgramName,
            p.LabelName AS Label,
            p.ProgramRoute AS Route,
            p.ApplicationId AS PlatformId,
            ISNULL(a.ApplicationName,'') AS PlatformName,
            
            ISNULL(p.IsActive,1) AS Active,
            ISNULL(p.IsDeleted,0) AS [Delete],
            CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
            p.CreatedAt AS CreatedDate,
            ISNULL(CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')), '') AS ModifiedUser,
            p.UpdatedAt AS ModifiedDate,
            ISNULL(CONCAT(uD.FirstName, ' ', ISNULL(uD.LastName, '')), '') AS DeletedUser,
            p.DeletedAt AS DeletedDate
        FROM SysProgram p
        LEFT JOIN Application a ON a.ApplicationId = p.ApplicationId     
        LEFT JOIN Users uC ON uC.UserId = p.CreatedBy
        LEFT JOIN Users uU ON uU.UserId = p.UpdatedBy
        LEFT JOIN Users uD ON uD.UserId = p.DeletedBy
        WHERE p.SysProgramId = @ProgramId;";

            using var connection = _context.CreateConnection();
            var dto = await connection.QueryFirstOrDefaultAsync<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramDto>(
                new CommandDefinition(sql, new { ProgramId = id }, cancellationToken: cancellationToken));
            if (dto == null) return null;

            var links = await GetLinksByProgramIdAsync(id, cancellationToken);
            dto.Action = links.ToList();
            return dto;
        }



        private async Task BulkInsertActionLinksAsync(IDbConnection connection, IDbTransaction tx, int programId, IEnumerable<int> actionIds, int createdBy, CancellationToken cancellationToken)
        {
            const string insertLink = @"INSERT INTO SysProgramActionsLink (ProgramId, ProgramActionId, CreatedBy) VALUES (@ProgramId, @ProgramActionId, @CreatedBy);";
            foreach (var actionId in actionIds)
            {
                await connection.ExecuteAsync(
                    new CommandDefinition(insertLink, new { ProgramId = programId, ProgramActionId = actionId, CreatedBy = createdBy }, transaction: tx, cancellationToken: cancellationToken));
            }
        }

        public async Task<SysProgram> CreateAsync(SysProgram entity, List<int> permissionIds, CancellationToken cancellationToken = default)
        {
            const string insertProgram = @"
            INSERT INTO SysProgram (ProgramName, LabelName, ProgramRoute, ApplicationId, CompanyId, CreatedBy)
            VALUES (@ProgramName, @LabelName, @ProgramRoute, @ApplicationId, @CompanyId, @CreatedBy);
            SELECT CAST(SCOPE_IDENTITY() AS int);";
          

            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open) connection.Open();
            using var tx = connection.BeginTransaction();
            try
            {
                var id = await connection.ExecuteScalarAsync<int>(
                    new CommandDefinition(insertProgram, new
                    {
                        ProgramName = entity.ProgramName,
                        LabelName = entity.LabelName,
                        ProgramRoute = entity.ProgramRoute,
                        ApplicationId = entity.ApplicationId,
                        CompanyId = entity.CompanyId,
                        CreatedBy = entity.CreatedBy
                    }, transaction: tx, cancellationToken: cancellationToken));
                entity.SysProgramId = id;

                // link permissions/actions if provided
                if (permissionIds != null && permissionIds.Count > 0)
                {
                    await BulkInsertActionLinksAsync(connection, tx, id, permissionIds, entity.CreatedBy ?? 0, cancellationToken);
                }

                tx.Commit();
                // Optionally load links if needed by service
                return entity;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink.SysProgramActionLinkDTO>> GetLinksByProgramIdAsync(int programId, CancellationToken cancellationToken = default)
        {
            const string sql = @"
            SELECT 
                l.ProgramActionId AS ActionId,
                a.ProgramActionName AS ActionName,
                ISNULL(l.IsActive, 1) AS Active,
                CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
                l.CreatedAt AS CreatedDate,
                CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')) AS ModifiedUser,
                l.UpdatedAt AS ModifiedDate
            FROM SysProgramActionsLink l
            INNER JOIN SysProgramActions a ON a.SysProgramActionId = l.ProgramActionId
            INNER JOIN Users uC ON uC.UserId = l.CreatedBy
            LEFT JOIN Users uU ON uU.UserId = l.UpdatedBy
            WHERE l.ProgramId = @ProgramId AND l.IsActive = 1;
            ";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink.SysProgramActionLinkDTO>(
                new CommandDefinition(sql, new { ProgramId = programId }, cancellationToken: cancellationToken));
        }




        public async Task<SysProgramActionsLink?> GetActiveByIdAsync
          (int SysProgramActionsLink, CancellationToken cancellationToken = default)
        {
            var sql = "SELECT * FROM SysProgramActionsLink WHERE SysProgramActionLinkId = @SysProgramActionLinkId And IsActive = 1 ;";

            var cmd = new CommandDefinition(sql, new { SysProgramActionsLink = SysProgramActionsLink }, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            var entity = await connection.QueryFirstOrDefaultAsync<SysProgramActionsLink>(cmd);
            return entity;
        }


        public async Task<bool> DeleteAsync(int id, int deletedBy, CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE SysProgram SET IsActive = 0 ,IsDeleted = 1, DeletedAt = SYSDATETIME(),DeletedBy = @deletedBy  WHERE SysProgramId = @ProgramId";

            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(sql, new { ProgramId = id, deletedBy }, cancellationToken: cancellationToken));
            return affectedRows > 0;
        }


        public async Task<int> RecoverProgramAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE SysProgram SET IsDeleted = 0 ,DeletedBy = NULL, DeletedAt = NULL , UpdatedBy = @LoginId, UpdatedAt = SYSDATETIME(),IsActive = 1  WHERE SysProgramId = @ProgramId";
            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(sql, new { ProgramId = id, LoginId = loginId }, cancellationToken: cancellationToken));
            return affectedRows;
        }

        //public async Task<VoiceFirst_Admin.Utilities.DTOs.Shared.PagedResultDto<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramDto>> GetAllAsync(VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramFilterDTO filter, CancellationToken cancellationToken = default)
        //{
        //    var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
        //    var limit = filter.Limit <= 0 ? 10 : filter.Limit;
        //    var offset = (page - 1) * limit;

        //    var parameters = new DynamicParameters();
        //    parameters.Add("Offset", offset);
        //    parameters.Add("Limit", limit);

        //    var baseSql = new StringBuilder(@"
        //    FROM SysProgram p
        //    LEFT JOIN Application a ON a.ApplicationId = p.ApplicationId
        //    LEFT JOIN Company c ON c.CompanyId = p.CompanyId
        //    INNER JOIN Users uC ON uC.UserId = p.CreatedBy
        //    LEFT JOIN Users uU ON uU.UserId = p.UpdatedBy
        //    LEFT JOIN Users uD ON uD.UserId = p.DeletedBy WHERE 1=1
        //    ");

        //    if (filter.Deleted.HasValue)
        //    {
        //        baseSql.Append(" AND p.IsDeleted = @IsDeleted");
        //        parameters.Add("IsDeleted", filter.Deleted.Value);
        //    }

        //    if (filter.Active.HasValue)
        //    {
        //        baseSql.Append(" AND p.IsActive = @IsActive");
        //        parameters.Add("IsActive", filter.Active.Value);
        //    }

        //    if (!string.IsNullOrWhiteSpace(filter.CreatedFromDate) && DateTime.TryParse(filter.CreatedFromDate, out var createdFrom))
        //    {
        //        baseSql.Append(" AND p.CreatedAt >= @CreatedFrom");
        //        parameters.Add("CreatedFrom", createdFrom);
        //    }
        //    if (!string.IsNullOrWhiteSpace(filter.CreatedToDate) && DateTime.TryParse(filter.CreatedToDate, out var createdTo))
        //    {
        //        baseSql.Append(" AND p.CreatedAt < DATEADD(day, 1, @CreatedTo)");
        //        parameters.Add("CreatedTo", createdTo.Date);
        //    }

        //    if (!string.IsNullOrWhiteSpace(filter.UpdatedFromDate) && DateTime.TryParse(filter.UpdatedFromDate, out var updatedFrom))
        //    {
        //        baseSql.Append(" AND p.UpdatedAt >= @UpdatedFrom");
        //        parameters.Add("UpdatedFrom", updatedFrom);
        //    }
        //    if (!string.IsNullOrWhiteSpace(filter.UpdatedToDate) && DateTime.TryParse(filter.UpdatedToDate, out var updatedTo))
        //    {
        //        baseSql.Append(" AND p.UpdatedAt < DATEADD(day, 1, @UpdatedTo)");
        //        parameters.Add("UpdatedTo", updatedTo.Date);
        //    }

        //    if (!string.IsNullOrWhiteSpace(filter.DeletedFromDate) && DateTime.TryParse(filter.DeletedFromDate, out var deletedFrom))
        //    {
        //        baseSql.Append(" AND p.DeletedAt >= @DeletedFrom");
        //        parameters.Add("DeletedFrom", deletedFrom);
        //    }
        //    if (!string.IsNullOrWhiteSpace(filter.DeletedToDate) && DateTime.TryParse(filter.DeletedToDate, out var deletedTo))
        //    {
        //        baseSql.Append(" AND p.DeletedAt < DATEADD(day, 1, @DeletedTo)");
        //        parameters.Add("DeletedTo", deletedTo.Date);
        //    }

        //    var searchByMap = new Dictionary<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramSearchBy, string>
        //    {
        //        [VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramSearchBy.ProgramName] = "p.ProgramName",
        //        [VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramSearchBy.Label] = "p.LabelName",
        //        [VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramSearchBy.Route] = "p.ProgramRoute",
        //        [VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramSearchBy.PlatformName] = "a.ApplicationName",
        //        [VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramSearchBy.CompanyName] = "c.CompanyName",
        //        [VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramSearchBy.CreatedUser] = "CONCAT(uC.FirstName,' ',uC.LastName)",
        //        [VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramSearchBy.ModifiedUser] = "CONCAT(uU.FirstName,' ',uU.LastName)",
        //        [VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramSearchBy.DeletedUser] = "CONCAT(uD.FirstName,' ',uD.LastName)"
        //    };

        //    if (!string.IsNullOrWhiteSpace(filter.SearchText))
        //    {
        //        if (filter.SearchBy.HasValue && searchByMap.TryGetValue(filter.SearchBy.Value, out var col))
        //        {
        //            baseSql.Append($" AND {col} LIKE @Search");
        //        }
        //        else
        //        {
        //            baseSql.Append(@"
        //    AND (
        //        p.ProgramName LIKE @Search OR p.LabelName LIKE @Search OR p.ProgramRoute LIKE @Search
        //     OR a.ApplicationName LIKE @Search OR c.CompanyName LIKE @Search
        //     OR uC.FirstName LIKE @Search OR uC.LastName LIKE @Search
        //     OR uU.FirstName LIKE @Search OR uU.LastName LIKE @Search
        //     OR uD.FirstName LIKE @Search OR uD.LastName LIKE @Search
        //    )");
        //        }
        //        parameters.Add("Search", $"%{filter.SearchText}%");
        //    }

        //    var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        //    {
        //        ["ProgramId"] = "p.SysProgramId",
        //        ["ProgramName"] = "p.ProgramName",
        //        ["Label"] = "p.LabelName",
        //        ["Route"] = "p.ProgramRoute",
        //        ["PlatformName"] = "a.ApplicationName",
        //        ["CompanyName"] = "c.CompanyName",
        //        ["Active"] = "p.IsActive",
        //        ["Deleted"] = "p.IsDeleted",
        //        ["CreatedDate"] = "p.CreatedAt",
        //        ["ModifiedDate"] = "p.UpdatedAt",
        //        ["DeletedDate"] = "p.DeletedAt",
        //    };

        //    var sortOrder = filter.SortOrder == VoiceFirst_Admin.Utilities.DTOs.Shared.SortOrder.Desc ? "DESC" : "ASC";
        //    var sortKey = string.IsNullOrWhiteSpace(filter.SortBy) ? "ProgramId" : filter.SortBy;
        //    if (!sortMap.TryGetValue(sortKey, out var sortColumn))
        //        sortColumn = sortMap["ProgramId"];

        //    var countSql = "SELECT COUNT(1) " + baseSql.ToString();

        //    var itemsSql = $@"
        //    SELECT
        //        p.SysProgramId AS ProgramId,
        //        p.ProgramName,
        //        p.LabelName AS Label,
        //        p.ProgramRoute AS Route,
        //        p.ApplicationId AS PlatformId,
        //        ISNULL(a.ApplicationName,'') AS PlatformName,
        //        p.CompanyId,
        //        ISNULL(c.CompanyName,'') AS CompanyName,
        //        ISNULL(p.IsActive,1) AS Active,
        //        ISNULL(p.IsDeleted,0) AS [Delete],
        //        CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
        //        p.CreatedAt AS CreatedDate,
        //        ISNULL(CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')), '') AS ModifiedUser,
        //        p.UpdatedAt AS ModifiedDate,
        //        ISNULL(CONCAT(uD.FirstName, ' ', ISNULL(uD.LastName, '')), '') AS DeletedUser,
        //        p.DeletedAt AS DeletedDate
        //    {baseSql}
        //    ORDER BY {sortColumn} {sortOrder}
        //    OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;";

        //    using var connection = _context.CreateConnection();
        //    var totalCount = await connection.ExecuteScalarAsync<int>(
        //        new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

        //    var items = (await connection.QueryAsync<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramDto>(
        //        new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken))).ToList();

        //    foreach (var item in items)
        //    {
        //        var links = await GetLinksByProgramIdAsync(item.ProgramId, cancellationToken);
        //        item.Action = links.ToList();
        //    }

        //    return new VoiceFirst_Admin.Utilities.DTOs.Shared.PagedResultDto<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramDto>
        //    {
        //        Items = items,
        //        TotalCount = totalCount,
        //        PageNumber = page,
        //        PageSize = limit
        //    };
        //}

        public async Task<VoiceFirst_Admin.Utilities.DTOs.Shared.PagedResultDto<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramDto>> GetAllAsync(VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramFilterDTO filter, CancellationToken cancellationToken = default)
        {
            var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var limit = filter.Limit <= 0 ? 10 : filter.Limit;
            var offset = (page - 1) * limit;

            var parameters = new DynamicParameters();
            parameters.Add("Offset", offset);
            parameters.Add("Limit", limit);

            var baseSql = new StringBuilder(@"
            FROM SysProgram p
            LEFT JOIN Application a ON a.ApplicationId = p.ApplicationId            
            INNER JOIN Users uC ON uC.UserId = p.CreatedBy
            LEFT JOIN Users uU ON uU.UserId = p.UpdatedBy
            LEFT JOIN Users uD ON uD.UserId = p.DeletedBy WHERE 1=1
            ");

            if (filter.Deleted.HasValue)
            {
                baseSql.Append(" AND p.IsDeleted = @IsDeleted");
                parameters.Add("IsDeleted", filter.Deleted.Value);
            }

            if (filter.Active.HasValue)
            {
                baseSql.Append(" AND p.IsActive = @IsActive");
                parameters.Add("IsActive", filter.Active.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.CreatedFromDate) && DateTime.TryParse(filter.CreatedFromDate, out var createdFrom))
            {
                baseSql.Append(" AND p.CreatedAt >= @CreatedFrom");
                parameters.Add("CreatedFrom", createdFrom);
            }
            if (!string.IsNullOrWhiteSpace(filter.CreatedToDate) && DateTime.TryParse(filter.CreatedToDate, out var createdTo))
            {
                baseSql.Append(" AND p.CreatedAt < DATEADD(day, 1, @CreatedTo)");
                parameters.Add("CreatedTo", createdTo.Date);
            }

            if (!string.IsNullOrWhiteSpace(filter.UpdatedFromDate) && DateTime.TryParse(filter.UpdatedFromDate, out var updatedFrom))
            {
                baseSql.Append(" AND p.UpdatedAt >= @UpdatedFrom");
                parameters.Add("UpdatedFrom", updatedFrom);
            }
            if (!string.IsNullOrWhiteSpace(filter.UpdatedToDate) && DateTime.TryParse(filter.UpdatedToDate, out var updatedTo))
            {
                baseSql.Append(" AND p.UpdatedAt < DATEADD(day, 1, @UpdatedTo)");
                parameters.Add("UpdatedTo", updatedTo.Date);
            }

            if (!string.IsNullOrWhiteSpace(filter.DeletedFromDate) && DateTime.TryParse(filter.DeletedFromDate, out var deletedFrom))
            {
                baseSql.Append(" AND p.DeletedAt >= @DeletedFrom");
                parameters.Add("DeletedFrom", deletedFrom);
            }
            if (!string.IsNullOrWhiteSpace(filter.DeletedToDate) && DateTime.TryParse(filter.DeletedToDate, out var deletedTo))
            {
                baseSql.Append(" AND p.DeletedAt < DATEADD(day, 1, @DeletedTo)");
                parameters.Add("DeletedTo", deletedTo.Date);
            }

            var searchByMap = new Dictionary<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramSearchBy, string>
            {
                [VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramSearchBy.ProgramName] = "p.ProgramName",
                [VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramSearchBy.Label] = "p.LabelName",
                [VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramSearchBy.Route] = "p.ProgramRoute",
                [VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramSearchBy.PlatformName] = "a.ApplicationName",
                [VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramSearchBy.CreatedUser] = "CONCAT(uC.FirstName,' ',uC.LastName)",
                [VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramSearchBy.ModifiedUser] = "CONCAT(uU.FirstName,' ',uU.LastName)",
                [VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramSearchBy.DeletedUser] = "CONCAT(uD.FirstName,' ',uD.LastName)"
            };

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                if (filter.SearchBy.HasValue && searchByMap.TryGetValue(filter.SearchBy.Value, out var col))
                {
                    baseSql.Append($" AND {col} LIKE @Search");
                }
                else
                {
                    baseSql.Append(@"
            AND (
                p.ProgramName LIKE @Search OR p.LabelName LIKE @Search OR p.ProgramRoute LIKE @Search
             OR a.ApplicationName LIKE @Search OR c.CompanyName LIKE @Search
             OR uC.FirstName LIKE @Search OR uC.LastName LIKE @Search
             OR uU.FirstName LIKE @Search OR uU.LastName LIKE @Search
             OR uD.FirstName LIKE @Search OR uD.LastName LIKE @Search
            )");
                }
                parameters.Add("Search", $"%{filter.SearchText}%");
            }

            var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["ProgramId"] = "p.SysProgramId",
                ["ProgramName"] = "p.ProgramName",
                ["Label"] = "p.LabelName",
                ["Route"] = "p.ProgramRoute",
                ["PlatformName"] = "a.ApplicationName",
                ["Active"] = "p.IsActive",
                ["Deleted"] = "p.IsDeleted",
                ["CreatedDate"] = "p.CreatedAt",
                ["ModifiedDate"] = "p.UpdatedAt",
                ["DeletedDate"] = "p.DeletedAt",
            };

            var sortOrder = filter.SortOrder == VoiceFirst_Admin.Utilities.DTOs.Shared.SortOrder.Desc ? "DESC" : "ASC";
            var sortKey = string.IsNullOrWhiteSpace(filter.SortBy) ? "ProgramId" : filter.SortBy;
            if (!sortMap.TryGetValue(sortKey, out var sortColumn))
                sortColumn = sortMap["ProgramId"];

            var countSql = "SELECT COUNT(1) " + baseSql.ToString();

            var itemsSql = $@"
            SELECT
                p.SysProgramId AS ProgramId,
                p.ProgramName,
                p.LabelName AS Label,
                p.ProgramRoute AS Route,
                p.ApplicationId AS PlatformId,
                ISNULL(a.ApplicationName,'') AS PlatformName,
              
                ISNULL(p.IsActive,1) AS Active,
                ISNULL(p.IsDeleted,0) AS [Delete],
                CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
                p.CreatedAt AS CreatedDate,
                ISNULL(CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')), '') AS ModifiedUser,
                p.UpdatedAt AS ModifiedDate,
                ISNULL(CONCAT(uD.FirstName, ' ', ISNULL(uD.LastName, '')), '') AS DeletedUser,
                p.DeletedAt AS DeletedDate
            {baseSql}
            ORDER BY {sortColumn} {sortOrder}
            OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;";

            using var connection = _context.CreateConnection();
            var totalCount = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

            var items = (await connection.QueryAsync<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramDto>(
                new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken))).ToList();

            foreach (var item in items)
            {
                var links = await GetLinksByProgramIdAsync(item.ProgramId, cancellationToken);
                item.Action = links.ToList();
            }

            return new VoiceFirst_Admin.Utilities.DTOs.Shared.PagedResultDto<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = limit
            };
        }
    }
}
