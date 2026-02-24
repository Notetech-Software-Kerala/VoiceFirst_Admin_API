namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class SysIssueMediaRuleType
{
    public int SysIssueMediaRuleTypeId { get; set; }
    public int IssueMediaRuleId { get; set; }
    public int IssueMediaTypeId { get; set; }
    public bool IsMandatory { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

}
