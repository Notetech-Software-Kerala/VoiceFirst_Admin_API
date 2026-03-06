using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities
{
    public class SysUserCustomField : BaseModel
    {
        public int SysUserCustomFieldId { get; set; }
        public string FieldName { get; set; } 
        public string FieldKey { get; set; } 
        public string FieldDataType { get; set; } 
    }
}
