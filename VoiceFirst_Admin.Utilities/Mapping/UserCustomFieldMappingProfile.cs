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
        }
    }
}
