using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Place
{
    public class PlaceUpdateDTO
    {

        public string? PlaceName { get; set; } = string.Empty;
        public bool? Active { get; set; }
        public List<PlaceZipCodeLinkUpdateDTO>? UpdateZipCodeLinkIds { get; set; }
        public List<int>? InsertZipCodeLinkIds { get; set; }
    }
}
