using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField
{
    public class UpdateCustomFieldValidationsDto
    {
        public int CustomFieldValidationId { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Rule name can contain only letters, numbers, and underscore. Spaces are not allowed.")]
        public string? RuleName { get; set; }
        public string? RuleValue { get; set; }
        public string? message { get; set; }
        public bool? Active { get; set; }
    }
}
