using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class Place : BaseModel
{
    public int PlaceId { get; set; }
    public string PlaceName { get; set; } = string.Empty;
   
}
