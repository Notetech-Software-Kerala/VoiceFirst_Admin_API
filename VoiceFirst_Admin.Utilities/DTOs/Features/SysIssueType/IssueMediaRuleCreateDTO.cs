namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType
{
    public class IssueMediaRuleCreateDTO
    {
        public int IssueMediaFormatId { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public int MaxSizeMB { get; set; }
        public List<IssueMediaRuleTypeCreateDTO>? MediaTypes { get; set; }
    }

    public class IssueMediaRuleTypeCreateDTO
    {
        public int IssueMediaTypeId { get; set; }
        public bool IsMandatory { get; set; }
    }

    public class IssueMediaRuleUpdateDTO
    {
        public int IssueMediaFormatId { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public int MaxSizeMB { get; set; }
        public bool? Active { get; set; }
        public List<IssueMediaRuleTypeUpdateDTO>? MediaTypes { get; set; }
    }

    public class IssueMediaRuleTypeUpdateDTO
    {
        public int IssueMediaTypeId { get; set; }
        public bool IsMandatory { get; set; }
        public bool? Active { get; set; }
    }
}
