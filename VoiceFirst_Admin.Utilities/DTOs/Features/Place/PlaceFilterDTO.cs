using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Place
{
    public class PlaceFilterDTO : CommonFilterDto
    {
        public PlaceSearchBy? SearchBy { get; set; }
        public List<int>? CountryIds { get; set; }
        public List<int>? DivisionOneIds { get; set; }
        public List<int>? DivisionTwoIds { get; set; }
        public List<int>? DivisionThreeIds { get; set; }
        public List<int>? PostOfficeIds { get; set; }
        public List<int>? ZipCodeLinkIds { get; set; }
    }
}
public enum PlaceSearchBy
{
    PlaceName,
    CreatedUser,
    ModifiedUser
}

