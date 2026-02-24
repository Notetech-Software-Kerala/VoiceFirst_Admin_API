using System;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType;

public class SysIssueTypeUpdateDTO
{
    public string? IssueType { get; set; }
    public bool? Active { get; set; }
    // For program-style updates: separate lists for updating existing rules and inserting new ones
    public List<IssueMediaRuleUpdateDTO>? UpdateMediaRules { get; set; }
    public List<IssueMediaRuleCreateDTO>? InsertMediaRules { get; set; }
}
