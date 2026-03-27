using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.UserRoleLink;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Users
{
    public class EmployeeDetailDto:EmployeeDto
    {
        public List<UserRoleLinksDto> Roles { get; set; } = new();
    }
}
