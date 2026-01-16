using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class SysBusinessActivityRepo : ISysBusinessActivityRepo
    {
        private readonly IDapperContext _context;

        public SysBusinessActivityRepo(IDapperContext context)
        {
            _context = context;
        }

        public async Task<SysBusinessActivity> CreateAsync(SysBusinessActivity entity, CancellationToken cancellationToken = default)
        {
            const string sql = @"INSERT INTO SysBusinessActivity (BusinessActivityName,CreatedBy)
                                 VALUES (@BusinessActivityName,@CreatedBy);
                                 SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                // IDbConnection only supports synchronous Open
                connection.Open();
            }

            var newId = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, entity, cancellationToken: cancellationToken));

            entity.SysBusinessActivityId = newId;
            return entity;
        }

        public async Task<SysBusinessActivity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT * FROM SysBusinessActivity WHERE SysBusinessActivityId = @Id";

            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            return await connection.QuerySingleOrDefaultAsync<SysBusinessActivity>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<SysBusinessActivity>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT * FROM SysBusinessActivity
                                 ORDER BY SysBusinessActivityId
                                 OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (filter.PageNumber - 1) * filter.Limit, // <-- use PageNumber
                PageSize = filter.Limit
            };

            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            return await connection.QueryAsync<SysBusinessActivity>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        }

        public async Task<bool> UpdateAsync(SysBusinessActivity entity, CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE SysBusinessActivity
                                 SET BusinessActivityName = @BusinessActivityName,
                                     IsActive = @IsActive
                                 WHERE SysBusinessActivityId = @SysBusinessActivityId";

            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(sql, entity, cancellationToken: cancellationToken));
            return affectedRows > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE SysBusinessActivity SET IsActive = 0 WHERE SysBusinessActivityId = @Id";

            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
            return affectedRows > 0;
        }
    }
}
