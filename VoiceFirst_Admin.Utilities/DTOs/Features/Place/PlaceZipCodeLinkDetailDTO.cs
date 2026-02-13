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
        public string CountryName { get; set; } = string.Empty;
        public string? DivisionOneLabel { get; set; }
        public string? DivisionTwoLabel { get; set; }
        public string? DivisionThreeLabel { get; set; }
        public string? DivisionOneName { get; set; }
        public string? DivisionTwoName { get; set; }
        public string? DivisionThreeName { get; set; }
        public List<PlaceZipCodeLinkDTO> ZipCodes { get; set; }

    }
}
