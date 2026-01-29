using System.Collections.Generic;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Role
{
    public class RolePlanLinkCreateDto
    {
        public int RoleId { get; set; }
        public List<int> PlanIds { get; set; } = new();
    }
}
