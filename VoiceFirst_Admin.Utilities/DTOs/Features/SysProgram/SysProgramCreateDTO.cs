using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram
{
    public class SysProgramCreateDTO
    {
        public string ProgramName { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public int PlatformId { get; set; }
        public int? CompanyId { get; set; }
        public List<int> ActionIds { get; set; } = new();
    }
}
