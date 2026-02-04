using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.Security;
public static class PasswordHasher
{

    private const int SaltSize = 32;
    private const int KeySize = 64;
    private const int Iterations = 350000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

    public static Task<PasswordHashResultDTO> HashPasswordAsync(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        var salt = Convert.ToBase64String(saltBytes);
        var hashBytes = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, HashAlgorithm, KeySize);
        var hash = Convert.ToBase64String(hashBytes);
        return Task.FromResult(new PasswordHashResultDTO { Hash = hash, Salt = salt });
    }

    public static Task<bool> VerifyPasswordAsync(string password, string storedHash, string storedSalt)
    {
        var saltBytes = Convert.FromBase64String(storedSalt);
        var hashBytes = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, HashAlgorithm, KeySize);
        var storedHashBytes = Convert.FromBase64String(storedHash);
        var result = CryptographicOperations.FixedTimeEquals(hashBytes, storedHashBytes);
        return Task.FromResult(result);
    }
}
