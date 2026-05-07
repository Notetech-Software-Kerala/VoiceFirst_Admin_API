using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField
{
    public class DataTypeLookUpDto
    {
        public int FieldDataTypeId { get; set; }
        public string FieldDataType { get; set; }
        public bool? IncludesOptions { get; set; }
        public List<string> AllowedValueDataTypes { get; set; }
    }
}
