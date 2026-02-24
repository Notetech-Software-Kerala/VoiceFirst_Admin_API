namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueMediaType
{
    public class SysIssueMediaTypeDTO { public int IssueMediaTypeId { get; set; } public string IssueMediaType { get; set; } = string.Empty; public bool Active { get; set; } public bool Deleted { get; set; } public string CreatedUser { get; set; } = string.Empty; public DateTime CreatedDate { get; set; } public string ModifiedUser { get; set; } = string.Empty; public DateTime? ModifiedDate { get; set; } public string DeletedUser { get; set; } = string.Empty; public DateTime? DeletedDate { get; set; } }
    public class SysIssueMediaTypeCreateDTO { public string IssueMediaType { get; set; } = string.Empty; }
    public class SysIssueMediaTypeUpdateDTO { public string? IssueMediaType { get; set; } public bool? Active { get; set; } }
    public class SysIssueMediaTypeActiveDTO { public int IssueMediaTypeId { get; set; } public string IssueMediaType { get; set; } = string.Empty; }
}
