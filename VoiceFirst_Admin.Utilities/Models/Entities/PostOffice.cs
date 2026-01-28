using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class PostOffice : BaseModel
{
    public int PostOfficeId { get; set; }
    public string PostOfficeName { get; set; } = string.Empty;
    public string? CountryName { get; set; } = string.Empty;
    public int? CountryId { get; set; }
    public int? DivisionOneId { get; set; }
    public int? DivisionTwoId { get; set; }
    public int? DivisionThreeId { get; set; }
    public string? DivisionOneName { get; set; }
    public string? DivisionTwoName { get; set; }
    public string? DivisionThreeName { get; set; }
   
}
