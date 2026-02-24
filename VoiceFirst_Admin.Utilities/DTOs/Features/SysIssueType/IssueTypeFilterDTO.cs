using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType
{
    public class IssueTypeFilterDTO : CommonFilterDto
    {
        public IssueTypeSearchBy? SearchBy { get; set; }
    }
}

public enum IssueTypeSearchBy
{
    IssueType,
    CreatedUser,
    ModifiedUser,
    DeletedUser
}
