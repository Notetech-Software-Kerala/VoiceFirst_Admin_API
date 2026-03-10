using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField
{
    public class CustomFieldCreateDto
    {
        [Required(ErrorMessage = "Field name is required.")]
        public string FieldName { get; set; }

        [Required(ErrorMessage = "Field key is required.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Field key can contain only letters, numbers, and underscore. Spaces are not allowed.")]
        public string FieldKey { get; set; }

        [Required(ErrorMessage = "Field data type is required.")]
        public string FieldDataType { get; set; }
        public IEnumerable<CreateCustomFieldValidationsDto>? AddValidations { get; set; }
        public IEnumerable<CreateCustomFieldOptionsDto>? AddOptions { get; set; }
    }


}
