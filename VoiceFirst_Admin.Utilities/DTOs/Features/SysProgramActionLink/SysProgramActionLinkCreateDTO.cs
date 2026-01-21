using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink
{
    public class SysProgramActionLinkCreateDTO
    {
        public int ProgramId { get; set; }
        public int ActionId { get; set; }
        public int CreatedUser { get; set; } 
    }
}
