using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Role
{
    public class PlanRoleActionLinkDto
    {
        
        public int ActionLinkId { get; set; }
        public string ActionName { get; set; } = string.Empty;
        public bool Active { get; set; }
        //public bool Deleted { get; set; }
        public string CreatedUser { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string ModifiedUser { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        //public string DeletedUser { get; set; } = string.Empty;
        //public DateTime? DeletedDate { get; set; }
    }
    
}
