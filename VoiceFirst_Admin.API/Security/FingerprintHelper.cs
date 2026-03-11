using System.Security.Cryptography;
using System.Text;

namespace VoiceFirst_Admin.API.Security;

/// <summary>
/// Computes a client fingerprint from multiple HTTP headers.
/// Combining several headers makes spoofing significantly harder
/// because an attacker must replicate all signals simultaneously.
/// </summary>
public static class FingerprintHelper
{
    public static string Compute(IHeaderDictionary headers)
    {
        var raw = string.Concat(
            headers.UserAgent.ToString(),
            headers.AcceptLanguage.ToString(),
            headers.AcceptEncoding.ToString(),
            headers["Sec-CH-UA"].ToString(),
            headers["Sec-CH-UA-Platform"].ToString(),
            headers["Sec-CH-UA-Mobile"].ToString()
        );

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToBase64String(bytes);
    }
}
