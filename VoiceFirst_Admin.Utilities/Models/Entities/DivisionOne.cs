using System;
using VoiceFirst_Admin.Utilities.Models;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class DivisionOne : BaseModel
{
    public int DivisionOneId { get; set; }
    public string DivisionOneName { get; set; } = string.Empty;
    public int CountryId { get; set; }
}
