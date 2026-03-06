using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities
{
    public class SysUserCustomFieldOptions : PartialBaseModel
    {
        public int SysUserCustomFieldOptionsId { get; set; }
        public int SysUserCustomFieldId { get; set; }
        public string label { get; set; } 
        public string value { get; set; }
    }
}
