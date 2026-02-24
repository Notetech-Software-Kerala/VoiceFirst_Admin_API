using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueMediaFormat
{
    public class IssueMediaFormatFilterDTO : CommonFilterDto { public IssueMediaFormatSearchBy? SearchBy { get; set; } }
}
public enum IssueMediaFormatSearchBy { IssueMediaFormat, CreatedUser, ModifiedUser, DeletedUser }
