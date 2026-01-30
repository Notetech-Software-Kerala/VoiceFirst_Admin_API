using System;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Plan
{
    public class PlanDetailDto
    {
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

         
    }
}
