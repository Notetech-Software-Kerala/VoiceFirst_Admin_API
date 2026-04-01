using System.IdentityModel.Tokens.Jwt;
using VoiceFirst_Admin.Business.Contracts.IServices;

namespace VoiceFirst_Admin.API.Security
{
    /// <summary>
    /// Provides the authenticated user's identity claims.
    /// Token validation (signature, expiry, issuer, audience, session,
    /// device, token-version, fingerprint) is performed by the JWT
    /// middleware in Program.cs before any controller code executes.
    /// This class only extracts already-validated claims.
    /// </summary>
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId => GetRequiredIntClaim(JwtRegisteredClaimNames.Sub);

        public int SessionId => GetRequiredIntClaim("sessionId");

        private int GetRequiredIntClaim(string claimType)
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(claimType);

            if (claim is null || !int.TryParse(claim.Value, out var value))
                throw new UnauthorizedAccessException("User identity is not available.");

            return value;
        }
    }
}
