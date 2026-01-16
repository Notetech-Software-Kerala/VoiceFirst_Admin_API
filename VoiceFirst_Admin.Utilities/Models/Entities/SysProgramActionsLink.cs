using System;
using VoiceFirst_Admin.Utilities.Models;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class SysProgramActionsLink : BaseModel
{
    public int SysProgramActionLinkId { get; set; }
    public int ProgramId { get; set; }
    public int ProgramActionId { get; set; }
}
