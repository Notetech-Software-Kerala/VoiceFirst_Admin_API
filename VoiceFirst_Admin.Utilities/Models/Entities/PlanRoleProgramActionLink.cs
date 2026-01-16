using System;
using VoiceFirst_Admin.Utilities.Models;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class PlanRoleProgramActionLink : BaseModel
{
    public int PlanRoleProgramActionLinkId { get; set; }
    public int PlanRoleLinkId { get; set; }
    public int ProgramActionLinkId { get; set; }
}
