using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.User;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface IUserProfileService
    {
        Task<ApiResponse<UserProfileDto>> GetProfileAsync
          (int userId,
            CancellationToken cancellationToken = default);
    }
}
