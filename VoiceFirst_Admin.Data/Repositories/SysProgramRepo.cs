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
            var sql = new StringBuilder("SELECT TOP 1 * FROM SysProgram WHERE ApplicationId = @ApplicationId AND ProgramName = @ActivityName");
            if (excludeId.HasValue)
                sql.Append(" AND ProgramId <> @ExcludeId");

            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SysProgram>(
                new CommandDefinition(sql.ToString(), new { ApplicationId = applicationId, Name = name, ExcludeId = excludeId }, cancellationToken: cancellationToken));
        }

        public async Task<SysProgram?> ExistsByLabelAsync(int applicationId, string label, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var sql = new StringBuilder("SELECT TOP 1 * FROM SysProgram WHERE ApplicationId = @ApplicationId AND LabelName = @Label");
            if (excludeId.HasValue)
                sql.Append(" AND ProgramId <> @ExcludeId");

            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SysProgram>(
                new CommandDefinition(sql.ToString(), new { ApplicationId = applicationId, Label = label, ExcludeId = excludeId }, cancellationToken: cancellationToken));
        }

        public async Task<SysProgram?> ExistsByRouteAsync(int applicationId, string route, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var sql = new StringBuilder("SELECT TOP 1 * FROM SysProgram WHERE ApplicationId = @ApplicationId AND ProgramRoute = @Route");
            if (excludeId.HasValue)
                sql.Append(" AND ProgramId <> @ExcludeId");

            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SysProgram>(
                new CommandDefinition(sql.ToString(), new { ApplicationId = applicationId, Route = route, ExcludeId = excludeId }, cancellationToken: cancellationToken));
        }

        public async Task<SysProgram?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT * FROM SysProgram WHERE ProgramId = @ActivityId";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SysProgram>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
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

//            const string selectLinks = @"
//SELECT l.SysProgramActionLinkId, l.ProgramId, l.ProgramActionId, l.Active, l.IsDeleted, l.CreatedAt, l.UpdatedAt
//FROM SysProgramActionsLink l WHERE l.ProgramId = @ProgramId AND l.IsDeleted = 0;";

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
                ISNULL(l.Active, 1) AS Active,
                CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
                l.CreatedAt AS CreatedDate,
                CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')) AS ModifiedUser,
                l.UpdatedAt AS ModifiedDate
            FROM SysProgramActionsLink l
            INNER JOIN SysProgramActions a ON a.SysProgramActionId = l.ProgramActionId
            INNER JOIN Users uC ON uC.UserId = l.CreatedBy
            LEFT JOIN Users uU ON uU.UserId = l.UpdatedBy
            WHERE l.ProgramId = @ProgramId AND l.Active = 1;
            ";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink.SysProgramActionLinkDTO>(
                new CommandDefinition(sql, new { ProgramId = programId }, cancellationToken: cancellationToken));
        }




        public async Task<SysProgramActionsLink> GetActiveByIdAsync
          (int SysProgramActionsLink, CancellationToken cancellationToken = default)
        {
            var sql = "SELECT * FROM SysProgramActionsLink WHERE SysProgramActionLinkId = @SysProgramActionLinkId And Active = 1 ;";

            var cmd = new CommandDefinition(sql, new { SysProgramActionsLink = SysProgramActionsLink }, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            var entity = await connection.QueryFirstOrDefaultAsync<SysProgramActionsLink>(cmd);
            return entity;
        }
    }
}
