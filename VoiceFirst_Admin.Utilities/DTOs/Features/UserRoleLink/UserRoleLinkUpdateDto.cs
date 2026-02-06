using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.UserRoleLink
{
    public class UserRoleLinkUpdateDto
    {

        public int RoleId { get; set; }
        public bool Active { get; set; } = true;
    }
}
