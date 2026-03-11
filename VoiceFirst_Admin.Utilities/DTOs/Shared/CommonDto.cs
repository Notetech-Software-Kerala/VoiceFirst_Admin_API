using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Shared
{
    public class CommonDto : PartialCommonDto
    {
        public bool? Deleted { get; set; }
        public string? DeletedUser { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
    public class PartialCommonDto
    {
        public bool? Active { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUser { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUser { get; set; }
    }
}
