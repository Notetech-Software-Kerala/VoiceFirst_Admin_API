using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField;
using VoiceFirst_Admin.Utilities.DTOs.Features.User;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userProfileRepository;
        private readonly IUserRoleLinkRepo _userRoleLinkRepo;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userProfileRepository, IUserRoleLinkRepo userRoleLinkRepo, IMapper mapper)
        {
            _userProfileRepository = userProfileRepository;
            _userRoleLinkRepo = userRoleLinkRepo;
            _mapper = mapper;   
        }

        public async Task<ApiResponse<UserProfileDto>> GetProfileAsync
          (int userId,
            CancellationToken cancellationToken = default)
        {
            var userProfile = await _userProfileRepository.GetProfileAsync(userId, cancellationToken);
            if (userProfile == null)
            {
                throw new Exception($"User profile with ID {userId} not found.");
            }
            userProfile.Roles = await _userRoleLinkRepo.GetRoleNamesByUserIdAsync(userId, cancellationToken);
 
            return  ApiResponse<UserProfileDto>.Ok(
                userProfile,
                Messages.UserProfile,
               StatusCodes.Status200OK
            );

        }


        public async Task<ApiResponse<UserProfileDto>> UpdateProfileAsync
            (
            int userId,
             UserProfileUpdateDto userProfileUpdateDto,
             CancellationToken cancellationToken = default)
        {
            var user = _mapper.Map<Users>((userProfileUpdateDto,userId));
            var Updated = await _userProfileRepository.UpdateProfileAsync(user, cancellationToken);
            if(Updated)
            {
               
                var userProfile =
                    await GetProfileAsync(userId, cancellationToken);
                return ApiResponse<UserProfileDto>.Ok(
                      userProfile.Data,
                      Messages.UserProfileUpdated,
                     StatusCodes.Status200OK
                  );
            }

            return ApiResponse<UserProfileDto>.Fail(
            Messages.UserProfileUpdated,
            StatusCodes.Status204NoContent,
            ErrorCodes.NoRowAffected);
         
        }   
    

    }
}
