using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice
{
    public class GetZipCodeLookUpDto
    {
        public int PostOfficeId { get; set; }
        public int? PlaceId { get; set; }
    }
}
