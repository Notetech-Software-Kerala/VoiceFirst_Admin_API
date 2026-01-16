using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction
{
    public class ProgramActionDto
    {
        public int ActionId { get; set; }
        public string? ActionName { get; set; }
        public bool? IsActive { get; set; }
    }
}
