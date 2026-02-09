using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Place
{
    public class PlaceDetailDTO:PlaceDTO
    {
        
        public List<PlaceZipCodeLinkDetailDTO> PostOffices { get; set; }
    
    }
}
