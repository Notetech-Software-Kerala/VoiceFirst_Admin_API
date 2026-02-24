using System;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType;

public class SysIssueTypeCreateDTO
{
    public string IssueType { get; set; } = string.Empty;
    public List<IssueMediaRuleCreateDTO>? MediaRules { get; set; }
}
