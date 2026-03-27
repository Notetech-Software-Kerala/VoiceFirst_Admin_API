using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities
{
    public class SysUserCustomFieldValidations : PartialBaseModel
    {
        public int SysUserCustomFieldValidationId { get; set; }
        public int SysUserCustomFieldDataTypeLinkId { get; set; }
        public int RuleId { get; set; } 
        public string RuleName { get; set; } 
        public string RuleValue { get; set; } 
        public string message { get; set; } 
    }
}
