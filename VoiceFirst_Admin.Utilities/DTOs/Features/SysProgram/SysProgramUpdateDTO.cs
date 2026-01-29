using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram
{
    public class SysProgramUpdateDTO
    {
        public string? ProgramName { get; set; } = string.Empty;
        public string? Label { get; set; } = string.Empty;
        public string? Route { get; set; } = string.Empty;
        public int? PlatformId { get; set; }
        public int? CompanyId { get; set; }
        public bool? Active { get; set; }
        public List<SysProgramActionLinkUpdateDTO>? UpdationActions { get; set; }
        public List<int>? InsertActions { get; set; }
    }
}
