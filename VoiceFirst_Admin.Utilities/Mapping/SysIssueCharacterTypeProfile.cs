using AutoMapper;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueCharacterType;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping
{
    public class SysIssueCharacterTypeProfile : Profile
    {
        public SysIssueCharacterTypeProfile()
        {
            CreateMap<SysIssueCharacterTypeCreateDTO, SysIssueCharacterType>()
                .ForMember(d => d.IssueCharacterType, o => o.MapFrom(s => s.IssueCharacterType))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => true));

            CreateMap<(SysIssueCharacterTypeUpdateDTO, int Id, int UserId), SysIssueCharacterType>()
                .ForMember(d => d.IssueCharacterType, o => o.MapFrom(s => s.Item1.IssueCharacterType))
                .ForMember(d => d.SysIssueCharacterTypeId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => s.Item1.Active))
                .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UserId));

            CreateMap<SysIssueCharacterType, SysIssueCharacterTypeDTO>()
                .ForMember(d => d.IssueCharacterTypeId, o => o.MapFrom(s => s.SysIssueCharacterTypeId))
                .ForMember(d => d.IssueCharacterType, o => o.MapFrom(s => s.IssueCharacterType))
                .ForMember(d => d.Active, o => o.MapFrom(s => s.IsActive))
                .ForMember(d => d.Deleted, o => o.MapFrom(s => s.IsDeleted))
                .ForMember(d => d.CreatedUser, o => o.MapFrom(s => s.CreatedUserName))
                .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.ModifiedUser, o => o.MapFrom(s => s.UpdatedUserName))
                .ForMember(d => d.ModifiedDate, o => o.MapFrom(s => s.UpdatedAt))
                .ForMember(d => d.DeletedUser, o => o.MapFrom(s => s.DeletedUserName))
                .ForMember(d => d.DeletedDate, o => o.MapFrom(s => s.DeletedAt));

            CreateMap<SysIssueCharacterType, SysIssueCharacterTypeActiveDTO>()
                .ForMember(d => d.IssueCharacterTypeId, o => o.MapFrom(s => s.SysIssueCharacterTypeId))
                .ForMember(d => d.IssueCharacterType, o => o.MapFrom(s => s.IssueCharacterType));
        }
    }
}
