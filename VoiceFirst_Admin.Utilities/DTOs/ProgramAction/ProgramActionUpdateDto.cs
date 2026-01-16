using System;

namespace VoiceFirst_Admin.Utilities.DTOs.ProgramAction;

public class ProgramActionUpdateDto
{
 public int SysProgramActionId { get; set; }
 public string? ProgramActionName { get; set; }
 public bool? IsActive { get; set; }
 public int? UpdatedBy { get; set; }
}
