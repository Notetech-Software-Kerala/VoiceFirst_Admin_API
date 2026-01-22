using System.Collections.Generic;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.PlanProgramActoinLink
{
    public class ProgramPlanDetailDto
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public List<ProgramActionPlanDetailDto> Action { get; set; } = new();
    }
}
