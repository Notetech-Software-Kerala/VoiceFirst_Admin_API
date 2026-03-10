using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField
{
    public class CustomFieldValidationsDto : PartialCommonDto
    {
        public int CustomFieldValidationId { get; set; }
        public int CustomFieldId { get; set; }
        public string RuleName { get; set; }
        public string RuleValue { get; set; }
        public string message { get; set; }
    }
}
