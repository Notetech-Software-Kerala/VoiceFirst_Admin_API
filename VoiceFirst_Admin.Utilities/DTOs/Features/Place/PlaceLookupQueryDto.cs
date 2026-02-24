using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Place
{
    public class PlaceLookupQueryDto:BasicFilterDto
    {
        public int ZipCodeId { get; set; }

    }
}
