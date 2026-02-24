namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueMediaFormat
{
    public class SysIssueMediaFormatDTO { public int IssueMediaFormatId { get; set; } public string IssueMediaFormat { get; set; } = string.Empty; public bool Active { get; set; } public bool Deleted { get; set; } public string CreatedUser { get; set; } = string.Empty; public DateTime CreatedDate { get; set; } public string ModifiedUser { get; set; } = string.Empty; public DateTime? ModifiedDate { get; set; } public string DeletedUser { get; set; } = string.Empty; public DateTime? DeletedDate { get; set; } }
    public class SysIssueMediaFormatCreateDTO { public string IssueMediaFormat { get; set; } = string.Empty; }
    public class SysIssueMediaFormatUpdateDTO { public string? IssueMediaFormat { get; set; } public bool? Active { get; set; } }
    public class SysIssueMediaFormatActiveDTO { public int IssueMediaFormatId { get; set; } public string IssueMediaFormat { get; set; } = string.Empty; }
}
