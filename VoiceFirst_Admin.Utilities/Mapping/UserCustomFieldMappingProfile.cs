using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueCharacterType;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping
{
    public class UserCustomFieldMappingProfile :Profile
    {
        public UserCustomFieldMappingProfile()
        {
            CreateMap<SysUserCustomField, SysUserCustomFieldDetailDto>()
                .ForMember(d => d.CustomFieldId, o => o.MapFrom(s => s.SysUserCustomFieldId))
                .ForMember(d => d.FieldName, o => o.MapFrom(s => s.FieldName))
                .ForMember(d => d.FieldName, o => o.MapFrom(s => s.FieldName))
                .ForMember(d => d.FieldKey, o => o.MapFrom(s => s.FieldKey))
                .ForMember(d => d.FieldDataType, o => o.MapFrom(s => s.FieldDataType))
                .ForMember(d => d.Active, o => o.MapFrom(s => s.IsActive))
                .ForMember(d => d.Deleted, o => o.MapFrom(s => s.IsDeleted))
                .ForMember(d => d.CreatedUser, o => o.MapFrom(s => s.CreatedUserName))
                .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.ModifiedUser, o => o.MapFrom(s => s.UpdatedUserName))
                .ForMember(d => d.ModifiedDate, o => o.MapFrom(s => s.UpdatedAt))
                .ForMember(d => d.DeletedUser, o => o.MapFrom(s => s.DeletedUserName))
                .ForMember(d => d.DeletedDate, o => o.MapFrom(s => s.DeletedAt));
            CreateMap<SysUserCustomField, SysUserCustomFieldDto>()
                .ForMember(d => d.CustomFieldId, o => o.MapFrom(s => s.SysUserCustomFieldId))
                .ForMember(d => d.FieldName, o => o.MapFrom(s => s.FieldName))
                .ForMember(d => d.FieldName, o => o.MapFrom(s => s.FieldName))
                .ForMember(d => d.FieldKey, o => o.MapFrom(s => s.FieldKey))
                .ForMember(d => d.FieldDataType, o => o.MapFrom(s => s.FieldDataType))
                .ForMember(d => d.Active, o => o.MapFrom(s => s.IsActive))
                .ForMember(d => d.Deleted, o => o.MapFrom(s => s.IsDeleted))
                .ForMember(d => d.CreatedUser, o => o.MapFrom(s => s.CreatedUserName))
                .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.ModifiedUser, o => o.MapFrom(s => s.UpdatedUserName))
                .ForMember(d => d.ModifiedDate, o => o.MapFrom(s => s.UpdatedAt))
                .ForMember(d => d.DeletedUser, o => o.MapFrom(s => s.DeletedUserName))
                .ForMember(d => d.DeletedDate, o => o.MapFrom(s => s.DeletedAt));

            CreateMap<UserCustomFieldCreateDto, SysUserCustomField>()
        .ForMember(d => d.FieldName, o => o.MapFrom(s => s.FieldName))
        .ForMember(d => d.FieldKey, o => o.MapFrom(s => s.FieldKey))
        .ForMember(d => d.FieldDataType, o => o.MapFrom(s => s.FieldDataType));

            CreateMap<SysUserCustomFieldUpdateDto, SysUserCustomField>()
        .ForMember(d => d.FieldName, o => o.MapFrom(s => s.FieldName))
        .ForMember(d => d.FieldKey, o => o.MapFrom(s => s.FieldKey))
        .ForMember(d => d.IsActive, o => o.MapFrom(s => s.Active))
        .ForMember(d => d.FieldDataType, o => o.MapFrom(s => s.FieldDataType));

            CreateMap<CustomFieldOptionsDto, SysUserCustomFieldOptions>()
                .ForMember(d => d.label, o => o.MapFrom(s => s.label))
                .ForMember(d => d.value, o => o.MapFrom(s => s.value));

            CreateMap<CustomFieldValidationsDto, SysUserCustomFieldValidations>()
                .ForMember(d => d.RuleName, o => o.MapFrom(s => s.RuleName))
                .ForMember(d => d.RuleValue, o => o.MapFrom(s => s.RuleValue))
                .ForMember(d => d.message, o => o.MapFrom(s => s.message));

            CreateMap<UpdateCustomFieldOptionsDto, SysUserCustomFieldOptions>()
                .ForMember(d => d.SysUserCustomFieldOptionsId, o => o.MapFrom(s => s.CustomFieldOptionsId))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => s.Active))
                .ForMember(d => d.label, o => o.MapFrom(s => s.label))
                .ForMember(d => d.value, o => o.MapFrom(s => s.value));

            CreateMap<UpdateCustomFieldValidationsDto, SysUserCustomFieldValidations>()
                .ForMember(d => d.SysUserCustomFieldValidationId, o => o.MapFrom(s => s.CustomFieldValidationId))
                .ForMember(d => d.RuleName, o => o.MapFrom(s => s.RuleName))
                .ForMember(d => d.RuleValue, o => o.MapFrom(s => s.RuleValue))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => s.Active))
                .ForMember(d => d.message, o => o.MapFrom(s => s.message));

            CreateMap<SysUserCustomFieldValidations, UserCustomFieldValidationsDto>()
                .ForMember(d => d.CustomFieldValidationId, o => o.MapFrom(s => s.SysUserCustomFieldValidationId))
                .ForMember(d => d.CustomFieldId, o => o.MapFrom(s => s.SysUserCustomFieldId))
                .ForMember(d => d.RuleName, o => o.MapFrom(s => s.RuleName))
                .ForMember(d => d.RuleValue, o => o.MapFrom(s => s.RuleValue))
                .ForMember(d => d.Active, o => o.MapFrom(s => s.IsActive))
                .ForMember(d => d.CreatedUser, o => o.MapFrom(s => s.CreatedUserName))
                .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.ModifiedUser, o => o.MapFrom(s => s.UpdatedUserName))
                .ForMember(d => d.ModifiedDate, o => o.MapFrom(s => s.UpdatedAt))
                .ForMember(d => d.message, o => o.MapFrom(s => s.message));
            CreateMap<SysUserCustomFieldOptions, UserCustomFieldOptionsDto>()
                .ForMember(d => d.CustomFieldOptionsId, o => o.MapFrom(s => s.SysUserCustomFieldOptionsId))
                .ForMember(d => d.CustomFieldId, o => o.MapFrom(s => s.SysUserCustomFieldId))
                .ForMember(d => d.label, o => o.MapFrom(s => s.label))
                .ForMember(d => d.value, o => o.MapFrom(s => s.value))
                .ForMember(d => d.Active, o => o.MapFrom(s => s.IsActive))
                .ForMember(d => d.CreatedUser, o => o.MapFrom(s => s.CreatedUserName))
                .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.ModifiedUser, o => o.MapFrom(s => s.UpdatedUserName))
                .ForMember(d => d.ModifiedDate, o => o.MapFrom(s => s.UpdatedAt));


        }
    }
}
