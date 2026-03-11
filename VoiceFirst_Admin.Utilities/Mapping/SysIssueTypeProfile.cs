using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping
{
    public class SysIssueTypeProfile : Profile
    {
        public SysIssueTypeProfile()
        {
            CreateMap<SysIssueTypeCreateDTO, SysIssueType>()
                .ForMember(d => d.IssueType, o => o.MapFrom(s => s.IssueType))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => true));

            CreateMap<(IssueMediaRuleCreateDTO Dto, int IssueTypeId, int LoginId), SysIssueMediaRule>()
                .ForMember(d => d.IssueTypeId, o => o.MapFrom(s => s.IssueTypeId))
                .ForMember(d => d.IssueMediaFormatId, o => o.MapFrom(s => s.Dto.IssueMediaFormatId))
                .ForMember(d => d.Min, o => o.MapFrom(s => s.Dto.Min))
                .ForMember(d => d.Max, o => o.MapFrom(s => s.Dto.Max))
                .ForMember(d => d.MaxSizeMB, o => o.MapFrom(s => s.Dto.MaxSizeMB))
                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.LoginId));

            CreateMap<(IssueMediaRuleUpdateDTO Dto, int IssueTypeId, int LoginId), SysIssueMediaRule>()
                .ForMember(d => d.IssueTypeId, o => o.MapFrom(s => s.IssueTypeId))
                .ForMember(d => d.IssueMediaFormatId, o => o.MapFrom(s => s.Dto.IssueMediaFormatId))
                .ForMember(d => d.Min, o => o.MapFrom(s => s.Dto.Min))
                .ForMember(d => d.Max, o => o.MapFrom(s => s.Dto.Max))
                .ForMember(d => d.MaxSizeMB, o => o.MapFrom(s => s.Dto.MaxSizeMB))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => s.Dto.Active))
                .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.LoginId));

            CreateMap<(SysIssueTypeUpdateDTO, int Id, int UserId), SysIssueType>()
                .ForMember(d => d.IssueType, o => o.MapFrom(s => s.Item1.IssueType))
                .ForMember(d => d.SysIssueTypeId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => s.Item1.Active))
                .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UserId));

            CreateMap<SysIssueType, SysIssueTypeDTO>()
                .ForMember(d => d.IssueTypeId, o => o.MapFrom(s => s.SysIssueTypeId))
                .ForMember(d => d.IssueType, o => o.MapFrom(s => s.IssueType))
                .ForMember(d => d.Active, o => o.MapFrom(s => s.IsActive))
                .ForMember(d => d.Deleted, o => o.MapFrom(s => s.IsDeleted))
                .ForMember(d => d.CreatedUser, o => o.MapFrom(s => s.CreatedUserName))
                .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.ModifiedUser, o => o.MapFrom(s => s.UpdatedUserName))
                .ForMember(d => d.ModifiedDate, o => o.MapFrom(s => s.UpdatedAt))
                .ForMember(d => d.DeletedUser, o => o.MapFrom(s => s.DeletedUserName))
                .ForMember(d => d.DeletedDate, o => o.MapFrom(s => s.DeletedAt));

            CreateMap<SysIssueType, SysIssueTypeActiveDTO>()
                .ForMember(d => d.IssueTypeId, o => o.MapFrom(s => s.SysIssueTypeId))
                .ForMember(d => d.IssueType, o => o.MapFrom(s => s.IssueType));
        }
    }
}
