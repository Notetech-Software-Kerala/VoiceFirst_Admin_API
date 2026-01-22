using System.Collections.Generic;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Plan
{
    public class PlanUpdateDto
    {
        public string? PlanName { get; set; }
        public bool? Active { get; set; }
        public List<VoiceFirst_Admin.Utilities.DTOs.Features.PlanProgramActoinLink.PlanProgramActionLinkUpdateDto>? ActionLinks { get; set; }
    }
}
