using System;
using VoiceFirst_Admin.Utilities.Models;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class PlanRoleLink : BaseModel
{
    public int PlanRoleLinkId { get; set; }
    public int PlanId { get; set; }
    public int RoleId { get; set; }
}
