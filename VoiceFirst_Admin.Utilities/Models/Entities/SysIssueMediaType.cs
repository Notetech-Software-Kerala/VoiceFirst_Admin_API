using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class SysIssueMediaType : BaseModel
{
    public int SysIssueMediaTypeId { get; set; }
    public string IssueMediaType { get; set; } = string.Empty;
}
