using System;
using System.Collections.Generic;
using VoiceFirst_Admin.Utilities.Models;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class Country : BaseModel
{
    public int CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public string? DivisionOneName { get; set; }
    public string? DivisionTwoName { get; set; }
    public string? DivisionThreeName { get; set; }
    public string CountryDialCode { get; set; } = string.Empty; // NVARCHAR(6)
    public string CountryIsoAlphaTwo { get; set; } = string.Empty; // CHAR(2)
}
