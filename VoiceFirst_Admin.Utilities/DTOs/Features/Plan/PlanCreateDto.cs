using System.Collections.Generic;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Plan
{
    public class PlanCreateDto
    {
        public string PlanName { get; set; } = string.Empty;
        public List<int> ProgramActionLinkIds { get; set; } = new();
    }
}
