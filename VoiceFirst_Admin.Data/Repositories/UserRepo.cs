using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using VoiceFirst_Admin.Data.Context;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;
using VoiceFirst_Admin.Utilities.DTOs.Features.UserRoleLink;
using VoiceFirst_Admin.Utilities.DTOs.Features.Users;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class UserRepo:IUserRepo
    {
        private readonly IDapperContext _context;
        private readonly IUserRoleLinkRepo _userRoleLinkRepo;

        public UserRepo(IDapperContext context, IUserRoleLinkRepo userRoleLinkRepo)
        {
            _context = context;
            _userRoleLinkRepo = userRoleLinkRepo;
        }




        public async Task<EmployeeDto> IsIdExistAsync(
         int userId,
         CancellationToken cancellationToken = default)
        {
            const string sql = @"
                    SELECT  s.UserId   AS EmployeeId,
                            s.IsDeleted      AS Deleted,
                            s.IsActive       AS Active,
                            s.MobileCountryId  AS MobileCountryCodeId
                    FROM dbo.Users s
                    WHERE s.UserId = @UserId;
                ";


            using var connection = _context.CreateConnection();

            var dto = await connection.QuerySingleOrDefaultAsync<EmployeeDto>(
                new CommandDefinition(
                    sql,
                    new { UserId = userId },
                    cancellationToken: cancellationToken
                )
            );
            return dto;
        }

      
        public async Task<bool>
            DeleteAsync(
            int id,
            int deletedBy,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE Users SET IsDeleted = 1, DeletedAt = SYSDATETIME(),DeletedBy = @deletedBy  WHERE UserId = @UserId And IsDeleted = 0;";

            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(sql, new { UserId = id, deletedBy }, cancellationToken: cancellationToken));
            return affectedRows > 0;
        }


        public async Task<bool>
            RecoverAsync
            (int id,
            int loginId,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"UPDATE Users SET IsDeleted = 0 ,DeletedBy = NULL, DeletedAt = NULL , UpdatedBy = @LoginId, UpdatedAt = SYSDATETIME() WHERE UserId = @UserId And IsDeleted = 1;";
            using var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(sql, new { UserId = id, LoginId = loginId }, cancellationToken: cancellationToken));
            return affectedRows > 0;
        }



        public async Task<EmployeeDetailDto?> GetByIdAsync
          (int id, IDbConnection connection, 
            IDbTransaction transaction, 
            CancellationToken cancellationToken = default)
        {


            const string sql = @"
        SELECT 
            P.UserId AS EmployeeId,
            P.FirstName,
            P.LastName,
            P.BirthYear,
            P.Gender,
            P.LinkedinId,
            P.FacebookId,
            P.GoogleId,
            P.Email,
            C.CountryId AS MobileCountryCodeId,
            ISNULL(c.CountryDialCode ,'') AS MobileCountryCode,
            P.MobileNo,
            P.BirthYear,                
            p.IsActive AS Active,
            p.IsDeleted AS [Deleted],
            CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
            p.CreatedAt AS CreatedDate,
            ISNULL(CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')), '') AS ModifiedUser,
            p.UpdatedAt AS ModifiedDate,
            ISNULL(CONCAT(uD.FirstName, ' ', ISNULL(uD.LastName, '')), '') AS DeletedUser,
            p.DeletedAt AS DeletedDate
        FROM Users p
        LEFT JOIN Country c ON c.CountryId = p.MobileCountryId     
        LEFT JOIN Users uC ON uC.UserId = p.CreatedBy
        LEFT JOIN Users uU ON uU.UserId = p.UpdatedBy
        LEFT JOIN Users uD ON uD.UserId = p.DeletedBy
        WHERE p.UserId = @UserId;";


            var dto = await connection.QueryFirstOrDefaultAsync<EmployeeDetailDto>(
                new CommandDefinition(sql, new { UserId = id }, transaction, cancellationToken: cancellationToken));
            if (dto == null) return null;

            var links = await _userRoleLinkRepo.GetRoleLinksByUserIdAsync(id, connection, transaction, cancellationToken);
            dto.Roles = links.ToList();
            return dto;
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



        public async Task<int> CreateAsync(
       Users user,
       IDbConnection connection,
       IDbTransaction transaction,
       CancellationToken cancellationToken)
        {
            const string sql = @"
        SET NOCOUNT ON;

        INSERT INTO dbo.Users
        (
            FirstName,
            LastName,
            Gender,
            Email,
            MobileCountryId,
            MobileNo,
            HashKey,
            SaltKey,
            BirthYear,
            CreatedBy
        )
        VALUES
        (
            @FirstName,
            @LastName,
            @Gender,
            @Email,
            @MobileCountryId,
            @MobileNo,
            @HashKey,
            @SaltKey,
            @BirthYear,
            @CreatedBy
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
    ";

            var command = new CommandDefinition(
                sql,
                parameters: user,
                transaction: transaction,
                cancellationToken: cancellationToken);

            var lastInsertId = await connection.ExecuteScalarAsync<int>(command);

            return lastInsertId;
        }




    



        public async Task<bool> UpdateAsync(
         Users entity,
         IDbConnection connection,
         IDbTransaction transaction,
         CancellationToken cancellationToken = default)
        {
            var sets = new List<string>();
            var parameters = new DynamicParameters();

            // -------------------------
            // REQUIRED PARAMETERS
            // -------------------------
            parameters.Add("UserId", entity.UserId);
            parameters.Add("UpdatedBy", entity.UpdatedBy);

            // -------------------------
            // OPTIONAL PARAMETERS (ONLY WHEN VALID)
            // -------------------------
            if (!string.IsNullOrWhiteSpace(entity.FirstName))
            {
                parameters.Add("FirstName", entity.FirstName);
                sets.Add("FirstName = @FirstName");
            }

            if (!string.IsNullOrWhiteSpace(entity.LastName))
            {
                parameters.Add("LastName", entity.LastName);
                sets.Add("LastName = @LastName");
            }

            if (!string.IsNullOrWhiteSpace(entity.Email))
            {
                parameters.Add("Email", entity.Email);
                sets.Add("Email = @Email");
            }
            if (!string.IsNullOrWhiteSpace(entity.MobileNo))
            {
                parameters.Add("MobileNo", entity.MobileNo);
                sets.Add("MobileNo = @MobileNo");
            }
            if (!string.IsNullOrWhiteSpace(entity.Gender))
            {
                parameters.Add("Gender", entity.Gender);
                sets.Add("Gender = @Gender");
            }

            if (entity.BirthYear.HasValue)
            {
                parameters.Add("BirthYear", entity.BirthYear);
                sets.Add("BirthYear = @BirthYear");
            }

            if (entity.MobileCountryId > 0)
            {
                parameters.Add("MobileCountryId", entity.MobileCountryId);
                sets.Add("MobileCountryId = @MobileCountryId");
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
            UPDATE Users
            SET {string.Join(", ", sets)}
            WHERE UserId = @UserId
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







        public async Task<Users> CheckEmailExistsAsync(
         string email,
         int? excludeId,
         CancellationToken cancellationToken)
        {
            var sql = "SELECT IsDeleted ,UserId  FROM Users WHERE email = @email";
            if (excludeId.HasValue)
                sql += " AND UserId <> @ExcludeId";

            var cmd = new CommandDefinition(sql, new { email = email, ExcludeId = excludeId }, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            var entity = await connection.QueryFirstOrDefaultAsync<Users>(cmd);
            return entity;
           
        }



       

        public async Task<Users> CheckMobileNoExistsAsync(
        string mobileNo,
        int? excludeId,
        CancellationToken cancellationToken)
        {
            var sql = "SELECT IsDeleted ,UserId  FROM Users WHERE MobileNo = @MobileNo";
            if (excludeId.HasValue)
                sql += " AND UserId <> @ExcludeId";

            var cmd = new CommandDefinition(sql, new { MobileNo = mobileNo, ExcludeId = excludeId }, cancellationToken: cancellationToken);
            using var connection = _context.CreateConnection();
            var entity = await connection.QueryFirstOrDefaultAsync<Users>(cmd);
            return entity;

        }




        public async Task<PagedResultDto<EmployeeDto>> GetAllAsync(
    EmployeeFilterDto filter,
    int loginUserId,
    CancellationToken cancellationToken = default)
        {
            var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var limit = filter.Limit <= 0 ? 10 : filter.Limit;
            var offset = (page - 1) * limit;

            var parameters = new DynamicParameters();
            parameters.Add("Offset", offset);
            parameters.Add("Limit", limit);
            parameters.Add("LoginUserId", loginUserId);

            var baseSql = new StringBuilder(@"
        FROM Users p
        LEFT JOIN Country a ON a.CountryId = p.MobileCountryId            
        INNER JOIN Users uC ON uC.UserId = p.CreatedBy
        LEFT JOIN Users uU ON uU.UserId = p.UpdatedBy
        LEFT JOIN Users uD ON uD.UserId = p.DeletedBy
        WHERE 1=1
        AND p.UserId != @LoginUserId
    ");

            // ✅ Filters
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

            // ✅ Date Filters (Still parsing because your DTO uses string)
            if (DateTime.TryParse(filter.CreatedFromDate, out var createdFrom))
            {
                baseSql.Append(" AND p.CreatedAt >= @CreatedFrom");
                parameters.Add("CreatedFrom", createdFrom);
            }

            if (DateTime.TryParse(filter.CreatedToDate, out var createdTo))
            {
                baseSql.Append(" AND p.CreatedAt < DATEADD(day, 1, @CreatedTo)");
                parameters.Add("CreatedTo", createdTo.Date);
            }

            if (DateTime.TryParse(filter.UpdatedFromDate, out var updatedFrom))
            {
                baseSql.Append(" AND p.UpdatedAt >= @UpdatedFrom");
                parameters.Add("UpdatedFrom", updatedFrom);
            }

            if (DateTime.TryParse(filter.UpdatedToDate, out var updatedTo))
            {
                baseSql.Append(" AND p.UpdatedAt < DATEADD(day, 1, @UpdatedTo)");
                parameters.Add("UpdatedTo", updatedTo.Date);
            }

            if (DateTime.TryParse(filter.DeletedFromDate, out var deletedFrom))
            {
                baseSql.Append(" AND p.DeletedAt >= @DeletedFrom");
                parameters.Add("DeletedFrom", deletedFrom);
            }

            if (DateTime.TryParse(filter.DeletedToDate, out var deletedTo))
            {
                baseSql.Append(" AND p.DeletedAt < DATEADD(day, 1, @DeletedTo)");
                parameters.Add("DeletedTo", deletedTo.Date);
            }

            // ✅ Search Mapping (SQL Injection Safe)
            var searchByMap = new Dictionary<EmployeeSearchBy, string>
            {
                [EmployeeSearchBy.FirstName] = "p.FirstName",
                [EmployeeSearchBy.LastName] = "p.LastName",
                [EmployeeSearchBy.Email] = "p.Email",
                [EmployeeSearchBy.MobileNo] = "p.MobileNo",
                [EmployeeSearchBy.Gender] = "p.Gender",
                [EmployeeSearchBy.BirthYear] = "p.BirthYear",
                [EmployeeSearchBy.MobileCountryCode] = "a.CountryDialCode",
                [EmployeeSearchBy.CreatedUser] = "CONCAT(uC.FirstName,' ',uC.LastName)",
                [EmployeeSearchBy.ModifiedUser] = "CONCAT(uU.FirstName,' ',uU.LastName)",
                [EmployeeSearchBy.DeletedUser] = "CONCAT(uD.FirstName,' ',uD.LastName)"
            };

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                parameters.Add("Search", $"%{filter.SearchText}%");

                if (filter.SearchBy.HasValue &&
                    searchByMap.TryGetValue(filter.SearchBy.Value, out var column))
                {
                    baseSql.Append($" AND {column} LIKE @Search");
                }
                else
                {
                    baseSql.Append(@"
                AND (
                    p.FirstName LIKE @Search
                    OR p.LastName LIKE @Search
                    OR p.MobileNo LIKE @Search
                    OR p.Email LIKE @Search
                    OR p.Gender LIKE @Search
                   OR p.BirthYear LIKE @Search
                    OR a.CountryDialCode LIKE @Search
                    OR uC.FirstName LIKE @Search OR uC.LastName LIKE @Search
                    OR uU.FirstName LIKE @Search OR uU.LastName LIKE @Search
                    OR uD.FirstName LIKE @Search OR uD.LastName LIKE @Search
                )");
                }
            }

            // ✅ Sorting (Injection Safe)
            var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["UserId"] = "p.UserId",
                ["FirstName"] = "p.FirstName",
                ["LastName"] = "p.LastName",
                ["Email"] = "p.Email",
                ["MobileNo"] = "p.MobileNo",
                ["Gender"] = "p.Gender",
                ["BirthYear"] = "p.BirthYear",
                ["MobileCountryCode"] = "a.CountryDialCode",
                ["Active"] = "p.IsActive",
                ["Deleted"] = "p.IsDeleted",
                ["CreatedDate"] = "p.CreatedAt",
                ["ModifiedDate"] = "p.UpdatedAt",
                ["DeletedDate"] = "p.DeletedAt"
            };

            var sortKey = string.IsNullOrWhiteSpace(filter.SortBy)
                ? "FirstName"
                : filter.SortBy;

            if (!sortMap.TryGetValue(sortKey, out var sortColumn))
                sortColumn = sortMap["UserId"];

            var sortOrder = filter.SortOrder == SortOrder.Desc ? "DESC" : "ASC";

            // ✅ Queries
            var countSql = $"SELECT COUNT(1) {baseSql}";

            var itemsSql = $@"
        SELECT
            p.UserId AS EmployeeId,
            p.FirstName,
            p.LastName,
            p.Email,
            p.Gender,
            p.MobileNo,
            p.BirthYear,
            ISNULL(a.CountryDialCode,'') AS MobileCountryCode,
            p.MobileCountryId AS MobileCountryCodeId,
            p.IsActive AS Active,
            p.IsDeleted AS [Deleted],
            CONCAT(uC.FirstName, ' ', ISNULL(uC.LastName, '')) AS CreatedUser,
            p.CreatedAt AS CreatedDate,
            ISNULL(CONCAT(uU.FirstName, ' ', ISNULL(uU.LastName, '')), '') AS ModifiedUser,
            p.UpdatedAt AS ModifiedDate,
            ISNULL(CONCAT(uD.FirstName, ' ', ISNULL(uD.LastName, '')), '') AS DeletedUser,
            p.DeletedAt AS DeletedDate
        {baseSql}
        ORDER BY {sortColumn} {sortOrder}
        OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;
    ";

            using var connection = _context.CreateConnection();

            var totalCount = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

            var items = (await connection.QueryAsync<EmployeeDto>(
                new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken)))
                .ToList();

            return new PagedResultDto<EmployeeDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = limit
            };
        }


    }
}
