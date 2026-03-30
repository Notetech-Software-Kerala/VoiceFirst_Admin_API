using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.User;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface IUserRepository
    {
        Task<UserProfileDto?> GetProfileAsync
          (int userId,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateProfileAsync
            (
             Users entity,
             CancellationToken cancellationToken = default);
    }
}
