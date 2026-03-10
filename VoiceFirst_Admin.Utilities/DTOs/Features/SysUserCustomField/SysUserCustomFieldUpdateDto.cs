using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField
{
    public class SysUserCustomFieldUpdateDto : UserCustomFieldCreateDto
    {
        public string? FieldName { get; set; }
        public string? FieldKey { get; set; }
        public string? FieldDataType { get; set; }
        public bool? Active { get; set; }
        public IEnumerable<CustomFieldValidationsDto>? AddValidations { get; set; }
        public IEnumerable<CustomFieldOptionsDto>? AddOptions { get; set; }
        public IEnumerable<UpdateCustomFieldValidationsDto>? UpdateValidations { get; set; }
        public IEnumerable<UpdateCustomFieldOptionsDto> UpdateOptions { get; set; }
    }
}
