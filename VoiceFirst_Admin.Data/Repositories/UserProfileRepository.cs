using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.User;
using VoiceFirst_Admin.Utilities.DTOs.Features.Users;

namespace VoiceFirst_Admin.Data.Repositories
{
    public class UserProfileRepository: IUserProfileRepository
    {
        
        private readonly IDapperContext _dapperContext;

        public UserProfileRepository(IDapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }


        public async Task<UserProfileDto?> GetProfileAsync
          (int userId, 
            CancellationToken cancellationToken = default)
        {


            const string sql = @"
            SELECT 
                P.FirstName,
                P.LastName,
                P.BirthYear,
                P.Gender,
                P.Email,
                C.CountryId AS DialCodeId,
                ISNULL(c.CountryDialCode ,'') AS DialCode,
                P.MobileNo,
                P.BirthYear
            FROM Users p
            LEFT JOIN Country c ON c.CountryId = p.MobileCountryId     
            WHERE p.UserId = @UserId;";
            var connection = _dapperContext.CreateConnection(); 

            var dto = await connection.QueryFirstOrDefaultAsync<UserProfileDto>(
                new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));
            return dto;
        }

    }
}
