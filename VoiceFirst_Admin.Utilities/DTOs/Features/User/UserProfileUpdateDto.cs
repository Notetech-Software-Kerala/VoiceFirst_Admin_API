using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.User
{
    public class UserProfileUpdateDto
    {
        public string? FirstName { get; set; } 
        public string? LastName { get; set; } 
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public string? MobileNo { get; set; }
        public string? BirthYear { get; set; }
        public int? DialCodeId { get; set; }
    }
}
