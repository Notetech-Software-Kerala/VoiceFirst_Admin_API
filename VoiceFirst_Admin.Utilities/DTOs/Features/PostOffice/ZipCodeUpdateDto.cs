using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice
{
    public class ZipCodeUpdateDto
    {
        public int? ZipCodeId { get; set; }
        public string ZipCode { get; set; } = string.Empty;
        public bool? Active { get; set; }
    }
}
