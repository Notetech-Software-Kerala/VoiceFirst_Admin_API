using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Models.Entities
{
    public class ZipCodes
    {
        public int ZipCodeId { get; set; }
        public string ZipCode { get; set; }
        public int CreatedBy { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
