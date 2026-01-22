using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.Application;
using VoiceFirst_Admin.Utilities.DTOs.Features.Plan;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class PlanRepo: IPlanRepo
    {
        private readonly IDapperContext _context;


        public PlanRepo(IDapperContext context)
        {
            _context = context;
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
