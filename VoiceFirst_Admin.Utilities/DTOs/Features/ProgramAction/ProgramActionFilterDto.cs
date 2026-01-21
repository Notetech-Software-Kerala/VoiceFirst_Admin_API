using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction
{
    public class ProgramActionFilterDto : CommonFilterDto
    {
        public ProgramActionSearchBy? SearchBy { get; set; }
    }
}

public enum ProgramActionSearchBy
{
    ActionName,
    CreatedUser,
    UpdatedUser,
    DeletedUser
}