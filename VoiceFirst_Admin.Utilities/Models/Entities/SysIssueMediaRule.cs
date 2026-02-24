namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class SysIssueMediaRule
{
    public int SysIssueMediaRuleId { get; set; }
    public int IssueTypeId { get; set; }
    public int IssueMediaFormatId { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }
    public int MaxSizeMB { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

}
