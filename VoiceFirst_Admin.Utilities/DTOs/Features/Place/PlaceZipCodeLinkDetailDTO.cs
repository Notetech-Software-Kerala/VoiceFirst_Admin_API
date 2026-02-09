using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Place
{
    public class PlaceZipCodeLinkDetailDTO
    {
        public int PostOfficeId { get; set; }
        public string PostOfficeName { get; set; } = string.Empty;
        public List<PlaceZipCodeLinkDTO> ZipCodes { get; set; }

    }
}
