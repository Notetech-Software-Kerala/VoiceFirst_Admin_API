using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class MenuProgramLink 
{
    public int MenuProgramLinkId { get; set; }
    public int MenuMasterId { get; set; }
    public int ProgramId { get; set; }
    public string ProgramName { get; set; }
    public string ProgramRoute { get; set; }
    public bool? IsPrimaryProgram { get; set; }
    public bool? IsActive { get; set; }
    public int CreatedBy { get; set; }
    public string? CreatedUserName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public string? UpdatedUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
