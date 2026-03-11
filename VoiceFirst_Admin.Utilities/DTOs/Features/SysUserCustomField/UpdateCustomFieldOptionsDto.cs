using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField
{
    public class UpdateCustomFieldOptionsDto
    {
        public int CustomFieldOptionsId { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Rule name can contain only letters, numbers, and underscore. Spaces are not allowed.")]
        public string? label { get; set; }
        public string? value { get; set; }
        public bool? Active { get; set; }
    }
}
