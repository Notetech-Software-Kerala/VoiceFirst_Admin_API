using AutoMapper;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueMediaType;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping
{
    public class SysIssueMediaTypeProfile : Profile
    {
        public SysIssueMediaTypeProfile()
        {
            CreateMap<SysIssueMediaTypeCreateDTO, SysIssueMediaType>().ForMember(d => d.IssueMediaType, o => o.MapFrom(s => s.IssueMediaType)).ForMember(d => d.IsActive, o => o.MapFrom(s => true));
            CreateMap<(SysIssueMediaTypeUpdateDTO, int Id, int UserId), SysIssueMediaType>().ForMember(d => d.IssueMediaType, o => o.MapFrom(s => s.Item1.IssueMediaType)).ForMember(d => d.SysIssueMediaTypeId, o => o.MapFrom(s => s.Id)).ForMember(d => d.IsActive, o => o.MapFrom(s => s.Item1.Active)).ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UserId));
            CreateMap<SysIssueMediaType, SysIssueMediaTypeDTO>().ForMember(d => d.IssueMediaTypeId, o => o.MapFrom(s => s.SysIssueMediaTypeId)).ForMember(d => d.IssueMediaType, o => o.MapFrom(s => s.IssueMediaType)).ForMember(d => d.Active, o => o.MapFrom(s => s.IsActive)).ForMember(d => d.Deleted, o => o.MapFrom(s => s.IsDeleted)).ForMember(d => d.CreatedUser, o => o.MapFrom(s => s.CreatedUserName)).ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedAt)).ForMember(d => d.ModifiedUser, o => o.MapFrom(s => s.UpdatedUserName)).ForMember(d => d.ModifiedDate, o => o.MapFrom(s => s.UpdatedAt)).ForMember(d => d.DeletedUser, o => o.MapFrom(s => s.DeletedUserName)).ForMember(d => d.DeletedDate, o => o.MapFrom(s => s.DeletedAt));
            CreateMap<SysIssueMediaType, SysIssueMediaTypeActiveDTO>().ForMember(d => d.IssueMediaTypeId, o => o.MapFrom(s => s.SysIssueMediaTypeId)).ForMember(d => d.IssueMediaType, o => o.MapFrom(s => s.IssueMediaType));
        }
    }
}
