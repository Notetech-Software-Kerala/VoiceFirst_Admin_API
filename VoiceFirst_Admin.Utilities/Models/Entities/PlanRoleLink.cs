using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class PlanRoleLink : BaseModel
{
    public int PlanRoleLinkId { get; set; }
    public int PlanId { get; set; }
    public int RoleId { get; set; }
}
