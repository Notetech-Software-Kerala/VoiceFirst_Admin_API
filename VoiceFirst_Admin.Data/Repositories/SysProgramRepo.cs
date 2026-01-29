using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
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


        public async Task<int> CreateAsync(
            SysProgram entity,IDbConnection connection, IDbTransaction transaction
            , CancellationToken cancellationToken = default)
        {
            const string insertProgram = @"
            INSERT INTO SysProgram (ProgramName, LabelName, ProgramRoute, ApplicationId, CompanyId, CreatedBy)
            VALUES (@ProgramName, @LabelName, @ProgramRoute, @ApplicationId, @CompanyId, @CreatedBy);
            SELECT CAST(SCOPE_IDENTITY() AS int);";           
           
               

                return await connection.ExecuteScalarAsync<int>(
                    new CommandDefinition(insertProgram, new
                    {
                        ProgramName = entity.ProgramName,
                        LabelName = entity.LabelName,
                        ProgramRoute = entity.ProgramRoute,
                        ApplicationId = entity.ApplicationId,
                        CompanyId = entity.CompanyId,
                        CreatedBy = entity.CreatedBy
                    }, transaction, cancellationToken: cancellationToken));                   
        }

        public async Task<bool> BulkInsertActionLinksAsync(
          int programId,
          IEnumerable<dynamic> actionIds,
          int createdBy,
          IDbConnection connection,
          IDbTransaction tx,
          CancellationToken cancellationToken)
        {
            const string sql = @"
            INSERT INTO SysProgramActionsLink (ProgramId, ProgramActionId, CreatedBy)
            VALUES (@ProgramId, @ProgramActionId, @CreatedBy);";

            var parameters = actionIds.Select(actionId => new
            {
                ProgramId = programId,
                ProgramActionId = actionId,
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


        public async Task<bool> BulkUpdateActionLinksAsync(
     int programId,
     IEnumerable<SysProgramActionLinkUpdateDTO> dtos,
     int updatedBy,
     IDbConnection connection,
     IDbTransaction tx,
     CancellationToken cancellationToken)
        {
            const string sql = @"
UPDATE SysProgramActionsLink
SET IsActive   = @IsActive,
    UpdatedBy = @UpdatedBy,
    UpdatedAt = SYSDATETIME()
WHERE ProgramId = @ProgramId
  AND ProgramActionId = @ProgramActionId
  AND IsActive <> @IsActive;
";

            var parameters = dtos.Select(dto => new
            {
                ProgramId = programId,
                ProgramActionId = dto.ActionId,
                IsActive = dto.Active,
                UpdatedBy = updatedBy
            });

            var rowsAffected = await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    parameters,
                    transaction: tx,
                    cancellationToken: cancellationToken
                ));

            return rowsAffected > 0;
        }

        public async Task<SysProgramDto?> GetByIdAsync
            (int id,IDbConnection connection,IDbTransaction transaction, CancellationToken cancellationToken = default)
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
            
            p.IsActive AS Active,
            p.IsDeleted AS [Deleted],
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

          
            var dto = await connection.QueryFirstOrDefaultAsync<SysProgramDto>(
                new CommandDefinition(sql, new { ProgramId = id }, transaction, cancellationToken: cancellationToken));
            if (dto == null) return null;

            var links = await GetLinksByProgramIdAsync(id, cancellationToken);
            dto.Action = links.ToList();
            return dto;
        }


        public async Task<SysProgramDto> IsIdExistAsync(
           int programId,
           CancellationToken cancellationToken = default)
        {
            const string sql = @"
                    SELECT  s.SysProgramId   AS ProgramId,
                            s.IsDeleted      AS Deleted,
                            s.IsActive       AS Active,
                            s.ApplicationId  AS PlatformId
                    FROM dbo.SysProgram s
                    WHERE s.SysProgramId = @ProgramId;
                ";


            using var connection = _context.CreateConnection();

            var dto = await connection.QuerySingleOrDefaultAsync<SysProgramDto>(
                new CommandDefinition(
                    sql,
                    new { ProgramId = programId },
                    cancellationToken: cancellationToken
                )
            );
            return dto;
        }



        public async Task<List<SysProgramLookUp>>
           GetProgramLookupAsync(CancellationToken cancellationToken = default)
        {
            //const string sql = @"
            //SELECT 
            //    p.SysProgramId AS ProgramId,
            //    p.ProgramName,
            //    p.LabelName AS Label,
            //    p.ProgramRoute AS Route,
            //    ISNULL(a.ApplicationName,'') AS PlatformName,
            //    ISNULL(c.CompanyName,'') AS CompanyName
            //FROM SysProgram p
            //LEFT JOIN Application a ON a.ApplicationId = p.ApplicationId
            //LEFT JOIN Company c ON c.CompanyId = p.CompanyId
            //WHERE p.IsDeleted = 0 AND p.IsActive = 1
            //ORDER BY p.ProgramName ASC;";

            const string sql = @"
            SELECT 
                p.SysProgramId AS ProgramId,
                p.ProgramName,
                p.LabelName AS Label,
                p.ProgramRoute AS Route,
                ISNULL(a.ApplicationName,'') AS PlatformName                
            FROM SysProgram p
            LEFT JOIN Application a ON a.ApplicationId = p.ApplicationId          
            WHERE p.IsDeleted = 0 AND p.IsActive = 1 And p.CompanyId = 0
            ORDER BY p.ProgramName ASC;";

            using var connection = _context.CreateConnection();
            var items = (await connection.QueryAsync<SysProgramLookUp>(
                new CommandDefinition(sql, cancellationToken: cancellationToken))).ToList();

            foreach (var item in items)
            {
                var actions = await GetActionLookupByProgramIdAsync(item.ProgramId, cancellationToken);
                item.Action = actions.ToList();
            }

            return items.ToList();
        }


        public async Task<IEnumerable<SysProgramActionLinkDTO>> GetLinksByProgramIdAsync(int programId, CancellationToken cancellationToken = default)
        {
            const string sql = @"
            SELECT 
                l.ProgramActionId AS ActionId,
                a.ProgramActionName AS ActionName,
                l.IsActive AS Active,
                CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
                l.CreatedAt AS CreatedDate,
                CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')) AS ModifiedUser,
                l.UpdatedAt AS ModifiedDate
            FROM SysProgramActionsLink l
            INNER JOIN SysProgramActions a ON a.SysProgramActionId = l.ProgramActionId
            INNER JOIN Users uC ON uC.UserId = l.CreatedBy
            LEFT JOIN Users uU ON uU.UserId = l.UpdatedBy
            WHERE l.ProgramId = @ProgramId ;
            ";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<SysProgramActionLinkDTO>(
                new CommandDefinition(sql, new { ProgramId = programId }, cancellationToken: cancellationToken));
        }

       
        public async Task<List<SysProgramActionLinkLookUp>> 
            GetActionLookupByProgramIdAsync(int programId,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
            SELECT 
                l.SysProgramActionLinkId AS ActionLinkId,
                a.ProgramActionName AS ActionName
            FROM SysProgramActionsLink l
            INNER JOIN SysProgramActions a ON a.SysProgramActionId = l.ProgramActionId
            WHERE l.ProgramId = @ProgramId AND l.IsActive = 1;";

            using var connection = _context.CreateConnection();

            var dto = await connection.QueryAsync<SysProgramActionLinkLookUp>(
                new CommandDefinition(sql, new { ProgramId = programId },
                cancellationToken: cancellationToken));

            return dto.ToList();
        }




        //public async Task<bool> UpdateAsync
        //    (SysProgram entity,
        //    IDbConnection connection,
        //   IDbTransaction transaction, 
        //   CancellationToken cancellationToken = default)
        //{
        //    var sets = new List<string>();
        //    var parameters = new DynamicParameters();

        //    if (!string.IsNullOrWhiteSpace(entity.ProgramName))
        //    {
        //        sets.Add("ProgramName = @ProgramName");
        //        parameters.Add("ProgramName", entity.ProgramName);
        //    }

        //    if (!string.IsNullOrWhiteSpace(entity.LabelName))
        //    {
        //        sets.Add("LabelName = @LabelName");
        //        parameters.Add("LabelName", entity.LabelName);
        //    }

        //    if (!string.IsNullOrWhiteSpace(entity.ProgramRoute))
        //    {
        //        sets.Add("ProgramRoute = @ProgramRoute");
        //        parameters.Add("ProgramRoute", entity.ProgramRoute);
        //    }

        //    if (entity.ApplicationId > 0)
        //    {
        //        sets.Add("ApplicationId = @ApplicationId");
        //        parameters.Add("ApplicationId", entity.ApplicationId);
        //    }

        //    if (entity.CompanyId > 0)
        //    {
        //        sets.Add("CompanyId = @CompanyId");
        //        parameters.Add("CompanyId", entity.CompanyId);
        //    }

        //    if (entity.IsActive.HasValue)
        //    {
        //        sets.Add("IsActive = @IsActive");
        //        parameters.Add("IsActive", entity.IsActive.Value);
        //    }

        //    if (sets.Count == 0)
        //        return false;

        //    sets.Add("UpdatedBy = @UpdatedBy");
        //    sets.Add("UpdatedAt = SYSDATETIME()");
        //    parameters.Add("UpdatedBy", entity.UpdatedBy);
        //    parameters.Add("ProgramId", entity.SysProgramId);

        //    var sql = new StringBuilder();
        //    sql.Append("UPDATE SysProgram SET ");
        //    sql.Append(string.Join(", ", sets));
        //    sql.Append(" WHERE SysProgramId = @ProgramId AND IsDeleted = 0;");


        //    var affected = await connection.ExecuteAsync
        //        (new CommandDefinition(sql.ToString(),
        //        parameters,transaction,
        //        cancellationToken: cancellationToken));

        //    return affected > 0;
        //}


        public async Task<bool> UpdateAsync(
     SysProgram entity,
     IDbConnection connection,
     IDbTransaction transaction,
     CancellationToken cancellationToken = default)
        {
            var sets = new List<string>();
            var parameters = new DynamicParameters();

            // -------------------------
            // REQUIRED PARAMETERS
            // -------------------------
            parameters.Add("ProgramId", entity.SysProgramId);
            parameters.Add("UpdatedBy", entity.UpdatedBy);

            // -------------------------
            // OPTIONAL PARAMETERS (ONLY WHEN VALID)
            // -------------------------
            if (!string.IsNullOrWhiteSpace(entity.ProgramName))
            {
                parameters.Add("ProgramName", entity.ProgramName);
                sets.Add("ProgramName = @ProgramName");
            }

            if (!string.IsNullOrWhiteSpace(entity.LabelName))
            {
                parameters.Add("LabelName", entity.LabelName);
                sets.Add("LabelName = @LabelName");
            }

            if (!string.IsNullOrWhiteSpace(entity.ProgramRoute))
            {
                parameters.Add("ProgramRoute", entity.ProgramRoute);
                sets.Add("ProgramRoute = @ProgramRoute");
            }

            if (entity.ApplicationId > 0)
            {
                parameters.Add("ApplicationId", entity.ApplicationId);
                sets.Add("ApplicationId = @ApplicationId");
            }

            if (entity.CompanyId > 0)
            {
                parameters.Add("CompanyId", entity.CompanyId);
                sets.Add("CompanyId = @CompanyId");
            }

            if (entity.IsActive.HasValue)
            {
                parameters.Add("IsActive", entity.IsActive.Value);
                sets.Add("IsActive = @IsActive");
            }

            // Nothing to update
            if (sets.Count == 0)
                return false;

            // -------------------------
            // AUDIT FIELDS
            // -------------------------
            parameters.Add("UpdatedBy", entity.UpdatedBy);
            sets.Add("UpdatedBy = @UpdatedBy");
            sets.Add("UpdatedAt = SYSDATETIME()");

            // -------------------------
            // SQL
            // -------------------------
            var sql = $@"
UPDATE SysProgram
SET {string.Join(", ", sets)}
WHERE SysProgramId = @ProgramId
  AND IsDeleted = 0;
";

            var affected = await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    parameters,
                    transaction,
                    cancellationToken: cancellationToken
                ));

            return affected > 0;
        }




        public async Task<Dictionary<int, bool?>>
          GetExistingProgramActionsAsync(
          int programId,
          IDbConnection connection,
          IDbTransaction transaction,
          CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT ProgramActionId, IsActive
                FROM SysProgramActionsLink
                WHERE ProgramId = @ProgramId
                ";

            var rows = await connection.QueryAsync<SysProgramActionsLink>(
                new CommandDefinition(
                    sql,
                    new { ProgramId = programId },
                    transaction,
                    cancellationToken: cancellationToken
                ));

            // ActionId → IsActive (nullable)
            return rows.ToDictionary(
                x => x.ProgramActionId,
                x => x.IsActive
            );
        }






        //public async Task<bool> 
        //    UpsertProgramActionLinksAsync(
        //    int programId, 
        //    IDbConnection connection,
        //    IDbTransaction transaction,
        //    IEnumerable<SysProgramActionLinkUpdateDTO> actions,
        //    int userId, CancellationToken cancellationToken = default)
        //{
        //    const string selectSql = @"SELECT TOP 1 * FROM SysProgramActionsLink WHERE ProgramId = @ProgramId AND ProgramActionId = @ActionId";
        //    const string insertSql = @"INSERT INTO SysProgramActionsLink (ProgramId, ProgramActionId, IsActive, CreatedBy, CreatedAt) VALUES (@ProgramId, @ActionId, @IsActive, @CreatedBy, SYSDATETIME())";
        //    const string updateSql = @"UPDATE SysProgramActionsLink SET IsActive = @IsActive, UpdatedBy = @UpdatedBy, UpdatedAt = SYSDATETIME() WHERE ProgramId = @ProgramId AND ProgramActionId = @ActionId";



        //    foreach (var action in actions)
        //    {
        //        var existing = await connection.QueryFirstOrDefaultAsync
        //            <SysProgramActionsLink>(
        //            new CommandDefinition(selectSql,
        //            new { ProgramId = programId, 
        //             ActionId = action.ActionId },
        //            transaction: transaction, 
        //            cancellationToken: cancellationToken));

        //        if (existing == null)
        //        {
        //            await connection.ExecuteAsync(
        //                new CommandDefinition(insertSql,
        //                new { ProgramId = programId,
        //                ActionId = action.ActionId, 
        //                IsActive = action.Active, 
        //                CreatedBy = userId }, 
        //                transaction: transaction, 
        //                cancellationToken: cancellationToken));
        //        }
        //        else
        //        {
        //            await connection.ExecuteAsync(
        //                new CommandDefinition(updateSql,
        //                new { ProgramId = programId,
        //                ActionId = action.ActionId, 
        //                IsActive = action.Active, 
        //                UpdatedBy = userId }, 
        //                transaction: transaction, 
        //                cancellationToken: cancellationToken));
        //        }
        //    }             
        //}



      

        public async Task<SysProgram?>
            GetActiveByIdAsync(
            int id, 
            CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT * FROM SysProgram WHERE SysProgramId = @ProgramId And IsActive = 1 And IsDeleted = 0 ;";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SysProgram>(
                new CommandDefinition(sql, new { ProgramId = id }, cancellationToken: cancellationToken));
        }



        

        public async Task<List<SysProgramByApplicationIdDTO>>
            GetAllActiveByApplicationIdAsync(
            int applicationId,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
            SELECT
                p.SysProgramId AS ProgramId,
                p.ProgramName                
            FROM SysProgram p         
            WHERE p.ApplicationId = @ApplicationId AND p.IsActive = 1 AND p.IsDeleted = 0
            ORDER BY p.ProgramName ASC;";

            using var connection = _context.CreateConnection();
            var items = (await connection.QueryAsync<SysProgramByApplicationIdDTO>(
                new CommandDefinition(sql, new { ApplicationId = applicationId }, cancellationToken: cancellationToken))).ToList();

            foreach (var item in items)
            {
                var links = await GetLinksByProgramIdForApplicationIdAsync(item.ProgramId, cancellationToken);
                item.Action = links.ToList();
            }
              
           return items;
        }


        public async Task<IEnumerable<SysProgramActionLinkByApplicationIdDTO>>
          GetLinksByProgramIdForApplicationIdAsync(
          int programId,
          CancellationToken cancellationToken = default)
        {
            const string sql = @"
            SELECT 
                l.SysProgramActionLinkId AS ActionLinkId,
                a.ProgramActionName AS ActionName               
            FROM SysProgramActionsLink l
            INNER JOIN SysProgramActions a ON a.SysProgramActionId = l.ProgramActionId
        
            WHERE l.ProgramId = @ProgramId AND l.IsActive = 1;
            ";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<SysProgramActionLinkByApplicationIdDTO>(
                new CommandDefinition(sql, new { ProgramId = programId }, cancellationToken: cancellationToken));
        }



        public async Task<SysProgramActionsLink?>
            GetProgramActionsLinkActiveByIdAsync
          (int SysProgramActionsLink,
            CancellationToken cancellationToken = default)
        {
            var sql = "SELECT * FROM SysProgramActionsLink WHERE SysProgramActionLinkId = @SysProgramActionLinkId And IsActive = 1 ;";

            var cmd = new CommandDefinition(sql, new { SysProgramActionsLink = SysProgramActionsLink }, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            var entity = await connection.QueryFirstOrDefaultAsync<SysProgramActionsLink>(cmd);
            return entity;
        }


        public async Task<bool> 
            DeleteAsync(
            int id, 
            int deletedBy,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE SysProgram SET IsDeleted = 1, DeletedAt = SYSDATETIME(),DeletedBy = @deletedBy  WHERE SysProgramId = @ProgramId And IsDeleted = 0;";

            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(sql, new { ProgramId = id, deletedBy }, cancellationToken: cancellationToken));
            return affectedRows > 0;
        }


        public async Task<bool> 
            RecoverProgramAsync
            (int id, 
            int loginId,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE SysProgram SET IsDeleted = 0 ,DeletedBy = NULL, DeletedAt = NULL , UpdatedBy = @LoginId, UpdatedAt = SYSDATETIME() WHERE SysProgramId = @ProgramId And IsDeleted = 1;";
            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(sql, new { ProgramId = id, LoginId = loginId }, cancellationToken: cancellationToken));
            return affectedRows > 0;
        }

        


      

        public async Task<PagedResultDto<SysProgramDto>> 
            GetAllAsync(SysProgramFilterDTO filter,
            CancellationToken cancellationToken = default)
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

            var searchByMap = new Dictionary<SysProgramSearchBy, string>
            {
                [SysProgramSearchBy.ProgramName] = "p.ProgramName",
                [SysProgramSearchBy.Label] = "p.LabelName",
                [SysProgramSearchBy.Route] = "p.ProgramRoute",
                [SysProgramSearchBy.PlatformName] = "a.ApplicationName",
                [SysProgramSearchBy.CreatedUser] = "CONCAT(uC.FirstName,' ',uC.LastName)",
                [SysProgramSearchBy.ModifiedUser] = "CONCAT(uU.FirstName,' ',uU.LastName)",
                [SysProgramSearchBy.DeletedUser] = "CONCAT(uD.FirstName,' ',uD.LastName)"
            };

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                if (filter.SearchBy.HasValue && searchByMap.TryGetValue(filter.SearchBy.Value, out var col))
                {
                    baseSql.Append($" AND {col} LIKE @Search");
                }
                else
                {
            //        baseSql.Append(@"
            //AND (
            //    p.ProgramName LIKE @Search OR p.LabelName LIKE @Search OR p.ProgramRoute LIKE @Search
            // OR a.ApplicationName LIKE @Search OR c.CompanyName LIKE @Search
            // OR uC.FirstName LIKE @Search OR uC.LastName LIKE @Search
            // OR uU.FirstName LIKE @Search OR uU.LastName LIKE @Search
            // OR uD.FirstName LIKE @Search OR uD.LastName LIKE @Search
            //)");

                    baseSql.Append(@"
            AND (
                p.ProgramName LIKE @Search OR p.LabelName LIKE @Search OR p.ProgramRoute LIKE @Search
             OR a.ApplicationName LIKE @Search 
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

            var sortOrder = filter.SortOrder == SortOrder.Desc ? "DESC" : "ASC";
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
              
               p.IsActive Active,
               p.IsDeleted AS [Deleted],
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

            var items = (await connection.QueryAsync<SysProgramDto>(
                new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken))).ToList();

            foreach (var item in items)
            {
                var links = await GetLinksByProgramIdAsync(item.ProgramId, cancellationToken);
                item.Action = links.ToList();
            }

            return new PagedResultDto<SysProgramDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = limit
            };
        }


        //public async Task<IEnumerable<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramDto>> GetAllActiveByApplicationIdAsync(int applicationId, CancellationToken cancellationToken = default)
        //{
        //    const string sql = @"
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
        //    FROM SysProgram p
        //    LEFT JOIN Application a ON a.ApplicationId = p.ApplicationId
        //    LEFT JOIN Company c ON c.CompanyId = p.CompanyId
        //    INNER JOIN Users uC ON uC.UserId = p.CreatedBy
        //    LEFT JOIN Users uU ON uU.UserId = p.UpdatedBy
        //    LEFT JOIN Users uD ON uD.UserId = p.DeletedBy
        //    WHERE p.ApplicationId = @ApplicationId AND p.IsActive = 1 AND p.IsDeleted = 0
        //    ORDER BY p.ProgramName ASC;";

        //    using var connection = _context.CreateConnection();
        //    var items = (await connection.QueryAsync<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram.SysProgramDto>(
        //        new CommandDefinition(sql, new { ApplicationId = applicationId }, cancellationToken: cancellationToken))).ToList();

        //    foreach (var item in items)
        //    {
        //        var links = await GetLinksByProgramIdAsync(item.ProgramId, cancellationToken);
        //        item.Action = links.ToList();
        //    }

        //    return items;
        //}

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


    }
}
