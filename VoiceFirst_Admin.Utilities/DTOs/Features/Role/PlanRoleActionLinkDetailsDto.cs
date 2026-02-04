using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Role;
public class PlanRoleActionLinkDetailsDto
{

    public int PlanRoleLinkId { get; set; }
    public List<PlanRoleActionLinkDto> PlanActionLink { get; set; }
}
