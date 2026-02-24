using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueMediaType
{
    public class IssueMediaTypeFilterDTO : CommonFilterDto
    {
        public IssueMediaTypeSearchBy? SearchBy { get; set; }
    }
}

public enum IssueMediaTypeSearchBy { IssueMediaType, CreatedUser, ModifiedUser, DeletedUser }
