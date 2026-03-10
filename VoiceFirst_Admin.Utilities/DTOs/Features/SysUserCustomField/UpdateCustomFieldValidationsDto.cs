using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField
{
    public class UpdateCustomFieldValidationsDto
    {
        public int CustomFieldValidationId { get; set; }
        public string? RuleName { get; set; }
        public string? RuleValue { get; set; }
        public string? message { get; set; }
        public bool? Active { get; set; }
    }
}
