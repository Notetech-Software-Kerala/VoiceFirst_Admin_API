using System;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction;

public class ProgramActionUpdateDto
{
     public int ActionId { get; set; }
     public string? ActionName { get; set; }
     public bool? Active { get; set; }
}
