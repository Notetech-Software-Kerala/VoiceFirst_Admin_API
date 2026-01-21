using System;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram
{
    public class SysProgramFilterDTO : CommonFilterDto
    {
        public SysProgramSearchBy? SearchBy { get; set; }
    }

    public enum SysProgramSearchBy
    {
        ProgramName,
        Label,
        Route,
        PlatformName,
        CompanyName,
        CreatedUser,
        ModifiedUser,
        DeletedUser
    }
}
