using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Country;

public class CountryDto
{
    public int CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public string? DivisionOne { get; set; }
    public string? DivisionTwo { get; set; }
    public string? DivisionThree { get; set; }
    public string DialCode { get; set; } = string.Empty;
    public string IsoAlphaTwo { get; set; } = string.Empty;
    public bool? Active { get; set; }

    public bool? Deleted { get; set; }
}
