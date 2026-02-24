using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram
{
    public class SysProgramLookUpDTO : ProgramLookUp
    {          
        public List<SysProgramActionLinkLookUp> Action { get; set; } = new();
    }
    public class ProgramLookUp
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
    }
}
