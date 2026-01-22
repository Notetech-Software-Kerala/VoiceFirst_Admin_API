using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.Application;
using VoiceFirst_Admin.Utilities.DTOs.Features.Plan;
using VoiceFirst_Admin.Utilities.DTOs.Features.PlanProgramActoinLink;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class PlanRepo: IPlanRepo
    {
        private readonly IDapperContext _context;


        public PlanRepo(IDapperContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteAsync(int id, int deletedBy, CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE dbo.[Plan] SET IsActive = 0, IsDeleted = 1, DeletedAt = SYSDATETIME(), DeletedBy = @DeletedBy WHERE PlanId = @PlanId";
            using var connection = _context.CreateConnection();
            var affected = await connection.ExecuteAsync(new CommandDefinition(sql, new { PlanId = id, DeletedBy = deletedBy }, cancellationToken: cancellationToken));
            return affected > 0;
        }

        public async Task<int> RecoverPlanAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE dbo.[Plan] SET IsDeleted = 0, DeletedBy = NULL, DeletedAt = NULL, UpdatedBy = @LoginId, UpdatedAt = SYSDATETIME(), IsActive = 1 WHERE PlanId = @PlanId";
            using var connection = _context.CreateConnection();
            var affected = await connection.ExecuteAsync(new CommandDefinition(sql, new { PlanId = id, LoginId = loginId }, cancellationToken: cancellationToken));
            return affected;
        }

        public async Task<VoiceFirst_Admin.Utilities.DTOs.Shared.PagedResultDto<PlanDetailDto>> GetAllAsync(VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanFilterDto filter, CancellationToken cancellationToken = default)
        {
            var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var limit = filter.Limit <= 0 ? 10 : filter.Limit;
            var offset = (page - 1) * limit;

            var parameters = new DynamicParameters();
            parameters.Add("Offset", offset);
            parameters.Add("Limit", limit);

            var baseSql = new StringBuilder(@"
            FROM dbo.[Plan] p
            INNER JOIN dbo.Users uC ON uC.UserId = p.CreatedBy
            LEFT JOIN dbo.Users uU ON uU.UserId = p.UpdatedBy
            LEFT JOIN dbo.Users uD ON uD.UserId = p.DeletedBy WHERE 1=1
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

            var searchByMap = new Dictionary<VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanSearchBy, string>
            {
                [VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanSearchBy.PlanName] = "p.PlanName",
                [VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanSearchBy.CreatedUser] = "CONCAT(uC.FirstName,' ',uC.LastName)",
                [VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanSearchBy.UpdatedUser] = "CONCAT(uU.FirstName,' ',uU.LastName)",
                [VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanSearchBy.DeletedUser] = "CONCAT(uD.FirstName,' ',uD.LastName)"
            };

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                if (filter.SearchBy.HasValue && searchByMap.TryGetValue(filter.SearchBy.Value, out var col))
                    baseSql.Append($" AND {col} LIKE @Search");
                else
                    baseSql.Append(@"
            AND (
                p.PlanName LIKE @Search
             OR uC.FirstName LIKE @Search OR uC.LastName LIKE @Search
             OR uU.FirstName LIKE @Search OR uU.LastName LIKE @Search
             OR uD.FirstName LIKE @Search OR uD.LastName LIKE @Search
            )");

                parameters.Add("Search", $"%{filter.SearchText}%");
            }

            var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["PlanId"] = "p.PlanId",
                ["PlanName"] = "p.PlanName",
                ["Active"] = "p.IsActive",
                ["Deleted"] = "p.IsDeleted",
                ["CreatedDate"] = "p.CreatedAt",
                ["ModifiedDate"] = "p.UpdatedAt",
                ["DeletedDate"] = "p.DeletedAt",
            };

            var sortOrder = filter.SortOrder == VoiceFirst_Admin.Utilities.DTOs.Shared.SortOrder.Desc ? "DESC" : "ASC";
            var sortKey = string.IsNullOrWhiteSpace(filter.SortBy) ? "PlanId" : filter.SortBy;
            if (!sortMap.TryGetValue(sortKey, out var sortColumn)) sortColumn = sortMap["PlanId"];

            var countSql = "SELECT COUNT(1) " + baseSql.ToString();

            var itemsSql = $@"
            SELECT
                p.PlanId,
                p.PlanName,
                p.IsActive AS Active,
                p.IsDeleted AS Deleted,
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
            var totalCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));
            var items = await connection.QueryAsync<PlanDetailDto>(new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken));

            return new VoiceFirst_Admin.Utilities.DTOs.Shared.PagedResultDto<PlanDetailDto>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = limit
            };
        }

        public async Task<bool> UpdatePlanAsync(int planId, string? planName, bool? active, int updatedBy, CancellationToken cancellationToken = default)
        {
            var sets = new List<string>();
            var p = new DynamicParameters();
            p.Add("PlanId", planId);
            p.Add("UpdatedBy", updatedBy);
            if (!string.IsNullOrWhiteSpace(planName)) { sets.Add("PlanName = @PlanName"); p.Add("PlanName", planName); }
            if (active.HasValue) { sets.Add("IsActive = @IsActive"); p.Add("IsActive", active.Value); }
            if (sets.Count == 0) return false;
            sets.Add("UpdatedBy = @UpdatedBy");
            sets.Add("UpdatedAt = SYSDATETIME()");
            var sql = $"UPDATE [Plan] SET {string.Join(", ", sets)} WHERE PlanId = @PlanId AND IsDeleted = 0";
            using var connection = _context.CreateConnection();
            var affected = await connection.ExecuteAsync(new CommandDefinition(sql, p, cancellationToken: cancellationToken));
            return affected > 0;
        }

        public async Task UpsertPlanProgramActionLinksAsync(int planId, IEnumerable<VoiceFirst_Admin.Utilities.DTOs.Features.PlanProgramActoinLink.PlanProgramActionLinkUpdateDto> actions, int userId, CancellationToken cancellationToken = default)
        {
            const string selectSql = @"SELECT TOP 1 * FROM dbo.PlanProgramActionLink WHERE PlanId = @PlanId AND ProgramActionLinkId = @ProgramActionLinkId";
            const string insertSql = @"INSERT INTO dbo.PlanProgramActionLink (PlanId, ProgramActionLinkId, IsActive, CreatedBy, CreatedAt) VALUES (@PlanId, @ProgramActionLinkId, @IsActive, @UserId, SYSDATETIME())";
            const string updateSql = @"UPDATE dbo.PlanProgramActionLink SET IsActive = @IsActive, UpdatedBy = @UserId, UpdatedAt = SYSDATETIME() WHERE PlanId = @PlanId AND ProgramActionLinkId = @ProgramActionLinkId";

            using var connection = _context.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            using var tx = connection.BeginTransaction();
            try
            {
                foreach (var a in actions)
                {
                    var exists = await connection.QueryFirstOrDefaultAsync<int>(new CommandDefinition(selectSql, new { PlanId = planId, ProgramActionLinkId = a.ProgramActionLinkId }, transaction: tx, cancellationToken: cancellationToken));
                    if (exists == 0)
                    {
                        await connection.ExecuteAsync(new CommandDefinition(insertSql, new { PlanId = planId, ProgramActionLinkId = a.ProgramActionLinkId, IsActive = a.Active, UserId = userId }, transaction: tx, cancellationToken: cancellationToken));
                    }
                    else
                    {
                        await connection.ExecuteAsync(new CommandDefinition(updateSql, new { PlanId = planId, ProgramActionLinkId = a.ProgramActionLinkId, IsActive = a.Active, UserId = userId }, transaction: tx, cancellationToken: cancellationToken));
                    }
                }
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<ProgramPlanDetailDto>> GetProgramDetailsByPlanIdAsync(int planId, CancellationToken cancellationToken = default)
        {
            const string sql = @"
            SELECT 
                sp.SysProgramId AS ProgramId,
                sp.ProgramName,
                al.SysProgramActionLinkId AS ActionLinkId,
                pa.ProgramActionName AS ActionName,
                ppl.IsActive AS Active,
                ppl.IsDeleted AS Deleted,
                CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
                ppl.CreatedAt AS CreatedDate,
                ISNULL(CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')), '') AS ModifiedUser,
                ppl.UpdatedAt AS ModifiedDate,
                ISNULL(CONCAT(uD.FirstName, ' ', ISNULL(uD.LastName, '')), '') AS DeletedUser,
                ppl.DeletedAt AS DeletedDate
            FROM PlanProgramActionLink ppl
            INNER JOIN SysProgramActionsLink al ON al.SysProgramActionLinkId = ppl.ProgramActionLinkId
            INNER JOIN SysProgram sp ON sp.SysProgramId = al.ProgramId
            INNER JOIN SysProgramActions pa ON pa.SysProgramActionId = al.ProgramActionId
            INNER JOIN Users uC ON uC.UserId = ppl.CreatedBy
            LEFT JOIN Users uU ON uU.UserId = ppl.UpdatedBy
            LEFT JOIN Users uD ON uD.UserId = ppl.DeletedBy
            WHERE ppl.PlanId = @PlanId;";

            using var connection = _context.CreateConnection();
            var rows = await connection.QueryAsync<dynamic>(new CommandDefinition(sql, new { PlanId = planId }, cancellationToken: cancellationToken));

            var programs = new Dictionary<int, ProgramPlanDetailDto>();
            foreach (var row in rows)
            {
                if (!programs.TryGetValue((int)row.ProgramId, out var program))
                {
                    program = new ProgramPlanDetailDto
                    {
                        ProgramId = row.ProgramId,
                        ProgramName = row.ProgramName
                    };
                    programs[(int)row.ProgramId] = program;
                }

                program.Action.Add(new ProgramActionPlanDetailDto
                {
                    ActionLinkId = row.ActionLinkId,
                    ActionName = row.ActionName,
                    Active = row.Active,
                    Deleted = row.Deleted,
                    CreatedUser = row.CreatedUser,
                    CreatedDate = row.CreatedDate,
                    ModifiedUser = row.ModifiedUser,
                    ModifiedDate = row.ModifiedDate,
                    DeletedUser = row.DeletedUser,
                    DeletedDate = row.DeletedDate
                });
            }

            return programs.Values.ToList();
        }

        public async Task<VoiceFirst_Admin.Utilities.Models.Entities.Plan?> GetByNameAsync(string planName, CancellationToken cancellationToken = default)
        {
            const string sql = "SELECT TOP 1 * FROM [Plan] WHERE PlanName = @PlanName";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<VoiceFirst_Admin.Utilities.Models.Entities.Plan>(new CommandDefinition(sql, new { PlanName = planName }, cancellationToken: cancellationToken));
        }

        public async Task<int> CreatePlanAsync(VoiceFirst_Admin.Utilities.Models.Entities.Plan plan, CancellationToken cancellationToken = default)
        {
            const string sql = @"INSERT INTO [Plan] (PlanName, CreatedBy) VALUES (@PlanName, @CreatedBy); SELECT CAST(SCOPE_IDENTITY() AS int);";
            using var connection = _context.CreateConnection();
            var id = await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { PlanName = plan.PlanName, CreatedBy = plan.CreatedBy }, cancellationToken: cancellationToken));
            return id;
        }

        public async Task LinkProgramActionLinksAsync(int planId, IEnumerable<int> programActionLinkIds, int createdBy, CancellationToken cancellationToken = default)
        {
            const string insertSql = @"INSERT INTO dbo.PlanProgramActionLink (PlanId, ProgramActionLinkId, CreatedBy) VALUES (@PlanId, @ProgramActionLinkId, @CreatedBy);";
            using var connection = _context.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            using var tx = connection.BeginTransaction();
            try
            {
                foreach (var id in programActionLinkIds)
                {
                    await connection.ExecuteAsync(new CommandDefinition(insertSql, new { PlanId = planId, ProgramActionLinkId = id, CreatedBy = createdBy }, transaction: tx, cancellationToken: cancellationToken));
                }
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }


        public async Task<IEnumerable<PlanActiveDto>>
      GetActiveAsync(CancellationToken cancellationToken = default)
        {
            var sql = @"
        SELECT PlanId, PlanName
        FROM [Plan]
        WHERE IsActive = 1
          AND IsDeleted = 0
        ORDER BY PlanName ASC;
    ";

            using var connection = _context.CreateConnection();

            var entities = await connection.QueryAsync<PlanActiveDto>(
                new CommandDefinition(sql, cancellationToken: cancellationToken)
            );

            return entities;
        }

    }
}
