using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Shared
{
    public class StatusChangeDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public bool Status { get; set; }
    }
}
