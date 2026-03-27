using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField
{
    [UniqueFieldDataTypeCombination]
    public class CustomFieldCreateDto
        {
            [Required(ErrorMessage = "Field name is required.")]
            public string FieldName { get; set; }

            [Required(ErrorMessage = "Field key is required.")]
            [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Field key can contain only letters, numbers, and underscore. Spaces are not allowed.")]
            public string FieldKey { get; set; }

            public List<CustomFieldDataTypeDto>? AddCustomFieldDataType { get; set; }

        }
    
    public class CustomFieldDataTypeDto {
            [Required(ErrorMessage = "Field data type is required.")]
            public int FieldDataTypeId { get; set; }
            public ValueDataType ValueDataType { get; set; }
            public IEnumerable<CreateCustomFieldValidationsDto>? AddValidations { get; set; }
            public IEnumerable<CreateCustomFieldOptionsDto>? AddOptions { get; set; }
        }

    public enum ValueDataType
    {
        Varchar,    // VARCHAR(255)   - fixed length text
        NVarchar,   // NVARCHAR(MAX)  - unicode text
        Int,        // INT            - whole numbers
        Float,      // FLOAT          - decimal numbers
        Decimal,    // DECIMAL(18,2)  - precise decimal numbers
        Bit,        // BIT            - true/false (boolean)
        DateTime,   // DATETIME       - date and time
        Date,       // DATE           - date only
    }
    public class UniqueFieldDataTypeCombinationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dto = validationContext.ObjectInstance as CustomFieldCreateDto;

            if (dto?.AddCustomFieldDataType == null || !dto.AddCustomFieldDataType.Any())
                return ValidationResult.Success;

            var duplicates = dto.AddCustomFieldDataType
                .GroupBy(x => new { x.FieldDataTypeId, x.ValueDataType })
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Any())
            {
                var duplicateDetails = string.Join(", ", duplicates
                    .Select(d => $"(FieldDataTypeId: {d.FieldDataTypeId}, ValueDataType: {d.ValueDataType})"));

                return new ValidationResult($"Duplicate combinations found: {duplicateDetails}");
            }

            return ValidationResult.Success;
        }
    }

}
