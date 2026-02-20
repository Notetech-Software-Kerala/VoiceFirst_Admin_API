using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Place
{
    public class PlaceFilterDTO : CommonFilterDto
    {
        public PlaceSearchBy? SearchBy { get; set; }
        public int? CountryId { get; set; }
        public int? DivisionOneId { get; set; }
        public int? DivisionTwoId { get; set; }
        public int? DivisionThreeId { get; set; }
        public int? PostOfficeId { get; set; }
        public int? ZipCodeLinkId { get; set; }
    }
}
public enum PlaceSearchBy
{
    PlaceName,
    CreatedUser,
    ModifiedUser,
    DeletedUser
}

