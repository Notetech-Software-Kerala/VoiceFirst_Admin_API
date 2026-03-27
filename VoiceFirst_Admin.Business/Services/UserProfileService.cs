using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField;
using VoiceFirst_Admin.Utilities.DTOs.Features.User;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Services
{
    public class UserProfileService:IUserProfileService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUserRoleLinkRepo _userRoleLinkRepo;

        public UserProfileService(IUserProfileRepository userProfileRepository, IUserRoleLinkRepo userRoleLinkRepo)
        {
            _userProfileRepository = userProfileRepository;
            _userRoleLinkRepo = userRoleLinkRepo;
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




    }
}
