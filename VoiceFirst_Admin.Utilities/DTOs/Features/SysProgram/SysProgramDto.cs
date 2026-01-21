using System;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram
{
    public class SysProgramDto
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public int PlatformId { get; set; } 
        public int CompanyId { get; set; }
        public bool Active { get; set; }
        public bool Delete { get; set; }
        public string CreatedUser { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string ModifiedUser { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string DeletedUser { get; set; } = string.Empty;
        public DateTime? DeletedDate { get; set; }
        public List<SysProgramActionLinkDTO> Action { get; set; } = new();
    }
}
