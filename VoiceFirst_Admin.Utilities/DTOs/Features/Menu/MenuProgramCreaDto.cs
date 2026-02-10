using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Menu;
public class MenuProgramCreateDto
{
    public int? ProgramId { get; set; }
    public bool Primary { get; set; }
}
public class MenuProgramUpdateDto
{
    public int? ProgramId { get; set; }
    public bool Primary { get; set; }
    public bool Active { get; set; }
    
}
