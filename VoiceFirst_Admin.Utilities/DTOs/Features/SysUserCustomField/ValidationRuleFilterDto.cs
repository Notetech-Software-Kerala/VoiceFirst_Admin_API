using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField
{
    public class ValidationRuleFilterDto : BasicFilterDto
    {
        [Required(ErrorMessage = "FieldDataTypeId is required.")]
        public int FieldDataTypeId { get; set; }
    }
}
