using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Place
{
    public class PlacePostOfficeLinksDTO
    {
        public int PostOfficeId { get; set; }
        public string PostOfficeName { get; set; } = string.Empty;
        public bool Active { get; set; }      
        public string CreatedUser { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string ModifiedUser { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
      
    }
}
