using AutoMapper;
using Microsoft.AspNetCore.Http;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.User;
using VoiceFirst_Admin.Utilities.Exceptions;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleLinkRepo _userRoleLinkRepo;
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository,
            IUserRoleLinkRepo userRoleLinkRepo,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _userRoleLinkRepo = userRoleLinkRepo;
            _mapper = mapper;
        }

        public async Task<ApiResponse<UserProfileDto>> GetProfileAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            var userProfile = await _userRepository.GetProfileAsync(userId, cancellationToken)
                ?? throw new BusinessNotFoundException(Messages.UserNotFound, ErrorCodes.NotFound);

            userProfile.Roles = await _userRoleLinkRepo.GetRoleNamesByUserIdAsync(userId, cancellationToken);

            return ApiResponse<UserProfileDto>.Ok(
                userProfile,
                Messages.UserProfile,
                StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<UserProfileDto>> UpdateProfileAsync(
            int userId,
            UserProfileUpdateDto userProfileUpdateDto,
            CancellationToken cancellationToken = default)
        {
            var user = _mapper.Map<Users>((userProfileUpdateDto, userId));

            var updated = await _userRepository.UpdateProfileAsync(user, cancellationToken);

            if (!updated)
            {
                return ApiResponse<UserProfileDto>.Fail(
                    Messages.UserProfileUpdateFailed,
                    StatusCodes.Status204NoContent,
                    ErrorCodes.NoRowAffected);
            }

            var profileResponse = await GetProfileAsync(userId, cancellationToken);

            return ApiResponse<UserProfileDto>.Ok(
                profileResponse.Data,
                Messages.UserProfileUpdated,
                StatusCodes.Status200OK);
        }
    }
}
