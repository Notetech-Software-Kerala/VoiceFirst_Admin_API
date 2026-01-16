using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Models.Common
{
    public class TokenPair
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAtUtc { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiresAtUtc { get; set; }
    }
}
