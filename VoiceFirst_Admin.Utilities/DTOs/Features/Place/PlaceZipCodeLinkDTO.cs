using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Place
{
    public class PlaceZipCodeLinkDTO
    {
        public int PlaceZipCodeLinkId { get; set; }
        public int ZipCodeId { get; set; }
        public string ZipCode { get; set; } = string.Empty;
        public bool Active { get; set; }      
        public string CreatedUser { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string ModifiedUser { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
    }
}
