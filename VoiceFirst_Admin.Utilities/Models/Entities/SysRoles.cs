using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class SysRoles : BaseModel
{
    public int SysRoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsMandatory { get; set; } = false;
    public string? RolePurpose { get; set; }
    public int ApplicationId { get; set; }
}
