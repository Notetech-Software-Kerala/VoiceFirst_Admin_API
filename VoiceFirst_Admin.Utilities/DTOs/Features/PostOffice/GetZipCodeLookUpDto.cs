using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice
{
    public class GetPostOfficeIdZipCodeLookUpDto
    {
        public int PostOfficeId { get; set; }
        public int? PlaceId { get; set; }
    }
    public class GetPostOfficeIdsZipCodeLookUpDto
    {
        public List<int> PostOfficeIds { get; set; }
        public int? PlaceId { get; set; }
    }
}
