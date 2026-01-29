using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities
{
    public class PlacePostOfiiceLink:BaseModel
    {
        public int PlacePostOfiiceLinkId { get; set; }
        public int? PlaceId { get; set; }
        public int? PostOfficeId { get; set; }
        public int? CountryId { get; set; }
        public int? DivisionOneId { get; set; }
        public int? DivisionTwoId { get; set; }
        public int? DivisionThreeId { get; set; }
    }
}
