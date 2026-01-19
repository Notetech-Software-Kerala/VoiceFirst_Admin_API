using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction
{
    public class ProgramActionDto
    {
        public int ActionId { get; set; }
        public string? ActionName { get; set; }
        public bool? Active { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUser { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUser { get; set; }
        public string? DeletedUser { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
