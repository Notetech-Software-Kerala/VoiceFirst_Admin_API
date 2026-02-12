using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.UserDevice;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface IAuthRepo
    {
        Task<Users?> GetUserForLoginAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task<Users?> GetUserByIdForAuthAsync(
            int userId,
            CancellationToken cancellationToken = default);

        Task<DeviceUpsertResult> UpsertDeviceAsync(
            UserDevice device,
            CancellationToken cancellationToken = default);

        Task<int> CreateSessionAsync(
            int userId,
            int userDeviceId,
            CancellationToken cancellationToken = default);

        Task InvalidateSessionAsync(
            int userDeviceLoginId,
            CancellationToken cancellationToken = default);

        Task InvalidateAllSessionsAsync(
            int userId,
            CancellationToken cancellationToken = default);

        Task<int?> GetApplicationVersionIdAsync(
            int version,
            CancellationToken cancellationToken = default);

        Task<bool> UpdatePasswordAsync(
            int userId,
            byte[] hashKey,
            byte[] saltKey,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> GetActiveRolesByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);
    }
}
