using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VoiceFirst_Admin.Business.Contracts.IServices;

namespace VoiceFirst_Admin.API.Security
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User
                    .FindFirst(JwtRegisteredClaimNames.Sub);

                if (claim is null || !int.TryParse(claim.Value, out var userId))
                    throw new UnauthorizedAccessException("User identity is not available.");

                return userId;
            }
        }
    }
}
