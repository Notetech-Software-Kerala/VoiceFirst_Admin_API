using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities
{
    public class SysUserCustomFieldDataTypeLink : BaseModel
    {
        public int SysUserCustomFieldDataTypeLinkId { get; set; }
        public int SysUserCustomFieldId { get; set; }
        public string FieldName { get; set; }
        public int SysUserCustomFieldDataTypeId { get; set; }
        public string FieldDataType { get; set; }
        public string ValueDataType { get; set; }
    }
}
