using AutoMapper;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueMediaFormat;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping
{
    public class SysIssueMediaFormatProfile : Profile
    {
        public SysIssueMediaFormatProfile()
        {
            CreateMap<SysIssueMediaFormatCreateDTO, SysIssueMediaFormat>().ForMember(d => d.IssueMediaFormat, o => o.MapFrom(s => s.IssueMediaFormat)).ForMember(d => d.IsActive, o => o.MapFrom(s => true));
            CreateMap<(SysIssueMediaFormatUpdateDTO, int Id, int UserId), SysIssueMediaFormat>().ForMember(d => d.IssueMediaFormat, o => o.MapFrom(s => s.Item1.IssueMediaFormat)).ForMember(d => d.SysIssueMediaFormatId, o => o.MapFrom(s => s.Id)).ForMember(d => d.IsActive, o => o.MapFrom(s => s.Item1.Active)).ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UserId));
            CreateMap<SysIssueMediaFormat, SysIssueMediaFormatDTO>().ForMember(d => d.IssueMediaFormatId, o => o.MapFrom(s => s.SysIssueMediaFormatId)).ForMember(d => d.IssueMediaFormat, o => o.MapFrom(s => s.IssueMediaFormat)).ForMember(d => d.Active, o => o.MapFrom(s => s.IsActive)).ForMember(d => d.Deleted, o => o.MapFrom(s => s.IsDeleted)).ForMember(d => d.CreatedUser, o => o.MapFrom(s => s.CreatedUserName)).ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedAt)).ForMember(d => d.ModifiedUser, o => o.MapFrom(s => s.UpdatedUserName)).ForMember(d => d.ModifiedDate, o => o.MapFrom(s => s.UpdatedAt)).ForMember(d => d.DeletedUser, o => o.MapFrom(s => s.DeletedUserName)).ForMember(d => d.DeletedDate, o => o.MapFrom(s => s.DeletedAt));
            CreateMap<SysIssueMediaFormat, SysIssueMediaFormatActiveDTO>().ForMember(d => d.IssueMediaFormatId, o => o.MapFrom(s => s.SysIssueMediaFormatId)).ForMember(d => d.IssueMediaFormat, o => o.MapFrom(s => s.IssueMediaFormat));
        }
    }
}
