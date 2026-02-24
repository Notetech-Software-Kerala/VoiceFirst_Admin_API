namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType
{
    public class SysIssueTypeDTO
    {
        public int IssueTypeId { get; set; }
        public string IssueType { get; set; } = string.Empty;
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string CreatedUser { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string ModifiedUser { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string DeletedUser { get; set; } = string.Empty;
        public DateTime? DeletedDate { get; set; }
        public List<IssueMediaRuleDTO>? MediaRules { get; set; }
    }

    public class IssueMediaRuleDTO
    {
        public int IssueMediaRuleId { get; set; }
        public int IssueMediaFormatId { get; set; }
        public string IssueMediaFormat { get; set; } = string.Empty;
        public int Min { get; set; }
        public int Max { get; set; }
        public int MaxSizeMB { get; set; }
        public string CreatedUser { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public bool Active { get; set; }
        public string ModifiedUser { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public List<IssueMediaRuleTypeDTO>? MediaTypes { get; set; }
    }

    public class IssueMediaRuleTypeDTO
    {
        public int IssueMediaRuleId { get; set; }
        public int IssueMediaRuleTypeId { get; set; }
        public int IssueMediaTypeId { get; set; }
        public string IssueMediaType { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public string CreatedUser { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public bool Active { get; set; }
        public string ModifiedUser { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
    }
}
