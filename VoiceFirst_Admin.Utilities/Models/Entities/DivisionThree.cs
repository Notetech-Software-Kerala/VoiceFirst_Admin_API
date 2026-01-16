using System;
using VoiceFirst_Admin.Utilities.Models;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class DivisionThree : BaseModel
{
    public int DivisionThreeId { get; set; }
    public string DivisionThreeName { get; set; } = string.Empty;
    public int DivisionTwoId { get; set; }
}
