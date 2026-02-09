using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface IAuthRepo
    {
        Task<Users?> GetUserByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task<bool> UpdatePasswordAsync(
            int userId,
            byte[] hashKey,
            byte[] saltKey,
            CancellationToken cancellationToken = default);
    }
}
