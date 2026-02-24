using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class SysIssueMediaFormat : BaseModel
{
    public int SysIssueMediaFormatId { get; set; }
    public string IssueMediaFormat { get; set; } = string.Empty;
}
