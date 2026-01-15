using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Security;

public static class JwtTokenHelper
{
    // Creates access + refresh JWT tokens
    public static TokenPair CreateTokenPair(IDictionary<string, object?> claimsData, JwtSettings settings)
    {
        var now = DateTime.UtcNow;

        var accessExpires = now.AddMinutes(settings.AccessTokenExpiryMinutes);
        var refreshExpires = now.AddDays(settings.RefreshTokenExpiryDays);

        var accessToken = CreateJwt(claimsData, settings, accessExpires, tokenType: "access");
        var refreshToken = CreateJwt(claimsData, settings, refreshExpires, tokenType: "refresh");

        return new TokenPair
        {
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAtUtc = refreshExpires
        };
    }

    // Creates only a new access token (fresh)
    public static (string accessToken, DateTime expiresAtUtc) CreateFreshAccessToken(
        IDictionary<string, object?> claimsData, JwtSettings settings)
    {
        var now = DateTime.UtcNow;
        var accessExpires = now.AddMinutes(settings.AccessTokenExpiryMinutes);
        var token = CreateJwt(claimsData, settings, accessExpires, tokenType: "access");
        return (token, accessExpires);
    }

    // Validate refresh token without DB (signature + expiry + tokenType)
    public static ClaimsPrincipal ValidateRefreshToken(string refreshToken, JwtSettings settings)
        => ValidateToken(refreshToken, settings, requiredTokenType: "refresh");
    public static ClaimsPrincipal ValidateToken(string token, JwtSettings jwt, bool validateLifetime = true)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,

            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,

            ValidateAudience = true,
            ValidAudience = jwt.Audience,

            ValidateLifetime = validateLifetime,
            ClockSkew = TimeSpan.FromSeconds(30),
        };

        return handler.ValidateToken(token, parameters, out _);
    }
    private static string CreateJwt(
        IDictionary<string, object?> claimsData,
        JwtSettings settings,
        DateTime expiresAtUtc,
        string tokenType)
    {
        if (!claimsData.TryGetValue(JwtRegisteredClaimNames.Sub, out var sub) || string.IsNullOrWhiteSpace(Convert.ToString(sub)))
            throw new ArgumentException($"Missing claim: {JwtRegisteredClaimNames.Sub}");

        if (!claimsData.TryGetValue(JwtRegisteredClaimNames.Email, out var email) || string.IsNullOrWhiteSpace(Convert.ToString(email)))
            throw new ArgumentException($"Missing claim: {JwtRegisteredClaimNames.Email}");

        var claims = new List<Claim>
        {
            // token type helps distinguish access vs refresh
            new("tokenType", tokenType)
        };

        foreach (var kv in claimsData)
        {
            if (kv.Value is null) continue;

            // roles array support
            //if (kv.Key.Equals("roles", StringComparison.OrdinalIgnoreCase))
            //{
            //    if (kv.Value is IEnumerable<string> roles)
            //    {
            //        foreach (var r in roles.Where(r => !string.IsNullOrWhiteSpace(r)))
            //            claims.Add(new Claim(ClaimTypes.Role, r));
            //    }
            //    continue;
            //}

            var valueStr = Convert.ToString(kv.Value, CultureInfo.InvariantCulture);
            if (string.IsNullOrWhiteSpace(valueStr)) continue;

            claims.Add(new Claim(kv.Key, valueStr));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static ClaimsPrincipal ValidateToken(string token, JwtSettings settings, string requiredTokenType)
    {
        var handler = new JwtSecurityTokenHandler();

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidIssuer = settings.Issuer,
            ValidAudience = settings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
            ClockSkew = TimeSpan.Zero
        };

        var principal = handler.ValidateToken(token, parameters, out _);

        var tokenType = principal.Claims.FirstOrDefault(c => c.Type == "tokenType")?.Value;
        if (!string.Equals(tokenType, requiredTokenType, StringComparison.OrdinalIgnoreCase))
            throw new SecurityTokenException("Invalid token type.");

        return principal;
    }
}