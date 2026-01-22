using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.PlanProgramActoinLink
{
    public class PlanProgramActionLinkDto
    {
        public int PlanActionLinkId { get; set; }
        public int PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string CreatedUser { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string ModifiedUser { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string DeletedUser { get; set; } = string.Empty;
        public DateTime? DeletedDate { get; set; }

        public List<SysProgramLookUp> Programs { get; set; } = new List<SysProgramLookUp>();
    }
}
