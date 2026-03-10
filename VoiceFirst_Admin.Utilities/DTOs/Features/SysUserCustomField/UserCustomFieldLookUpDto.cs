using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField
{
    public class UserCustomFieldLookUpDto 
    {
        public int CustomFieldId { get; set; }
        public string FieldName { get; set; }
        public string FieldDataType { get; set; }
    }
}
