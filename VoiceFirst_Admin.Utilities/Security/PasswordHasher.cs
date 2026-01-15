using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Security;
public static class PasswordHasher
{
    public static (byte[] hash, byte[] salt) CreateHash(string password)
    {
        // Simple & common approach; for production you can upgrade to PBKDF2/Argon2.
        using var hmac = new HMACSHA512();
        var salt = hmac.Key;
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return (hash, salt);
    }

    public static bool Verify(string password, byte[] storedHash, byte[] storedSalt)
    {
        using var hmac = new HMACSHA512(storedSalt);
        var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return CryptographicOperations.FixedTimeEquals(computed, storedHash);
    }
}
