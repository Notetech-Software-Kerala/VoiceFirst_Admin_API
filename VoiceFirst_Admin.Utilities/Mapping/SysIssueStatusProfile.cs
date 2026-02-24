using AutoMapper;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueStatus;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping
{
    public class SysIssueStatusProfile : Profile
    {
        public SysIssueStatusProfile()
        {
            CreateMap<SysIssueStatusCreateDTO, SysIssueStatus>()
                .ForMember(d => d.IssueStatus, o => o.MapFrom(s => s.IssueStatus))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => true));

            CreateMap<(SysIssueStatusUpdateDTO, int Id, int UserId), SysIssueStatus>()
                .ForMember(d => d.IssueStatus, o => o.MapFrom(s => s.Item1.IssueStatus))
                .ForMember(d => d.SysIssueStatusId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => s.Item1.Active))
                .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UserId));

            CreateMap<SysIssueStatus, SysIssueStatusDTO>()
                .ForMember(d => d.IssueStatusId, o => o.MapFrom(s => s.SysIssueStatusId))
                .ForMember(d => d.IssueStatus, o => o.MapFrom(s => s.IssueStatus))
                .ForMember(d => d.Active, o => o.MapFrom(s => s.IsActive))
                .ForMember(d => d.Deleted, o => o.MapFrom(s => s.IsDeleted))
                .ForMember(d => d.CreatedUser, o => o.MapFrom(s => s.CreatedUserName))
                .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.ModifiedUser, o => o.MapFrom(s => s.UpdatedUserName))
                .ForMember(d => d.ModifiedDate, o => o.MapFrom(s => s.UpdatedAt))
                .ForMember(d => d.DeletedUser, o => o.MapFrom(s => s.DeletedUserName))
                .ForMember(d => d.DeletedDate, o => o.MapFrom(s => s.DeletedAt));

            CreateMap<SysIssueStatus, SysIssueStatusActiveDTO>()
                .ForMember(d => d.IssueStatusId, o => o.MapFrom(s => s.SysIssueStatusId))
                .ForMember(d => d.IssueStatus, o => o.MapFrom(s => s.IssueStatus));
        }
    }
}
