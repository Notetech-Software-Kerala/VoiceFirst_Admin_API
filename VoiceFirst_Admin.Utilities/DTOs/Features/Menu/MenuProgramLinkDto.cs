using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Menu
{
    public class MenuProgramLinkDto
    {
        public int MenuProgramLinkId { get; set; }
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string? Route { get; set; }
        public bool? Primary { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUser { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUser { get; set; }
    }
}
