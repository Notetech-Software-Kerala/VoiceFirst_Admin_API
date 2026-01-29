using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Place
{
    public class PlaceDTO
    {
        public int PlaceId { get; set; }
        public string PlaceName { get; set; } = string.Empty;
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string CreatedUser { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string ModifiedUser { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string DeletedUser { get; set; } = string.Empty;
        public DateTime? DeletedDate { get; set; }
    }
}
