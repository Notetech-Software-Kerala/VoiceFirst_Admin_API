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

        public async Task<Users?> GetUserForLoginAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT UserId, FirstName, LastName, Email,
                       HashKey, SaltKey, IsDeleted, IsActive
                FROM dbo.Users
                WHERE Email = @Email;
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

        public async Task<int> UpsertDeviceAsync(
            UserDevice device,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
                MERGE dbo.UserDevice AS target
                USING (SELECT @DeviceID AS DeviceID, @ApplicationVersionId AS ApplicationVersionId) AS source
                ON target.DeviceID = source.DeviceID AND target.ApplicationVersionId = source.ApplicationVersionId
                WHEN MATCHED THEN
                    UPDATE SET
                        DeviceName    = @DeviceName,
                        DeviceType    = @DeviceType,
                        OS            = @OS,
                        OSVersion     = @OSVersion,
                        Manufacturer  = @Manufacturer,
                        Model         = @Model,
                        UpdatedAt     = SYSDATETIME()
                WHEN NOT MATCHED THEN
                    INSERT (DeviceID, ApplicationVersionId, DeviceName, DeviceType, OS, OSVersion, Manufacturer, Model, CreatedAt, IsDeleted)
                    VALUES (@DeviceID, @ApplicationVersionId, @DeviceName, @DeviceType, @OS, @OSVersion, @Manufacturer, @Model, SYSDATETIME(), 0)
                OUTPUT inserted.UserDeviceId;
            ";

            using var connection = _context.CreateConnection();

            return await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    sql,
                    new
                    {
                        device.DeviceID,
                        device.ApplicationVersionId,
                        device.DeviceName,
                        device.DeviceType,
                        device.OS,
                        device.OSVersion,
                        device.Manufacturer,
                        device.Model
                    },
                    cancellationToken: cancellationToken
                )
            );
        }

        public async Task<int> CreateSessionAsync(
            int userId,
            int userDeviceId,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
                -- Deactivate previous sessions for this user on this device
                UPDATE dbo.UserDeviceLogin
                SET IsCurrentSession = 0, UpdatedAt = SYSDATETIME()
                WHERE UserId = @UserId AND UserDeviceId = @UserDeviceId AND IsCurrentSession = 1;

                -- Create new session
                INSERT INTO dbo.UserDeviceLogin (UserId, UserDeviceId, CreatedAt, IsCurrentSession)
                VALUES (@UserId, @UserDeviceId, SYSDATETIME(), 1);

                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            using var connection = _context.CreateConnection();

            return await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    sql,
                    new { UserId = userId, UserDeviceId = userDeviceId },
                    cancellationToken: cancellationToken
                )
            );
        }

        public async Task InvalidateSessionAsync(
            int userDeviceLoginId,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
                UPDATE dbo.UserDeviceLogin
                SET IsCurrentSession = 0, UpdatedAt = SYSDATETIME()
                WHERE UserDeviceLoginId = @UserDeviceLoginId;
            ";

            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    new { UserDeviceLoginId = userDeviceLoginId },
                    cancellationToken: cancellationToken
                )
            );
        }

        public async Task InvalidateAllSessionsAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
                UPDATE dbo.UserDeviceLogin
                SET IsCurrentSession = 0, UpdatedAt = SYSDATETIME()
                WHERE UserId = @UserId AND IsCurrentSession = 1;
            ";

            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    new { UserId = userId },
                    cancellationToken: cancellationToken
                )
            );
        }

        public async Task<int?> GetApplicationVersionIdAsync(
            int version,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT ApplicationVersionId
                FROM dbo.ApplicationVersion
                WHERE ApplicationVersionId  = @ApplicationVersionId  AND IsActive  = 1;
            ";

            using var connection = _context.CreateConnection();

            return await connection.QuerySingleOrDefaultAsync<int?>(
                new CommandDefinition(
                    sql,
                    new { ApplicationVersionId = version },
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
