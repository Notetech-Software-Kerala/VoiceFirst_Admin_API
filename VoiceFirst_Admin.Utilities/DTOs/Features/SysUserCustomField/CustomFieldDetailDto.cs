using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField
{
    public class CustomFieldDetailDto : CommonDto
    {
        public int CustomFieldId { get; set; }
        public string FieldName { get; set; }
        public string FieldKey { get; set; }
        public List<CustomFieldDataTypeDetailsDto>? FieldDataTypes { get; set; }

    }
    public class CustomFieldDataTypeDetailsDto : PartialCommonDto
    {
        public int CustomFieldLinkId { get; set; }
        public int FieldDataTypeId { get; set; }
        public int CustomFieldId { get; set; }
        public string FieldDataType { get; set; }
        public string ValueDataType { get; set; }
        public IEnumerable<CustomFieldValidationsDto> Validations { get; set; }
        public IEnumerable<CustomFieldOptionsDto> Options { get; set; }
    }
}
