using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.Application;
using VoiceFirst_Admin.Utilities.DTOs.Features.ApplicationVersion;
using VoiceFirst_Admin.Utilities.Enums;
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


        public async Task<Application>  IsIdExistAsync
         (int ApplicationId, CancellationToken cancellationToken = default)
        {
            var sql = "SELECT IsActive  FROM Application WHERE ApplicationId = @ApplicationId ;";

            var cmd = new CommandDefinition(sql, new { ApplicationId = ApplicationId }, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            var entity = await connection.QueryFirstOrDefaultAsync<Application>(cmd);
            return entity;
        }

        public async Task<PlatformVersionDto?>
            VersionExistsAsync(
            int applicationId,
            string version, 
            ClientType  type, 
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT ApplicationVersionId, IsDeleted AS Deleted
                FROM ApplicationVersion
                WHERE ApplicationId = @ApplicationId
                  AND Version = @Version
                  AND Type = @Type;";

            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<PlatformVersionDto?>(
                new CommandDefinition(sql, new 
                { ApplicationId = applicationId,
                    Version = version, 
                    Type = type },
                    cancellationToken: cancellationToken));
        }

        public async Task<int> CreateVersionAsync(ApplicationVersion entity, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                INSERT INTO ApplicationVersion (ApplicationId, Version, Type, CreatedBy)
                VALUES (@ApplicationId, @Version, @Type, @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, new { entity.ApplicationId, entity.Version, entity.Type, entity.CreatedBy }, cancellationToken: cancellationToken));
        }

        public async Task<PlatformVersionDto?> GetVersionByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT av.ApplicationVersionId, av.ApplicationId, a.ApplicationName,
                    av.Version, av.Type, av.IsActive AS Active, av.IsDeleted AS Deleted,
                    av.CreatedAt AS CreatedDate, av.UpdatedAt AS ModifiedDate, av.DeletedAt AS DeletedDate,
                    CONCAT(cu.FirstName,' ',ISNULL(cu.LastName,'')) AS CreatedUser,
                    CONCAT(uu.FirstName,' ',ISNULL(uu.LastName,'')) AS ModifiedUser,
                    CONCAT(du.FirstName,' ',ISNULL(du.LastName,'')) AS DeletedUser
                FROM dbo.ApplicationVersion av
                INNER JOIN dbo.Application a ON a.ApplicationId = av.ApplicationId
                INNER JOIN dbo.Users cu ON cu.UserId = av.CreatedBy
                LEFT JOIN dbo.Users uu ON uu.UserId = av.UpdatedBy
                LEFT JOIN dbo.Users du ON du.UserId = av.DeletedBy
                WHERE av.ApplicationVersionId = @Id;";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<PlatformVersionDto?>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }
    }
}
