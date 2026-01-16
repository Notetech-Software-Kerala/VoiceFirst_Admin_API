using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class PlanProgramActionLink : BaseModel
{
    public int PlanProgramActionLinkId { get; set; }
    public int PlanId { get; set; }
    public int ProgramActionLinkId { get; set; }
}
