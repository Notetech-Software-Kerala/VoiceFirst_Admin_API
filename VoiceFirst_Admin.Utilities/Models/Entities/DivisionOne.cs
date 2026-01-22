using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class DivisionOne
{
    public int DivisionOneId { get; set; }
    public string DivisionOneName { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
}
