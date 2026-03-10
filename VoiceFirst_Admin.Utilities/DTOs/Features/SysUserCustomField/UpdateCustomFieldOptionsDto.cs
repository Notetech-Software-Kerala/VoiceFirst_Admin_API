using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField
{
    public class UpdateCustomFieldOptionsDto
    {
        public int CustomFieldOptionsId { get; set; }
        public string? label { get; set; }
        public string? value { get; set; }
        public bool? Active { get; set; }
    }
}
