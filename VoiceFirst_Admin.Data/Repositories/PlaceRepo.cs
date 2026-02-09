using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.Place;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class PlaceRepo: IPlaceRepo
    {
        private readonly IDapperContext _context;






        public PlaceRepo(IDapperContext context)
        {
            _context = context;
        }


        public async Task<PlaceDTO> PlaceExistsAsync
            (string name, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var sql = "SELECT IsDeleted As Deleted,PlaceId As PlaceId  FROM Place WHERE PlaceName = @PlaceName";
            if (excludeId.HasValue)
                sql += " AND PlaceId <> @ExcludeId";

            var cmd = new CommandDefinition(sql, new { PlaceName = name, ExcludeId = excludeId }, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            var entity = await connection.QueryFirstOrDefaultAsync<PlaceDTO>(cmd);
            return entity;
        }

        public async Task<bool> BulkInsertPlaceZipCodeLinksAsync(
           int placeId,
           IEnumerable<int> zipCodeLinkIds,
           int createdBy,
           IDbConnection connection,
           IDbTransaction tx,
           CancellationToken cancellationToken)
        {
            if (zipCodeLinkIds == null || !zipCodeLinkIds.Any())
                return false;

            // -------------------------
            // CONVERT dynamic → int
            // -------------------------

            if (zipCodeLinkIds.Count() == 0)
                return false;

            // -------------------------
            // SQL
            // -------------------------
            const string sql = @"
        INSERT INTO PlaceZipCodeLink
            (PlaceId, PostOfficeZipCodeLinkId, CreatedBy)
        VALUES
            (@PlaceId, @PostOfficeZipCodeLinkId, @CreatedBy);";

            // -------------------------
            // PARAMETER OBJECTS
            // -------------------------
            var parameters = zipCodeLinkIds.Select(id => new
            {
                PlaceId = placeId,
                PostOfficeZipCodeLinkId = id,
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



        public async Task<bool> BulkUpdatePlaceZipCodeLinksAsync(
         int placeId,
         IEnumerable<PlaceZipCodeLinkUpdateDTO> dtos,
         int updatedBy,
         IDbConnection connection,
         IDbTransaction tx,
         CancellationToken cancellationToken)
        {
            const string sql = @"
            UPDATE PlaceZipCodeLink
            SET IsActive   = @IsActive,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = SYSDATETIME()
            WHERE PlaceId = @PlaceId
              AND PostOfficeZipCodeLinkId = @PostOfficeZipCodeLinkId
              AND IsActive <> @IsActive;
            ";

            var parameters = dtos.Select(dto => new
            {
                PlaceId = placeId,
                PostOfficeZipCodeLinkId = dto.ZipCodeLinkId,
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












        public async Task<int> CreateAsync(
    Place entity,
    IDbConnection connection,
    IDbTransaction transaction,
    CancellationToken cancellationToken = default)
        {
            const string sql = @"
        INSERT INTO Place (PlaceName, CreatedBy)
        VALUES (@PlaceName, @CreatedBy);
        SELECT CAST(SCOPE_IDENTITY() AS int);";

            var cmd = new CommandDefinition(
                sql,
                new
                {
                    entity.PlaceName,
                    entity.CreatedBy
                },
                transaction: transaction, // ✅ THIS is the key
                cancellationToken: cancellationToken
            );

            int id = await connection.ExecuteScalarAsync<int>(cmd);
            return id;
        }



        public async Task<PlaceDetailDTO?> GetByIdAsync(
          int PlaceId,         
          IDbConnection connection,
          IDbTransaction transaction,
          CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT 
                    s.PlaceId   ,
                    s.PlaceName     ,
                    s.IsActive             As Active        ,
                    s.IsDeleted            As Deleted      ,
                    s.CreatedAt             As CreatedDate    ,
                    s.UpdatedAt             As   ModifiedDate  ,
                    s.DeletedAt              As   DeletedDate  ,

                    -- Created User
                    CONCAT(cu.FirstName, ' ', ISNULL(cu.LastName, '')) AS CreatedUser,

                    -- Updated User
                    CONCAT(uu.FirstName, ' ', ISNULL(uu.LastName, '')) AS ModifiedUser,

                    -- Deleted User
                    CONCAT(du.FirstName, ' ', ISNULL(du.LastName, '')) AS DeletedUser

                FROM dbo.Place s
                INNER JOIN dbo.Users cu ON cu.UserId = s.CreatedBy
                LEFT JOIN dbo.Users uu ON uu.UserId = s.UpdatedBy
                LEFT JOIN dbo.Users du ON du.UserId = s.DeletedBy
                WHERE s.PlaceId = @PlaceId;
                ";

            var entity = await connection.QuerySingleOrDefaultAsync<PlaceDetailDTO>(
                new CommandDefinition(sql, 
                new { PlaceId = PlaceId },
                transaction, 
                cancellationToken: cancellationToken)
            );
            if(entity == null)
                return null;
            entity.PostOffices = (await GetPlaceZipCodeLinksByPlaceIdAsync(PlaceId, connection, transaction, cancellationToken)).ToList();
            return entity;
        }



        public async Task<IEnumerable<PlaceZipCodeLinkDetailDTO>>
            GetPlaceZipCodeLinksByPlaceIdAsync(int placeId, IDbConnection connection,
            IDbTransaction transaction, CancellationToken cancellationToken = default)
        {
           

            var sql = @"SELECT 
                        po.PostOfficeId,
                        po.PostOfficeName,
                        pzl.PlaceZipCodeLinkId, -- split starts here
                        pzl.PostOfficeZipCodeLinkId AS ZipCodeLinkId, 
                        z.ZipCode,
                        pzl.IsActive AS Active,
                        CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
                        pzl.CreatedAt AS CreatedDate,

                        CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')) AS ModifiedUser,
                        pzl.UpdatedAt AS ModifiedDate
                        FROM PlaceZipCodeLink pzl
                        INNER JOIN PostOfficeZipCodeLink pozl
                            ON pozl.PostOfficeZipCodeLinkId = pzl.PostOfficeZipCodeLinkId
                            AND pozl.IsActive = 1
                        INNER JOIN PostOffice po
                            ON po.PostOfficeId = pozl.PostOfficeId
                            AND po.IsDeleted = 0
                            AND po.IsActive = 1
                        INNER JOIN ZipCode z
                            ON z.ZipCodeId = pozl.ZipCodeId
                        INNER JOIN Users uC
                            ON uC.UserId = pzl.CreatedBy
                        LEFT JOIN Users uU
                            ON uU.UserId = pzl.UpdatedBy
                        WHERE pzl.PlaceId = @PlaceId
                        ORDER BY po.PostOfficeName, z.ZipCode;";


            var lookup = new Dictionary<int, PlaceZipCodeLinkDetailDTO>();

            var result = await connection.QueryAsync<
               PlaceZipCodeLinkDetailDTO,
               PlaceZipCodeLinkDTO,
               PlaceZipCodeLinkDetailDTO>(
               sql,
               (postOffice, zip) =>
               {
                   if (!lookup.TryGetValue(postOffice.PostOfficeId, out var dto))
                   {
                       dto = postOffice;
                       dto.ZipCodes = new List<PlaceZipCodeLinkDTO>();
                       lookup.Add(dto.PostOfficeId, dto);
                   }

                   dto.ZipCodes.Add(zip);
                   return dto;
               },
               param: new { placeId },
               transaction: transaction,
               splitOn: "PlaceZipCodeLinkId"
           );


            return lookup.Values;
        }




        public async Task<PagedResultDto<PlaceDTO>>
        GetAllAsync(PlaceFilterDTO filter, CancellationToken cancellationToken = default)
        {
            var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var limit = filter.Limit <= 0 ? 10 : filter.Limit;
            var offset = (page - 1) * limit;

            var parameters = new DynamicParameters();
            parameters.Add("Offset", offset);
            parameters.Add("Limit", limit);

            var baseSql = new StringBuilder(@"
            FROM Place spa
            INNER JOIN Users uC ON uC.UserId = spa.CreatedBy
            LEFT JOIN Users uU ON uU.UserId = spa.UpdatedBy
            LEFT JOIN Users uD ON uD.UserId = spa.DeletedBy WHERE 1=1
            ");

            if (filter.Deleted.HasValue)
            {
                baseSql.Append(" AND spa.IsDeleted = @IsDeleted");
                parameters.Add("IsDeleted", filter.Deleted.Value);
            }

            if (filter.Active.HasValue)
            {
                baseSql.Append(" AND spa.IsActive = @IsActive");
                parameters.Add("IsActive", filter.Active.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.CreatedFromDate) &&
                DateTime.TryParse(filter.CreatedFromDate, out var createdFrom))
            {
                baseSql.Append(" AND spa.CreatedAt >= @CreatedFrom");
                parameters.Add("CreatedFrom", createdFrom);
            }

            if (!string.IsNullOrWhiteSpace(filter.CreatedToDate) &&
                DateTime.TryParse(filter.CreatedToDate, out var createdTo))
            {
                baseSql.Append(" AND spa.CreatedAt < DATEADD(day, 1, @CreatedTo)");
                parameters.Add("CreatedTo", createdTo.Date);
            }

            if (!string.IsNullOrWhiteSpace(filter.UpdatedFromDate) &&
                DateTime.TryParse(filter.UpdatedFromDate, out var updatedFrom))
            {
                baseSql.Append(" AND spa.UpdatedAt >= @UpdatedFrom");
                parameters.Add("UpdatedFrom", updatedFrom);
            }

            if (!string.IsNullOrWhiteSpace(filter.UpdatedToDate) &&
                DateTime.TryParse(filter.UpdatedToDate, out var updatedTo))
            {
                baseSql.Append(" AND spa.UpdatedAt < DATEADD(day, 1, @UpdatedTo)");
                parameters.Add("UpdatedTo", updatedTo.Date);
            }

            if (!string.IsNullOrWhiteSpace(filter.DeletedFromDate) &&
                DateTime.TryParse(filter.DeletedFromDate, out var deletedFrom))
            {
                baseSql.Append(" AND spa.DeletedAt >= @DeletedFrom");
                parameters.Add("DeletedFrom", deletedFrom);
            }

            if (!string.IsNullOrWhiteSpace(filter.DeletedToDate) &&
                DateTime.TryParse(filter.DeletedToDate, out var deletedTo))
            {
                baseSql.Append(" AND spa.DeletedAt < DATEADD(day, 1, @DeletedTo)");
                parameters.Add("DeletedTo", deletedTo.Date);
            }

            var searchByMap = new Dictionary<PlaceSearchBy, string>
            {
                [PlaceSearchBy.PlaceName] = "spa.PlaceName",
                [PlaceSearchBy.CreatedUser] = "CONCAT(uC.FirstName,' ',uC.LastName)",
                [PlaceSearchBy.ModifiedUser] = "CONCAT(uU.FirstName,' ',uU.LastName)",
                [PlaceSearchBy.DeletedUser] = "CONCAT(uD.FirstName,' ',uD.LastName)"
            };

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                if (filter.SearchBy.HasValue && searchByMap.TryGetValue(filter.SearchBy.Value, out var col))
                    baseSql.Append($" AND {col} LIKE @Search");
                else
                    baseSql.Append(@"
                    AND (
                        spa.PlaceName LIKE @Search
                     OR uC.FirstName LIKE @Search OR uC.LastName LIKE @Search
                     OR uU.FirstName LIKE @Search OR uU.LastName LIKE @Search
                     OR uD.FirstName LIKE @Search OR uD.LastName LIKE @Search
                    )");

                parameters.Add("Search", $"%{filter.SearchText}%");
            }

            var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["PlaceId"] = "spa.PlaceId",
                ["PlaceName"] = "spa.PlaceName",
                ["Active"] = "spa.IsActive",
                ["Deleted"] = "spa.IsDeleted",
                ["CreatedDate"] = "spa.CreatedAt",
                ["ModifiedDate"] = "spa.UpdatedAt",
                ["DeletedDate"] = "spa.DeletedAt",
            };

            var sortOrder = filter.SortOrder == SortOrder.Desc ? "DESC" : "ASC";

            // 🔹 CHANGED: Default sorting is now PlaceName (Alphabetical order)
            var sortKey = string.IsNullOrWhiteSpace(filter.SortBy)
                ? "PlaceName"
                : filter.SortBy;

            if (!sortMap.TryGetValue(sortKey, out var sortColumn))
                sortColumn = sortMap["PlaceId"];

            var countSql = "SELECT COUNT(1) " + baseSql;

            var itemsSql = $@"
    SELECT
        spa.PlaceId,
        spa.PlaceName,
        spa.CreatedAt,
        spa.IsActive,
        spa.UpdatedAt,
        spa.IsDeleted,
        spa.DeletedAt,
        uC.UserId AS CreatedById,
        CONCAT(uC.FirstName, ' ', uC.LastName) AS CreatedUserName,
        uU.UserId AS UpdatedById,
        CONCAT(uU.FirstName, ' ', uU.LastName) AS UpdatedUserName,
        uD.UserId AS DeletedById,
        CONCAT(uD.FirstName, ' ', uD.LastName) AS DeletedUserName,
        spa.PlaceId AS PlaceId,
        spa.PlaceName AS PlaceName,
        spa.IsActive AS Active,
        spa.IsDeleted AS Deleted,
        spa.CreatedAt AS CreatedDate,
        spa.UpdatedAt AS ModifiedDate,
        spa.DeletedAt AS DeletedDate,
        CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
        ISNULL(CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')), '') AS ModifiedUser,
        ISNULL(CONCAT(uD.FirstName, ' ', ISNULL(uD.LastName, '')), '') AS DeletedUser
    {baseSql}
    ORDER BY {sortColumn} {sortOrder}
    OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;
    ";

            using var connection = _context.CreateConnection();

            var totalCount = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

            var items = await connection.QueryAsync<PlaceDTO>(
                new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken));

            return new PagedResultDto<PlaceDTO>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = limit
            };
        }




        public async Task<PlaceDTO> IsIdExistAsync(
         int PlaceId,
         CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT  s.PlaceId    As PlaceId ,
                        s.IsDeleted            As Deleted      
                FROM dbo.Place s
                WHERE PlaceId = @PlaceId
                 ;
                 ";

            using var connection = _context.CreateConnection();

            var dto = await connection.QuerySingleOrDefaultAsync<PlaceDTO>(
                new CommandDefinition(
                    sql,
                    new { PlaceId = PlaceId },
                    cancellationToken: cancellationToken
                )
            );
            return dto;
        }



    


        public async Task<List<PlaceLookUpDTO?>> GetActiveAsync(
         CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT 
                PlaceId As PlaceId,
                PlaceName  As PlaceName     
                FROM dbo.Place 
                WHERE  isDeleted = 0 And isActive = 1 ORDER BY PlaceName ASC;
                ";

            using var connection = _context.CreateConnection();
            var entity = await connection.QueryAsync<PlaceLookUpDTO?>(
                new CommandDefinition(sql, cancellationToken: cancellationToken)
            );
            return entity.ToList();
        }



        public async Task<bool> UpdateAsync(
           Place entity, IDbConnection connection,
            IDbTransaction transaction,
           CancellationToken cancellationToken = default)
        {
            var sets = new List<string>();
            var parameters = new DynamicParameters();

            // Always add parameters (nullable-safe)
            parameters.Add("PlaceName", entity.PlaceName);
            parameters.Add("Active", entity.IsActive.HasValue
                ? (entity.IsActive.Value ? 1 : 0)
                : (int?)null);

            if (!string.IsNullOrWhiteSpace(entity.PlaceName))
                sets.Add("PlaceName = @PlaceName");

            if (entity.IsActive.HasValue)
                sets.Add("IsActive = @Active");

            if (sets.Count == 0)
                return false;

            // Audit fields (only when real change occurs)
            sets.Add("UpdatedBy = @UpdatedBy");
            sets.Add("UpdatedAt = SYSDATETIME()");

            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("PlaceId", entity.PlaceId);

            var sql = $@"
                UPDATE Place
                SET {string.Join(", ", sets)}
                WHERE PlaceId = @PlaceId
                  AND IsDeleted = 0
                  AND (
                        (@PlaceName IS NOT NULL 
                            AND PlaceName <> @PlaceName)
                     OR (@Active IS NOT NULL 
                            AND IsActive <> @Active)
                  );";


            var affected = await connection.ExecuteAsync(
                new CommandDefinition(sql, parameters,transaction, cancellationToken: cancellationToken));

            return affected > 0;
        }






        public async Task<bool>
            CheckPlaceZipCodeLinksExistAsync(
                        int placeId,
                IEnumerable<int> PostOfficeZipCodeLinkId,
                bool update,
                IDbConnection connection,
                IDbTransaction transaction,
                CancellationToken cancellationToken = default)
        {
            // If no IDs are sent, treat as invalid
            if (PostOfficeZipCodeLinkId == null || !PostOfficeZipCodeLinkId.Any())
                return false;

            const string sql = @"
                    SELECT COUNT(1)
                    FROM PlaceZipCodeLink
                    WHERE PostOfficeZipCodeLinkId IN @PostOfficeZipCodeLinkId And PlaceId = @placeId
                ";
            var exists = await connection.ExecuteScalarAsync<int>(
               new CommandDefinition(
                   sql,
                   new { placeId, PostOfficeZipCodeLinkId },
                   transaction,
                   cancellationToken: cancellationToken
               ));

            if (!update)
            {
                // INSERT case
                // true  → already exists (block insert)
                // false → safe to insert
                return exists > 0;
            }

            // UPDATE case
            // true  → all records exist
            // false → some records missing
            return exists == PostOfficeZipCodeLinkId.Count();



        }



        public async Task<bool> CheckAlreadyPlaceZipCodeLinkedAsync(
    int placeId,
    IEnumerable<int> postOfficeZipCodeLinkId,
    IDbConnection connection,
    IDbTransaction transaction,
    CancellationToken cancellationToken = default)
        {
            if (postOfficeZipCodeLinkId == null || !postOfficeZipCodeLinkId.Any())
                return false;

            const string sql = @"
        SELECT 1
        FROM PlaceZipCodeLink
        WHERE PlaceId = @placeId
          AND PostOfficeZipCodeLinkId IN @PostOfficeZipCodeLinkId
    ";

            var exists = await connection.ExecuteScalarAsync<int?>(
                new CommandDefinition(
                    sql,
                    new { placeId, postOfficeZipCodeLinkId },
                    transaction,
                    cancellationToken: cancellationToken
                ));

            // true  → at least one already exists
            // false → safe to insert
            return exists.HasValue;
        }



        public async Task<bool> DeleteAsync
            (int id, int deletedBy,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE Place SET IsDeleted = 1, DeletedAt = SYSDATETIME(),DeletedBy = @deletedBy  WHERE PlaceId = @PlaceId And IsDeleted = 0;";

            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(sql, new { PlaceId = id, deletedBy }, cancellationToken: cancellationToken));
            return affectedRows > 0;
        }




        public async Task<bool> RecoverAsync
            (int id, int loginId, 
            CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE Place SET IsDeleted = 0 ,DeletedBy = NULL, DeletedAt = NULL , UpdatedBy = @LoginId, UpdatedAt = SYSDATETIME() WHERE PlaceId = @PlaceId And IsDeleted = 1";
            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(sql, new { PlaceId = id, LoginId = loginId }, cancellationToken: cancellationToken));
            return affectedRows > 0;
        }





    }
}
