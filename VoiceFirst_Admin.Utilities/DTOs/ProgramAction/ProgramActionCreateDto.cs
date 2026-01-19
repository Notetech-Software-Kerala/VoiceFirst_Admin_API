using System;

namespace VoiceFirst_Admin.Utilities.DTOs.ProgramAction;

public class ProgramActionCreateDto
{
 public string ActionName { get; set; } = string.Empty;
 public int CreatedBy { get; set; }
}
