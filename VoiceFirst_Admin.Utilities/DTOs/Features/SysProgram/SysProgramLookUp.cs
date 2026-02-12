using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram
{
    public class SysProgramLookUp : ProgramLookUp
    {
       
        //public string Label { get; set; } = string.Empty;
        //public string Route { get; set; } = string.Empty;
        //public string PlatformName { get; set; } = string.Empty;
        //public string CompanyName { get; set; } = string.Empty;        
        public List<SysProgramActionLinkLookUp> Action { get; set; } = new();
    }
    public class ProgramLookUp
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
    }
}
