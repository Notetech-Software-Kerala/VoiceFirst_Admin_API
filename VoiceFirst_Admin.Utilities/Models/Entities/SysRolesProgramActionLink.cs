using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class SysRolesProgramActionLink : BaseModel
{
    public int SysRoleProgramActionLinkId { get; set; }
    public int SysRoleId { get; set; }
    public string ProgramActionName { get; set; }
    public int ProgramActionLinkId { get; set; }
    public int ProgramActionId { get; set; }
}
