using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class AuthRepo : IAuthRepo
    {
        private readonly IDapperContext _context;

        public AuthRepo(IDapperContext context)
        {
            _context = context;
        }

        public async Task<Users?> GetUserByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT UserId, FirstName, LastName, Email,
                       HashKey, SaltKey, IsDeleted, IsActive
                FROM dbo.Users
                WHERE Email = @Email AND IsDeleted = 0;
            ";

            using var connection = _context.CreateConnection();

            return await connection.QuerySingleOrDefaultAsync<Users>(
                new CommandDefinition(
                    sql,
                    new { Email = email },
                    cancellationToken: cancellationToken
                )
            );
        }

        public async Task<bool> UpdatePasswordAsync(
            int userId,
            byte[] hashKey,
            byte[] saltKey,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
                UPDATE dbo.Users
                SET HashKey   = @HashKey,
                    SaltKey   = @SaltKey,
                    UpdatedAt = SYSDATETIME()
                WHERE UserId = @UserId AND IsDeleted = 0;
            ";

            using var connection = _context.CreateConnection();

            var affected = await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    new { UserId = userId, HashKey = hashKey, SaltKey = saltKey },
                    cancellationToken: cancellationToken
                )
            );

            return affected > 0;
        }
    }
}
