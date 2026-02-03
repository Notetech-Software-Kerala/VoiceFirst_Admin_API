using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using VoiceFirst_Admin.Data.Context;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class UserRepo:IUserRepo
    {
        private readonly DapperContext _context;

        public UserRepo(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateUserAsync
            (
            Users user,
            IDbConnection connection,
            IDbTransaction transaction
            )
        {
            const string sql = @"
            INSERT INTO dbo.Users
            (
                FirstName,
                LastName,
                Gender,
                LinkedinId,
                FacebookId,
                GoogleId,
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
                @LinkedinId,
                @FacebookId,
                @GoogleId,
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

           
                var lastInsertId = await connection. ExecuteScalarAsync<int>
                (sql, user, transaction);

             
                return lastInsertId;
           
        }




        public async Task<Users> CheckUserEmailExistsAsync(
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




        //public async Task<UserEntity> CheckUserGoogleIdExistsAsync(
        //string google_id,
        //IDbConnection dbConnection,
        //IDbTransaction dbTransaction = null)
        //{
        //    const string sql = @"
        //    SELECT * FROM users 
        //    WHERE google_id = @google_id AND is_deleted = 0;
        //";

        //    return await RecordExistsAsync(sql, new { google_id = google_id }, "google_id", dbConnection, dbTransaction);
        //}



        //public async Task<UserEntity> CheckUserMobileExistsAsync(
        //    string mobileNo,
        //    IDbConnection dbConnection,
        //    IDbTransaction dbTransaction = null)
        //{
        //    const string sql = @"
        //    SELECT * FROM users 
        //    WHERE mobile_no = @MobileNo AND is_deleted = 0;
        //";

        //    return await RecordExistsAsync(sql, new { MobileNo = mobileNo }, "Mobile number", dbConnection, dbTransaction);
        //}


    }
}
