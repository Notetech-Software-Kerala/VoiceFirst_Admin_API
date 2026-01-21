using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class ApplicationRepo: IApplicationRepo
    {
        private readonly IDapperContext _context;

 
        public ApplicationRepo(IDapperContext context)
        {
            _context = context;
        }

        public async Task<Application> GetActiveByIdAsync
            ( int ApplicationId, CancellationToken cancellationToken = default)
        {
            var sql = "SELECT * FROM Application WHERE ApplicationId = @ApplicationId And Active = 1;";
 
            var cmd = new CommandDefinition(sql, new { ApplicationId = ApplicationId }, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            var entity = await connection.QueryFirstOrDefaultAsync<Application>(cmd);
            return entity;
        }

    }
}
