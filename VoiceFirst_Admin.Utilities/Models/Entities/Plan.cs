using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class Plan : BaseModel
{
    public int PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
}
