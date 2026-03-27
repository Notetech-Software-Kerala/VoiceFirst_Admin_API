using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.User
{
    public class UserProfileDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Gender { get; set; }
        public string? MobileNo { get; set; }
        public string? BirthYear { get; set; }
        public int DialCodeId { get; set; }
        public string DialCode { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
