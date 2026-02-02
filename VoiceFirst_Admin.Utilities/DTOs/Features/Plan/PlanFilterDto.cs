using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Plan
{
    public class PlanFilterDto : CommonFilterDto
    {
        public PlanSearchBy? SearchBy { get; set; }
    }

    public enum PlanSearchBy
    {
        PlanName,
        CreatedUser,
        ModifiedUser,
        DeletedUser
    }
}
