using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices;

public interface ISessionService
{
    Task ActivateSessionAsync(int userId, int sessionId, int deviceId);

    Task<bool> VerifySessionDeviceAsync(int userId, int sessionId, int deviceId);

    Task<long> InitTokenVersionAsync(int userId, int sessionId);

    Task<long> IncrementTokenVersionAsync(int userId, int sessionId);

    Task StoreRefreshTokenAsync(int userId, int sessionId, string refreshToken);

    Task<bool> VerifyRefreshTokenAsync(int userId, int sessionId, string refreshToken);

    Task InvalidateSessionKeysAsync(int userId, int sessionId);

    Task InvalidateAllUserSessionsAsync(int userId, int? excludeSessionId = null);

    Task DeleteRefreshTokenAsync(int userId, int sessionId);

    Task<bool> IsLoginLockedOutAsync(string email);

    Task<int> GetLockoutMinutesRemainingAsync(string email);

    Task<(bool lockedOut, int remainingAttempts)> RecordFailedLoginAsync(string email);

    Task ClearLoginAttemptsAsync(string email);

    TokenPair GenerateTokens(IDictionary<string, object?> claims);

    (string accessToken, DateTime expiresAtUtc) GenerateFreshAccessToken(IDictionary<string, object?> claims);
}
