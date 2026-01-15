using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Security
{
    public static class BrowserFingerprint
    {
        public static string Compute(string? userAgent, string? acceptLanguage, string? ip)
        {
            // keep it stable: UA + language (IP changes often, include only if you want strict)
            var raw = $"{userAgent ?? ""}|{acceptLanguage ?? ""}|{ip ?? ""}";

            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
            return Convert.ToBase64String(bytes);
        }
    }
}
