using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class SysIssueStatus : BaseModel
{
    public int SysIssueStatusId { get; set; }
    public string IssueStatus { get; set; } = string.Empty;
}
