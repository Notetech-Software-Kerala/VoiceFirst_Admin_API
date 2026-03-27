using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.User;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface IUserProfileRepository
    {
        Task<UserProfileDto?> GetProfileAsync
          (int userId,
            CancellationToken cancellationToken = default);
    }
}
