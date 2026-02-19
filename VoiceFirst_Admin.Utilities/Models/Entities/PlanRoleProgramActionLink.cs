using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class PlanRoleProgramActionLink : BaseModel
{
    public int PlanRoleProgramActionLinkId { get; set; }
    public string PlanName { get; set; }
    public int PlanId { get; set; }
    public bool? PlanRoleLinkActive { get; set; }
    public int PlanRoleLinkId { get; set; }
    public int ProgramActionLinkId { get; set; }
    public string ProgramActionName { get; set; }
    public string PlanRoleLinkCreatedUserName { get; set; }
    public DateTime PlanRoleLinkCreatedAt { get; set; }
    public string? PlanRoleLinkUpdatedUserName { get; set; }
    public DateTime? PlanRoleLinkUpdatedAt { get; set; }
}
