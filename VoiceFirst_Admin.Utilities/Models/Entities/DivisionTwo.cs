using System;
using VoiceFirst_Admin.Utilities.Models;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class DivisionTwo : BaseModel
{
    public int DivisionTwoId { get; set; }
    public string DivisionTwoName { get; set; } = string.Empty;
    public int DivisionOneId { get; set; }
}
