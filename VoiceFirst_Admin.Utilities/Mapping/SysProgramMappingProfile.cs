using AutoMapper;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping
{
    public class SysProgramMappingProfile : Profile
    {
        public SysProgramMappingProfile()
        {
            CreateMap<SysProgram, SysProgramDto>()
 
              .ForMember(d => d.ProgramId, opt => opt.MapFrom(s => s.SysProgramId))
              .ForMember(d => d.ProgramName, opt => opt.MapFrom(s => s.ProgramName))
              .ForMember(d => d.Label, opt => opt.MapFrom(s => s.LabelName))
              .ForMember(d => d.Route, opt => opt.MapFrom(s => s.ProgramRoute))
              .ForMember(d => d.PlatformId, opt => opt.MapFrom(s => s.ApplicationId))
              .ForMember(d => d.CompanyId, opt => opt.MapFrom(s => s.CompanyId))
              .ForMember(d => d.Active, o => o.MapFrom(s => s.IsActive))
              .ForMember(d => d.Deleted, o => o.MapFrom(s => s.IsDeleted))
              .ForMember(d => d.CreatedUser, o => o.MapFrom(s => s.CreatedUserName))
              .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedAt))
              .ForMember(d => d.ModifiedUser, o => o.MapFrom(s => s.UpdatedUserName))
              .ForMember(d => d.ModifiedDate, o => o.MapFrom(s => s.UpdatedAt))
              .ForMember(d => d.DeletedUser, o => o.MapFrom(s => s.DeletedUserName))
              .ForMember(d => d.DeletedDate, o => o.MapFrom(s => s.DeletedAt))
             ;

          

            CreateMap<(SysProgramUpdateDTO,int programId,int updatingUserId), SysProgram>()
                .ForMember(d => d.SysProgramId, opt => opt.MapFrom(s => s.programId))
                .ForMember(d => d.ProgramName, opt => opt.MapFrom(s => s.Item1.ProgramName ?? string.Empty))
                .ForMember(d => d.LabelName, opt => opt.MapFrom(s => s.Item1.Label ?? string.Empty))
                .ForMember(d => d.ProgramRoute, opt => opt.MapFrom(s => s.Item1.Route ?? string.Empty))
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(s => s.Item1.CompanyId ?? 0))
                .ForMember(d => d.UpdatedBy, opt => opt.MapFrom(s => s.updatingUserId))
                .ForMember(d => d.UpdatedBy, opt => opt.MapFrom(s => s.Item1.Active))
                .ForMember(d => d.ApplicationId, opt => opt.MapFrom(s => s.Item1.PlatformId ?? 0));

            CreateMap<SysProgramCreateDTO, SysProgram>()
                .ForMember(d => d.ProgramName, opt => opt.MapFrom(s => s.ProgramName))
                .ForMember(d => d.LabelName, opt => opt.MapFrom(s => s.Label))
                .ForMember(d => d.ProgramRoute, opt => opt.MapFrom(s => s.Route))
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(s => s.CompanyId))
                .ForMember(d => d.ApplicationId, opt => opt.MapFrom(s => s.PlatformId));
        }
    }
}
