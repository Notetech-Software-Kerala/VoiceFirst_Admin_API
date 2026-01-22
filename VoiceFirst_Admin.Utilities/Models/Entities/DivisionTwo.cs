using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class DivisionTwo 
{
    public int DivisionTwoId { get; set; }
    public string DivisionTwoName { get; set; } = string.Empty;
    public string DivisionOneName { get; set; } = string.Empty;
    public int DivisionOneId { get; set; }

    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
}
