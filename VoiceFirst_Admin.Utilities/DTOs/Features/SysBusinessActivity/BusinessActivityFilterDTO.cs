using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity
{
    public class BusinessActivityFilterDTO : CommonFilterDto
    {
        public BusinessActivitySearchBy? SearchBy { get; set; }
    }
}

public enum BusinessActivitySearchBy
{
    ActivityName,
    CreatedUser,
    UpdatedUser,
    DeletedUser
}

