using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class Place : BaseModel
{
    public int PlaceId { get; set; }
    public string PlaceName { get; set; } = string.Empty;
    public int? PostOfficeId { get; set; }
    public int? CountryId { get; set; }
    public int? DivisionOneId { get; set; }
    public int? DivisionTwoId { get; set; }
    public int? DivisionThreeId { get; set; }
}
