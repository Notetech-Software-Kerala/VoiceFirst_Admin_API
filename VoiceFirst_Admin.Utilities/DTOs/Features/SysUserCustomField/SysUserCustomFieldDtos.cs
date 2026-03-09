using System.Collections.Generic;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField
{
    public class UserCustomFieldCreateDto
    {
        public string FieldName { get; set; }
        public string FieldKey { get; set; }
        public string FieldDataType { get; set; }
        public IEnumerable<CustomFieldValidationsDto>? AddValidations { get; set; }
        public IEnumerable<CustomFieldOptionsDto>? AddOptions { get; set; }
    }

    public class SysUserCustomFieldFilterDto : VoiceFirst_Admin.Utilities.DTOs.Shared.CommonFilterDto
    {
        public SysUserCustomFieldSearchBy? SearchBy { get; set; }
    }

    public enum SysUserCustomFieldSearchBy
    {
        FieldName,
        FieldKey,
        FieldDataType,
        CreatedUser,
        UpdatedUser,
        DeletedUser
    }
    public class CustomFieldValidationsDto
    {
        public string RuleName { get; set; }
        public string RuleValue { get; set; }
        public string message { get; set; }
    }
    public class CustomFieldOptionsDto
    {
        public string label { get; set; }
        public string value { get; set; }
    }
    public class UpdateCustomFieldValidationsDto 
    {
        public int CustomFieldValidationId { get; set; }
        public string? RuleName { get; set; }
        public string? RuleValue { get; set; }
        public string? message { get; set; }
        public bool? Active { get; set; }
    }
    public class UpdateCustomFieldOptionsDto 
    {
        public int CustomFieldOptionsId { get; set; }
        public string? label { get; set; }
        public string? value { get; set; }
        public bool? Active { get; set; }
    }
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

    public class SysUserCustomFieldDetailDto
    {
        public int CustomFieldId { get; set; }
        public string FieldName { get; set; }
        public string FieldKey { get; set; }
        public string FieldDataType { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string CreatedUser { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string ModifiedUser { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string DeletedUser { get; set; } = string.Empty;
        public DateTime? DeletedDate { get; set; }
        public IEnumerable<SysUserCustomFieldValidations> Validations { get; set; }
        public IEnumerable<SysUserCustomFieldOptions> Options { get; set; }
    }
}
