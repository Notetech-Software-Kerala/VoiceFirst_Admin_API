using System;
using System.Collections.Generic;
using System.Linq;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram
{
    public class SysProgramCreateDTO
    {
        public string ProgramName { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public int PlatformId { get; set; }
        public int? CompanyId { get; set; }

        private List<int> _actionIds = new();

        public List<int> ActionIds
        {
            get => _actionIds;
            set => _actionIds = value?
                .Distinct()
                .ToList()
                ?? new List<int>();
        }
    }
}
