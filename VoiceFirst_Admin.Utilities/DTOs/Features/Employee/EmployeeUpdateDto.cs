using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;
using VoiceFirst_Admin.Utilities.DTOs.Features.UserRoleLink;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Users
{
    public class EmployeeUpdateDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public string? MobileNo { get; set; }
        public short? BirthYear { get; set; }
        public int? DialCodeId { get; set; }
        public bool? Active { get; set; }    
        public List<UserRoleLinkUpdateDto>? UpdateRoles { get; set; }
        public List<int>? InsertRoles { get; set; }
    }
}
