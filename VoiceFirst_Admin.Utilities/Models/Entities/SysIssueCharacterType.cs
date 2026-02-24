using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class SysIssueCharacterType : BaseModel
{
    public int SysIssueCharacterTypeId { get; set; }
    public string IssueCharacterType { get; set; } = string.Empty;
}
