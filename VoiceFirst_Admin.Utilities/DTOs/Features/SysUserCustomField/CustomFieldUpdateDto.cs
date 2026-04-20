using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField
{
    public class CustomFieldUpdateDto 
    {
        public string? FieldName { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Rule name can contain only letters, numbers, and underscore. Spaces are not allowed.")]
        public string? FieldKey { get; set; }
        public bool? Active { get; set; }
        public List<UpdateCustomFieldDataTypeDto>? UpdateCustomFieldDataTypes { get; set; }
        public List<CustomFieldDataTypeDto>? addCustomFieldDataTypes { get; set; }

    }
    public class UpdateCustomFieldDataTypeDto
    {
        public int CustomFieldLinkId { get; set; }
        //public int FieldDataTypeId { get; set; }
        public ValueDataType? ValueDataType { get; set; }
        public bool? Active { get; set; }
        public IEnumerable<CreateCustomFieldValidationsDto>? AddValidations { get; set; }
        public IEnumerable<CreateCustomFieldOptionsDto>? AddOptions { get; set; }
        public IEnumerable<UpdateCustomFieldValidationsDto>? UpdateValidations { get; set; }
        public IEnumerable<UpdateCustomFieldOptionsDto>? UpdateOptions { get; set; }
    }
}
