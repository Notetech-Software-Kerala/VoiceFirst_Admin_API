using System;
using VoiceFirst_Admin.Utilities.Models;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class MenuProgramLink : BaseModel
{
    public int MenuProgramLinkId { get; set; }
    public int MenuMasterId { get; set; }
    public int ProgramId { get; set; }
    public bool IsPrimaryProgram { get; set; }
}
