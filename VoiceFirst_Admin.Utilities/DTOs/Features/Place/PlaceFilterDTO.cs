using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Place
{
    public class PlaceFilterDTO : CommonFilterDto
    {
        public PlaceSearchBy? SearchBy { get; set; }
    }
}
public enum PlaceSearchBy
{
    PlaceName,
    CreatedUser,
    ModifiedUser
}

