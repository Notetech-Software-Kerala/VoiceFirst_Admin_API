using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField
{
    public class CustomFieldOptionsDto : PartialCommonDto
    {
        public int CustomFieldOptionsId { get; set; }
        public int CustomFieldLinkId { get; set; }
        public string label { get; set; }
        public string value { get; set; }
    }
}
