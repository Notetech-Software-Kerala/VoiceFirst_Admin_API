using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Common;

public static class TextGenerator
{
    // --------- Charsets ----------
    private const string Digits = "0123456789";
    private const string AlphaLower = "abcdefghijklmnopqrstuvwxyz";
    private const string AlphaUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string AlphaNumeric = AlphaLower + AlphaUpper + Digits;

    // --------- OTP ----------
    public static string GenerateOtp(int length = 6, bool allowLeadingZero = false)
    {
        if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));

        // If leading zero not allowed, first digit must be 1-9
        if (!allowLeadingZero)
        {
            var first = GetRandomChars("123456789", 1);
            var rest = length == 1 ? "" : GetRandomChars(Digits, length - 1);
            return first + rest;
        }

        return GetRandomChars(Digits, length);
    }

    // --------- Random Text ----------
    public static string GenerateRandomString(int length = 12)
    {
        if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));
        return GetRandomChars(AlphaNumeric, length);
    }

    public static string GenerateRandomLower(int length = 12)
    {
        if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));
        return GetRandomChars(AlphaLower, length);
    }

    public static string GenerateRandomUpper(int length = 12)
    {
        if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));
        return GetRandomChars(AlphaUpper, length);
    }

    public static string GenerateRandomNumeric(int length = 12, bool allowLeadingZero = true)
    {
        if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));
        return GenerateOtp(length, allowLeadingZero);
    }

    // --------- Path / File helpers ----------
    public static string MediaPath => "Media/";

    public static string BuildMediaFileName(string originalFileName, int randomPartLength = 8)
    {
        var ext = Path.GetExtension(originalFileName)?.ToLowerInvariant() ?? "";
        var stamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var rnd = GenerateRandomString(randomPartLength);

        return $"IMG_{rnd}_{stamp}{ext}";
    }

    public static string BuildFileName(string prefix, string originalFileName, int randomPartLength = 8)
    {
        var ext = Path.GetExtension(originalFileName)?.ToLowerInvariant() ?? "";
        var stamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var rnd = GenerateRandomString(randomPartLength);

        return $"{prefix}_{rnd}_{stamp}{ext}";
    }

    public static string SafeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return "file";

        foreach (var c in Path.GetInvalidFileNameChars())
            fileName = fileName.Replace(c, '_');

        return fileName.Trim();
    }

    // --------- Internal secure random ----------
    private static string GetRandomChars(string charset, int length)
    {
        var result = new char[length];
        var bytes = RandomNumberGenerator.GetBytes(length);

        for (int i = 0; i < length; i++)
            result[i] = charset[bytes[i] % charset.Length];

        return new string(result);
    }
}
