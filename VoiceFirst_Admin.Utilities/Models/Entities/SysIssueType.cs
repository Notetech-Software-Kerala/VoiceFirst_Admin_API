using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class SysIssueType : BaseModel
{
    public int SysIssueTypeId { get; set; }
    public string IssueType { get; set; } = string.Empty;
}
