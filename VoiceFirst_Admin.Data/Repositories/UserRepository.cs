using Dapper;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.User;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDapperContext _dapperContext;

        public UserRepository(IDapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<UserProfileDto?> GetProfileAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT 
                    P.FirstName,
                    P.LastName,
                    P.Gender,
                    P.Email,
                    C.CountryId AS DialCodeId,
                    ISNULL(c.CountryDialCode ,'') AS DialCode,
                    P.MobileNo,
                    P.BirthYear
                FROM Users p
                LEFT JOIN Country c ON c.CountryId = p.MobileCountryId     
                WHERE p.UserId = @UserId;";

            using var connection = _dapperContext.CreateConnection();

            var dto = await connection.QueryFirstOrDefaultAsync<UserProfileDto>(
                new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));

            return dto;
        }

        public async Task<bool> UpdateProfileAsync(
            Users entity,
            CancellationToken cancellationToken = default)
        {
            var sets = new List<string>();
            var parameters = new DynamicParameters();

            // -------------------------
            // REQUIRED PARAMETERS
            // -------------------------
            parameters.Add("UserId", entity.UserId);
            parameters.Add("UpdatedBy", entity.UserId);

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

            if (!string.IsNullOrWhiteSpace(entity.Gender))
            {
                parameters.Add("Gender", entity.Gender);
                sets.Add("Gender = @Gender");
            }

            if (!string.IsNullOrWhiteSpace(entity.MobileNo))
            {
                parameters.Add("MobileNo", entity.MobileNo);
                sets.Add("MobileNo = @MobileNo");
            }

            if (entity.MobileCountryId > 0)
            {
                parameters.Add("MobileCountryId", entity.MobileCountryId);
                sets.Add("MobileCountryId = @MobileCountryId");
            }

            if (entity.BirthYear > 0)
            {
                parameters.Add("BirthYear", entity.BirthYear);
                sets.Add("BirthYear = @BirthYear");
            }

            // Nothing to update
            if (sets.Count == 0)
                return false;

            // -------------------------
            // AUDIT FIELDS
            // -------------------------
            sets.Add("UpdatedBy = @UpdatedBy");
            sets.Add("UpdatedAt = SYSDATETIME()");

            // -------------------------
            // SQL
            // -------------------------
            var sql = $@"
                UPDATE Users
                SET {string.Join(", ", sets)}
                WHERE UserId = @UserId
                  AND IsDeleted = 0 AND IsActive = 1;";

            using var connection = _dapperContext.CreateConnection();

            var affected = await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    parameters,
                    cancellationToken: cancellationToken));

            return affected > 0;
        }
    }
}
