using Dapper;
using System.Data;
using System.Text;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.Plan;
using VoiceFirst_Admin.Utilities.DTOs.Features.PlanProgramActoinLink;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class PlanRepo: IPlanRepo
    {
        private readonly IDapperContext _context;


        public PlanRepo(IDapperContext context)
        {
            _context = context;
        }

        public async Task<int> LinkPlanRoleAsync(int roleId, List<int> planId, int createdBy, CancellationToken cancellationToken = default)
        {
            const string insertSql = @"INSERT INTO PlanRoleLink (PlanId, RoleId, CreatedBy) VALUES (@PlanId, @RoleId, @CreatedBy);";
            using var connection = _context.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            using var tx = connection.BeginTransaction();
            try
            {
                var affected = 0;
                foreach (var pid in planId)
                {
                    affected += await connection.ExecuteAsync(new CommandDefinition(insertSql, new { PlanId = pid, RoleId = roleId, CreatedBy = createdBy }, transaction: tx, cancellationToken: cancellationToken));
                }
                tx.Commit();
                return affected;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<int>> GetExistingPlanIdsAsync(IEnumerable<int> planIds, CancellationToken cancellationToken = default)
        {
            var sql = "SELECT PlanId FROM dbo.[Plan] WHERE PlanId IN @PlanIds AND IsDeleted = 0";
            using var connection = _context.CreateConnection();
            var rows = await connection.QueryAsync<int>(new CommandDefinition(sql, new { PlanIds = planIds }, cancellationToken: cancellationToken));
            return rows;
        }

        //public async Task<PlanDetailDto?> 
        //    GetByIdAsync(int planId,IDbConnection connection,IDbTransaction transaction,
        //    CancellationToken cancellationToken = default)
        //{
        //    const string sql = @"
        //    SELECT
        //        p.PlanId,
        //        p.PlanName,
        //        p.IsActive AS Active,
        //        p.IsDeleted AS Deleted,
        //        CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
        //        p.CreatedAt AS CreatedDate,
        //        ISNULL(CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')), '') AS ModifiedUser,
        //        p.UpdatedAt AS ModifiedDate,
        //        ISNULL(CONCAT(uD.FirstName, ' ', ISNULL(uD.LastName, '')), '') AS DeletedUser,
        //        p.DeletedAt AS DeletedDate
        //    FROM dbo.[Plan] p
        //    INNER JOIN dbo.Users uC ON uC.UserId = p.CreatedBy
        //    LEFT JOIN dbo.Users uU ON uU.UserId = p.UpdatedBy
        //    LEFT JOIN dbo.Users uD ON uD.UserId = p.DeletedBy
        //    WHERE p.PlanId = @PlanId;";


        //    var dto = await connection.QueryFirstOrDefaultAsync<PlanDetailDto>(
        //        new CommandDefinition(sql, new { PlanId = planId },transaction, cancellationToken: cancellationToken));
        //    return dto;
        //}



        public async Task<PlanDetailDto?> GetByIdAsync
            (int id, 
            IDbConnection connection, 
            IDbTransaction transaction, 
            CancellationToken cancellationToken = default)
        {
           

            const string sql = @"
        SELECT 
            p.PlanId AS PlanId,
            p.PlanName,            
            p.IsActive AS Active,
            p.IsDeleted AS [Deleted],
            CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
            p.CreatedAt AS CreatedDate,
            ISNULL(CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')), '') AS ModifiedUser,
            p.UpdatedAt AS ModifiedDate,
            ISNULL(CONCAT(uD.FirstName, ' ', ISNULL(uD.LastName, '')), '') AS DeletedUser,
            p.DeletedAt AS DeletedDate
        FROM [Plan] p
        LEFT JOIN Users uC ON uC.UserId = p.CreatedBy
        LEFT JOIN Users uU ON uU.UserId = p.UpdatedBy
        LEFT JOIN Users uD ON uD.UserId = p.DeletedBy
        WHERE p.PlanId = @PlanId;";


            var dto = await connection.QueryFirstOrDefaultAsync<PlanDetailDto>(
                new CommandDefinition(sql, new { PlanId = id }, transaction, cancellationToken: cancellationToken));
            if (dto == null) return null;

            var links = await GetProgramDetailsByPlanIdAsync(id, connection, transaction, cancellationToken);
            dto.ProgramPlanDetails = links.ToList();
            return dto;
        }





        public async Task<PlanDetailDto> IsIdExistAsync(
         int planId,
         CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT  s.PlanId    As PlanId ,
                        s.IsDeleted            As Deleted      
                FROM dbo.[Plan] s
                WHERE PlanId = @PlanId
                 ;
                 ";

            using var connection = _context.CreateConnection();

            var dto = await connection.QuerySingleOrDefaultAsync<PlanDetailDto>(
                new CommandDefinition(
                    sql,
                    new { PlanId = planId },
                    cancellationToken: cancellationToken
                )
            );
            return dto;
        }

        public async Task<bool> DeleteAsync(int id, int deletedBy, CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE dbo.[Plan] SET  IsDeleted = 1, DeletedAt = SYSDATETIME(), DeletedBy = @DeletedBy WHERE PlanId = @PlanId And IsDeleted = 0;";
            using var connection = _context.CreateConnection();
            var affected = await connection.ExecuteAsync(new CommandDefinition(sql, new { PlanId = id, DeletedBy = deletedBy }, cancellationToken: cancellationToken));
            return affected > 0;
        }

        public async Task<bool> RecoverAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE dbo.[Plan] SET IsDeleted = 0, DeletedBy = NULL, DeletedAt = NULL, UpdatedBy = @LoginId, UpdatedAt = SYSDATETIME() WHERE PlanId = @PlanId And IsDeleted = 1;";
            using var connection = _context.CreateConnection();
            var affected = await connection.ExecuteAsync(new CommandDefinition(sql, new { PlanId = id, LoginId = loginId }, cancellationToken: cancellationToken));
            return affected > 0;
        }

        public async Task<PagedResultDto<PlanDetailDto>> GetAllAsync(PlanFilterDto filter, CancellationToken cancellationToken = default)
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

            var searchByMap = new Dictionary<PlanSearchBy, string>
            {
                [PlanSearchBy.PlanName] = "p.PlanName",
                [PlanSearchBy.CreatedUser] = "CONCAT(uC.FirstName,' ',uC.LastName)",
                [PlanSearchBy.ModifiedUser] = "CONCAT(uU.FirstName,' ',uU.LastName)",
                [PlanSearchBy.DeletedUser] = "CONCAT(uD.FirstName,' ',uD.LastName)"
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

      


            var sortOrder = filter.SortOrder == SortOrder.Desc ? "DESC" : "ASC";
            // 🔹 CHANGED: Default sorting is now ProgramName (Alphabetical order)
            var sortKey = string.IsNullOrWhiteSpace(filter.SortBy)
                ? "PlanName"
                : filter.SortBy;

            if (!sortMap.TryGetValue(sortKey, out var sortColumn))
                sortColumn = sortMap["PlanId"];

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

            

            return new PagedResultDto<PlanDetailDto>
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
            var sql = $"UPDATE dbo.[Plan] SET {string.Join(", ", sets)} WHERE PlanId = @PlanId AND IsDeleted = 0";
            using var connection = _context.CreateConnection();
            var affected = await connection.ExecuteAsync(new CommandDefinition(sql, p, cancellationToken: cancellationToken));
            return affected > 0;
        }

        public async Task UpsertPlanProgramActionLinksAsync(int planId, IEnumerable<PlanProgramActionLinkUpdateDto> actions, int userId, CancellationToken cancellationToken = default)
        {
            const string selectSql = @"SELECT TOP 1 * FROM dbo.[PlanProgramActionLink] WHERE PlanId = @PlanId AND ProgramActionLinkId = @ProgramActionLinkId";
            const string insertSql = @"INSERT INTO dbo.[PlanProgramActionLink] (PlanId, ProgramActionLinkId, IsActive, CreatedBy, CreatedAt) VALUES (@PlanId, @ProgramActionLinkId, @IsActive, @UserId, SYSDATETIME())";
            const string updateSql = @"UPDATE PlanProgramActionLink SET IsActive = @IsActive, UpdatedBy = @UserId, UpdatedAt = SYSDATETIME() WHERE PlanId = @PlanId AND ProgramActionLinkId = @ProgramActionLinkId";

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

        public async Task<IEnumerable<ProgramPlanDetailDto>> GetProgramDetailsByPlanIdAsync(int planId,
            IDbConnection connection,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default)
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
            FROM dbo.[PlanProgramActionLink] ppl
            INNER JOIN SysProgramActionsLink al ON al.SysProgramActionLinkId = ppl.ProgramActionLinkId
            INNER JOIN SysProgram sp ON sp.SysProgramId = al.ProgramId
            INNER JOIN SysProgramActions pa ON pa.SysProgramActionId = al.ProgramActionId
            INNER JOIN Users uC ON uC.UserId = ppl.CreatedBy
            LEFT JOIN Users uU ON uU.UserId = ppl.UpdatedBy
            LEFT JOIN Users uD ON uD.UserId = ppl.DeletedBy
            WHERE ppl.PlanId = @PlanId;";

            
            var rows = await connection.QueryAsync<dynamic>(
                new CommandDefinition(sql, 
                new { PlanId = planId },
                transaction,
                cancellationToken: cancellationToken));

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

                program.Actions.Add(new ProgramActionPlanDetailDto
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

        public async Task<PlanDto> PlanExistsAsync
           (string name, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var sql = "SELECT IsDeleted As Deleted,PlanId As PlanId  FROM [Plan] WHERE PlanName = @PlanName";
            if (excludeId.HasValue)
                sql += " AND PlanId <> @ExcludeId";

            var cmd = new CommandDefinition(sql, new { PlanName = name, ExcludeId = excludeId }, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            var entity = await connection.QueryFirstOrDefaultAsync<PlanDto>(cmd);
            return entity;
        }



        public async Task<Plan?> GetByNameAsync(string planName, CancellationToken cancellationToken = default)
        {
            const string sql = "SELECT TOP 1 * FROM dbo.[Plan] WHERE PlanName = @PlanName";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Plan>(new CommandDefinition(sql, new { PlanName = planName }, cancellationToken: cancellationToken));
        }

        //public async Task<int> CreatePlanAsync(VoiceFirst_Admin.Utilities.Models.Entities.Plan plan, CancellationToken cancellationToken = default)
        //{
        //    const string sql = @"INSERT INTO [Plan] (PlanName, CreatedBy) VALUES (@PlanName, @CreatedBy); SELECT CAST(SCOPE_IDENTITY() AS int);";
        //    using var connection = _context.CreateConnection();
        //    var id = await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { PlanName = plan.PlanName, CreatedBy = plan.CreatedBy }, cancellationToken: cancellationToken));
        //    return id;
        //}




        //public async Task LinkProgramActionLinksAsync(int planId, IEnumerable<int> programActionLinkIds, int createdBy, CancellationToken cancellationToken = default)
        //{
        //    const string insertSql = @"INSERT INTO dbo.PlanProgramActionLink (PlanId, ProgramActionLinkId, CreatedBy) VALUES (@PlanId, @ProgramActionLinkId, @CreatedBy);";
        //    using var connection = _context.CreateConnection();
        //    if (connection.State != System.Data.ConnectionState.Open) connection.Open();
        //    using var tx = connection.BeginTransaction();
        //    try
        //    {
        //        foreach (var id in programActionLinkIds)
        //        {
        //            await connection.ExecuteAsync(new CommandDefinition(insertSql, new { PlanId = planId, ProgramActionLinkId = id, CreatedBy = createdBy }, transaction: tx, cancellationToken: cancellationToken));
        //        }
        //        tx.Commit();
        //    }
        //    catch
        //    {
        //        tx.Rollback();
        //        throw;
        //    }
        //}
        public async Task<int> CreatePlanAsync(
            Plan plan,
            IDbConnection connection,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default)
        {
                const string sql = @"
            INSERT INTO [Plan] (PlanName, CreatedBy)
            VALUES (@PlanName, @CreatedBy);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

                return await connection.ExecuteScalarAsync<int>(
                    new CommandDefinition(
                        sql,
                        new { plan.PlanName, plan.CreatedBy },
                        transaction,
                        cancellationToken: cancellationToken));
          
           
        }

        public async Task<bool> BulkInsertActionLinksAsync(
        int planId,
        IEnumerable<int> programActionLinkIds,
        int createdBy,
        IDbConnection connection,
        IDbTransaction tx,
        CancellationToken cancellationToken)
        {
            if (programActionLinkIds == null || !programActionLinkIds.Any())
                return false;

            // -------------------------
            // CONVERT dynamic → int
            // -------------------------

            if (programActionLinkIds.Count() == 0)
                return false;

            // -------------------------
            // SQL
            // -------------------------
            const string sql = @"
            INSERT INTO dbo.PlanProgramActionLink
            (PlanId, ProgramActionLinkId, CreatedBy)
            VALUES (@PlanId, @ProgramActionLinkId, @CreatedBy);";

            // -------------------------
            // PARAMETER OBJECTS
            // -------------------------
            var parameters = programActionLinkIds.Select(id => new
            {
                PlanId = planId,
                ProgramActionLinkId = id,
                CreatedBy = createdBy
            });

            var rowsAffected = await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    parameters,
                    transaction: tx,
                    cancellationToken: cancellationToken));

            return rowsAffected > 0;
        }

        //public async Task LinkProgramActionLinksAsync(
        //int planId,
        //IEnumerable<int> programActionLinkIds,
        //int createdBy,
        //IDbConnection connection,
        //IDbTransaction transaction,
        //CancellationToken cancellationToken = default)
        //    {
        //        const string insertSql = @"
        //    INSERT INTO dbo.PlanProgramActionLink
        //    (PlanId, ProgramActionLinkId, CreatedBy)
        //    VALUES (@PlanId, @ProgramActionLinkId, @CreatedBy);";

        //        foreach (var id in programActionLinkIds)
        //        {
        //            await connection.ExecuteAsync(
        //                new CommandDefinition(
        //                    insertSql,
        //                    new { PlanId = planId, ProgramActionLinkId = id, CreatedBy = createdBy },
        //                    transaction,
        //                    cancellationToken: cancellationToken));
        //        }
        //    }



        public async Task<IEnumerable<PlanActiveDto>>
      GetActiveAsync(CancellationToken cancellationToken = default)
        {
            var sql = @"
        SELECT PlanId, PlanName
        FROM dbo.[Plan]
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
