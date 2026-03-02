using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.Application;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class ApplicationRepository: IApplicationRepository
    {
        private readonly IDapperContext _context;

 
        public ApplicationRepository(IDapperContext context)
        {
            _context = context;
        }

       

       

        public async Task<IEnumerable<PlatformLookupDto>>
        GetActiveApplicationsAsync(CancellationToken cancellationToken = default)
        {
            const string sql = @"
        SELECT 
            ApplicationId   AS PlatformId,
            ApplicationName AS PlatformName
        FROM Application
        WHERE IsActive = 1
          AND ApplicationId <> 1
        ORDER BY ApplicationName ASC;";

            using var connection = _context.CreateConnection();

            var command = new CommandDefinition(
                sql,
                cancellationToken: cancellationToken);

            var result = await connection
                .QueryAsync<PlatformLookupDto>(command);

            return result;
        }


        public async Task<Application> IsIdExistAsync
         (int ApplicationId, CancellationToken cancellationToken = default)
        {
            var sql = "SELECT IsActive  FROM Application WHERE ApplicationId = @ApplicationId ;";

            var cmd = new CommandDefinition(sql, new { ApplicationId = ApplicationId }, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            var entity = await connection.QueryFirstOrDefaultAsync<Application>(cmd);
            return entity;
        }
    }
}
