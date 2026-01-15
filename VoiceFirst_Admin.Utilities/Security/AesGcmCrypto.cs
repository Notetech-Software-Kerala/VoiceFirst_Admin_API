using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Security;

public static class AesGcmCrypto
{
    public static byte[] GenerateKey(int keySizeBytes = 32)
        => RandomNumberGenerator.GetBytes(keySizeBytes);

    // packed: [nonce(12)] + [tag(16)] + [ciphertext(N)]
    public static byte[] Encrypt(string plaintext, byte[] key, byte[]? associatedData = null)
    {
        if (plaintext is null) throw new ArgumentNullException(nameof(plaintext));
        if (key is null) throw new ArgumentNullException(nameof(key));
        if (key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new ArgumentException("AES key must be 16, 24, or 32 bytes.", nameof(key));

        var nonce = RandomNumberGenerator.GetBytes(12);
        var plainBytes = Encoding.UTF8.GetBytes(plaintext);

        var cipherBytes = new byte[plainBytes.Length];
        var tag = new byte[16];

        using (var aes = new AesGcm(key))
        {
            aes.Encrypt(nonce, plainBytes, cipherBytes, tag, associatedData);
        }

        var output = new byte[nonce.Length + tag.Length + cipherBytes.Length];
        Buffer.BlockCopy(nonce, 0, output, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, output, nonce.Length, tag.Length);
        Buffer.BlockCopy(cipherBytes, 0, output, nonce.Length + tag.Length, cipherBytes.Length);

        return output;
    }

    public static string Decrypt(byte[] packed, byte[] key, byte[]? associatedData = null)
    {
        if (packed is null) throw new ArgumentNullException(nameof(packed));
        if (key is null) throw new ArgumentNullException(nameof(key));
        if (packed.Length < 12 + 16) throw new ArgumentException("Invalid encrypted payload.", nameof(packed));
        if (key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new ArgumentException("AES key must be 16, 24, or 32 bytes.", nameof(key));

        var nonce = new byte[12];
        var tag = new byte[16];
        var cipher = new byte[packed.Length - nonce.Length - tag.Length];

        Buffer.BlockCopy(packed, 0, nonce, 0, nonce.Length);
        Buffer.BlockCopy(packed, nonce.Length, tag, 0, tag.Length);
        Buffer.BlockCopy(packed, nonce.Length + tag.Length, cipher, 0, cipher.Length);

        var plainBytes = new byte[cipher.Length];

        using (var aes = new AesGcm(key))
        {
            aes.Decrypt(nonce, cipher, tag, plainBytes, associatedData);
        }

        return Encoding.UTF8.GetString(plainBytes);
    }

    public static string EncryptToBase64(string plaintext, byte[] key, byte[]? associatedData = null)
        => Convert.ToBase64String(Encrypt(plaintext, key, associatedData));

    public static string DecryptFromBase64(string b64Packed, byte[] key, byte[]? associatedData = null)
        => Decrypt(Convert.FromBase64String(b64Packed), key, associatedData);
}
