using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Role;
public class PlanRoleActionLinkUpdateDto
{
    public int RolePlanLinkId { get; set; }
    public bool? Active { get; set; }
    public List<ActionLinkDto>? UpdateActionLinks { get; set; }
}
