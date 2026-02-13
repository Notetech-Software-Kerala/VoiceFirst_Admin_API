using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Security;

namespace VoiceFirst_Admin.Business.Services;

public class SessionService : ISessionService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly JwtSettings _jwtSettings;

    private const string RefreshTokenKeyPrefix = "refresh_token:";
    private const string SessionKeyPrefix = "session:";
    private const string LoginAttemptKeyPrefix = "login_attempts:";
    private const string LoginLockoutKeyPrefix = "login_lockout:";
    private const int MaxLoginAttempts = 5;
    private static readonly TimeSpan LoginLockoutDuration = TimeSpan.FromMinutes(15);

    public SessionService(IConnectionMultiplexer redis, JwtSettings jwtSettings)
    {
        _redis = redis;
        _jwtSettings = jwtSettings;
    }

    public async Task ActivateSessionAsync(int userId, int sessionId, int deviceId, string fingerprint)
    {
        var db = _redis.GetDatabase();
        var key = $"{SessionKeyPrefix}{userId}:{sessionId}";
        await db.HashSetAsync(key,
        [
            new HashEntry("deviceId", deviceId.ToString()),
            new HashEntry("fingerprint", fingerprint)
        ]);
        await db.KeyExpireAsync(key, TimeSpan.FromDays(_jwtSettings.RefreshTokenExpiryDays));
    }

    public async Task<bool> VerifySessionDeviceAsync(int userId, int sessionId, int deviceId)
    {
        var db = _redis.GetDatabase();
        var key = $"{SessionKeyPrefix}{userId}:{sessionId}";
        var storedDeviceId = await db.HashGetAsync(key, "deviceId");

        if (storedDeviceId.IsNullOrEmpty)
            return false;

        return storedDeviceId == deviceId.ToString();
    }

    public async Task<bool> VerifyFingerprintAsync(int userId, int sessionId, string fingerprint)
    {
        var db = _redis.GetDatabase();
        var key = $"{SessionKeyPrefix}{userId}:{sessionId}";
        var stored = await db.HashGetAsync(key, "fingerprint");

        if (stored.IsNullOrEmpty)
            return false;

        return stored == fingerprint;
    }

    public async Task<long> InitTokenVersionAsync(int userId, int sessionId)
    {
        var db = _redis.GetDatabase();
        var key = $"{SessionKeyPrefix}{userId}:{sessionId}";
        await db.HashSetAsync(key, "tokenVer", "1");
        await db.KeyExpireAsync(key, TimeSpan.FromDays(_jwtSettings.RefreshTokenExpiryDays));
        return 1;
    }

    public async Task<long> IncrementTokenVersionAsync(int userId, int sessionId)
    {
        var db = _redis.GetDatabase();
        var key = $"{SessionKeyPrefix}{userId}:{sessionId}";
        var newVersion = await db.HashIncrementAsync(key, "tokenVer");
        await db.KeyExpireAsync(key, TimeSpan.FromDays(_jwtSettings.RefreshTokenExpiryDays));
        return newVersion;
    }

    public async Task StoreRefreshTokenAsync(int userId, int sessionId, string refreshToken)
    {
        var db = _redis.GetDatabase();
        var hash = HashToken(refreshToken);
        var key = $"{RefreshTokenKeyPrefix}{userId}:{sessionId}";
        await db.StringSetAsync(
            key, hash, TimeSpan.FromDays(_jwtSettings.RefreshTokenExpiryDays));
    }

    public async Task<bool> VerifyRefreshTokenAsync(int userId, int sessionId, string refreshToken)
    {
        var db = _redis.GetDatabase();
        var key = $"{RefreshTokenKeyPrefix}{userId}:{sessionId}";
        var storedHash = await db.StringGetAsync(key);

        if (storedHash.IsNullOrEmpty)
            return false;

        var incomingHash = HashToken(refreshToken);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(storedHash!),
            Encoding.UTF8.GetBytes(incomingHash));
    }

    public async Task DeleteRefreshTokenAsync(int userId, int sessionId)
    {
        var db = _redis.GetDatabase();
        var key = $"{RefreshTokenKeyPrefix}{userId}:{sessionId}";
        await db.KeyDeleteAsync(key);
    }

    public async Task InvalidateSessionKeysAsync(int userId, int sessionId)
    {
        var db = _redis.GetDatabase();
        var refreshKey = $"{RefreshTokenKeyPrefix}{userId}:{sessionId}";
        var sessionKey = $"{SessionKeyPrefix}{userId}:{sessionId}";
        await db.KeyDeleteAsync(refreshKey);
        await db.KeyDeleteAsync(sessionKey);
    }

    public async Task InvalidateAllUserSessionsAsync(int userId, int? excludeSessionId = null)
    {
        var db = _redis.GetDatabase();
        var server = _redis.GetServers().First();
        var suffix = excludeSessionId.HasValue ? $":{excludeSessionId.Value}" : null;

        var patterns = new[]
        {
            $"{SessionKeyPrefix}{userId}:*",
            $"{RefreshTokenKeyPrefix}{userId}:*"
        };

        foreach (var pattern in patterns)
        {
            await foreach (var key in server.KeysAsync(pattern: pattern))
            {
                if (suffix is not null && ((string)key!).EndsWith(suffix)) continue;
                await db.KeyDeleteAsync(key);
            }
        }
    }

    public async Task<bool> IsLoginLockedOutAsync(string email)
    {
        var db = _redis.GetDatabase();
        var lockoutKey = $"{LoginLockoutKeyPrefix}{email.ToLowerInvariant()}";
        return await db.KeyExistsAsync(lockoutKey);
    }

    public async Task<int> GetLockoutMinutesRemainingAsync(string email)
    {
        var db = _redis.GetDatabase();
        var lockoutKey = $"{LoginLockoutKeyPrefix}{email.ToLowerInvariant()}";
        var ttl = await db.KeyTimeToLiveAsync(lockoutKey);
        return ttl.HasValue ? (int)Math.Ceiling(ttl.Value.TotalMinutes) : (int)LoginLockoutDuration.TotalMinutes;
    }

    public async Task<(bool lockedOut, int remainingAttempts)> RecordFailedLoginAsync(string email)
    {
        var db = _redis.GetDatabase();
        var normalizedEmail = email.ToLowerInvariant();
        var attemptKey = $"{LoginAttemptKeyPrefix}{normalizedEmail}";

        var attempts = await db.StringIncrementAsync(attemptKey);

        if (attempts == 1)
        {
            await db.KeyExpireAsync(attemptKey, LoginLockoutDuration);
        }

        if (attempts >= MaxLoginAttempts)
        {
            var lockoutKey = $"{LoginLockoutKeyPrefix}{normalizedEmail}";
            await db.StringSetAsync(lockoutKey, "1", LoginLockoutDuration);
            await db.KeyDeleteAsync(attemptKey);
            return (true, 0);
        }

        return (false, MaxLoginAttempts - (int)attempts);
    }

    public async Task ClearLoginAttemptsAsync(string email)
    {
        var db = _redis.GetDatabase();
        var attemptKey = $"{LoginAttemptKeyPrefix}{email.ToLowerInvariant()}";
        await db.KeyDeleteAsync(attemptKey);
    }

    public TokenPair GenerateTokens(IDictionary<string, object?> claims)
    {
        return JwtTokenHelper.CreateTokenPair(claims, _jwtSettings);
    }

    public (string accessToken, DateTime expiresAtUtc) GenerateFreshAccessToken(IDictionary<string, object?> claims)
    {
        return JwtTokenHelper.CreateFreshAccessToken(claims, _jwtSettings);
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}
