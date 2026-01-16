using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class SysProgramActions : BaseModel
{
    public int SysProgramActionId { get; set; }
    public string ProgramActionName { get; set; } = string.Empty;
}
