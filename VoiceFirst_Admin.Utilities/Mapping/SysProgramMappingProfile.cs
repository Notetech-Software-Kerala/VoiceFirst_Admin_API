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
                .ForMember(d => d.ProgramName, opt => opt.MapFrom(s => s.ProgramName))
                .ForMember(d => d.Label, opt => opt.MapFrom(s => s.LabelName))
                .ForMember(d => d.Route, opt => opt.MapFrom(s => s.ProgramRoute))
                .ForMember(d => d.PlatformId, opt => opt.MapFrom(s => s.ApplicationId))
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(s => s.CompanyId))
                .ForMember(d => d.Active, opt => opt.MapFrom(s => s.IsActive ?? true));

            CreateMap<SysProgramCreateDTO, SysProgram>()
                .ForMember(d => d.ProgramName, opt => opt.MapFrom(s => s.ProgramName))
                .ForMember(d => d.LabelName, opt => opt.MapFrom(s => s.Label))
                .ForMember(d => d.ProgramRoute, opt => opt.MapFrom(s => s.Route))
                .ForMember(d => d.CompanyId, opt => opt.MapFrom(s => s.CompanyId))
                .ForMember(d => d.ApplicationId, opt => opt.MapFrom(s => s.PlatformId));
        }
    }
}
