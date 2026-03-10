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
        public string FieldDataType { get; set; }
        public IEnumerable<UserCustomFieldValidationsDto> Validations { get; set; }
        public IEnumerable<UserCustomFieldOptionsDto> Options { get; set; }
    }
}
