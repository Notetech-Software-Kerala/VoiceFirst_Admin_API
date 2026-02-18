using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Role;
public class PlanRoleActionLinkDetailsDto
{

    public int PlanRoleLinkId { get; set; }
    public int PlanId { get; set; }
    public bool Active { get; set; }
    public string PlanName { get; set; }
    public string CreatedUser { get; set; } 
    public DateTime CreatedDate { get; set; }
    public string? ModifiedUser { get; set; } 
    public DateTime? ModifiedDate { get; set; }
    public List<PlanRoleActionLinkDto> PlanActionLink { get; set; }
}
