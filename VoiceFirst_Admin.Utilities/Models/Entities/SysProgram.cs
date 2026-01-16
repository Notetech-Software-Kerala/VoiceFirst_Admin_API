using System;
using VoiceFirst_Admin.Utilities.Models;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class SysProgram : BaseModel
{
    public int SysProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public string LabelName { get; set; } = string.Empty;
    public string ProgramRoute { get; set; } = string.Empty;
    public int ApplicationId { get; set; }
    public int CompanyId { get; set; }
}
