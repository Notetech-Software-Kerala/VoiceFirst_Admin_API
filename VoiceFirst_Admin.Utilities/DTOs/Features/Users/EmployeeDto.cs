using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;
using VoiceFirst_Admin.Utilities.DTOs.Features.UserRoleLink;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Users
{
    public class EmployeeDto
    {
        public int  EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Gender { get; set; }
        public string? MobileNo { get; set; }
        public string? BirthYear { get; set; }
        public int MobileCountryCodeId { get; set; }
        public string MobileCountryCode { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string CreatedUser { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string ModifiedUser { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string DeletedUser { get; set; } = string.Empty;
        public DateTime? DeletedDate { get; set; }
      

    }
}
