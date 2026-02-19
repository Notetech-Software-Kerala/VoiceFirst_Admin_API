using Microsoft.AspNetCore.Authorization;
using VoiceFirst_Admin.Utilities.Constants;

namespace VoiceFirst_Admin.API.Security
{
    public class AuthorizeSuperAdminAttribute : AuthorizeAttribute
    {
        public AuthorizeSuperAdminAttribute()
            : base(AuthPolicies.SuperAdmin) { }
    }

    public class AuthorizeAdminAttribute : AuthorizeAttribute
    {
        public AuthorizeAdminAttribute()
            : base(AuthPolicies.Admin) { }
    }
}
