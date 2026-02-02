using System;
using VoiceFirst_Admin.Utilities.DTOs.Features.PlanProgramActoinLink;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Plan
{
    public class PlanDetailDto: PlanDto
    {
        public List<ProgramPlanDetailDto> ProgramPlanDetails { get; set; } = new();


    }
}
