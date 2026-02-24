using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueCharacterType
{
    public class IssueCharacterTypeFilterDTO : CommonFilterDto
    {
        public IssueCharacterTypeSearchBy? SearchBy { get; set; }
    }
}

public enum IssueCharacterTypeSearchBy
{
    IssueCharacterType,
    CreatedUser,
    ModifiedUser,
    DeletedUser
}
