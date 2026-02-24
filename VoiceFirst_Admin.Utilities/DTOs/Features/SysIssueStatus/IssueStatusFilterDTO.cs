using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueStatus
{
    public class IssueStatusFilterDTO : CommonFilterDto
    {
        public IssueStatusSearchBy? SearchBy { get; set; }
    }
}

public enum IssueStatusSearchBy
{
    IssueStatus,
    CreatedUser,
    ModifiedUser,
    DeletedUser
}
