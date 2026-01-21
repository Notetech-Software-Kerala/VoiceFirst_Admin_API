using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class SysBusinessActivity : BaseModel
{
    public int SysBusinessActivityId { get; set; }
    public string BusinessActivityName { get; set; } = string.Empty;
   
}
