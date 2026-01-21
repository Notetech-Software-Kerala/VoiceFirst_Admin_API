using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink
{
    public class SysProgramActionLinkDTO
    {
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public bool Active { get; set; }
        public string CreatedUser { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string ModifiedUser { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
       
    }
}
