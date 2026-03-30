using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.User;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface IUserService
    {
        Task<ApiResponse<UserProfileDto>> GetProfileAsync
          (int userId,
            CancellationToken cancellationToken = default);

        Task<ApiResponse<UserProfileDto>> UpdateProfileAsync
            (
            int userId,
             UserProfileUpdateDto userProfileUpdateDto,
             CancellationToken cancellationToken = default);
    }
}
