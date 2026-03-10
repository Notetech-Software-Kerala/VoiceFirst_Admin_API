using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField
{
    public class UserCustomFieldOptionsDto : PartialCommonDto
    {
        public int CustomFieldOptionsId { get; set; }
        public int CustomFieldId { get; set; }
        public string label { get; set; }
        public string value { get; set; }
    }
}
