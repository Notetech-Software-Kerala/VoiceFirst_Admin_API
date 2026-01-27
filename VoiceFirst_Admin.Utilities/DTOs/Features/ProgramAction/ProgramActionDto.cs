using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction
{
    public class ProgramActionDto : CommonDto
    {
        public int ActionId { get; set; }
        public string? ActionName { get; set; }
    }
}
