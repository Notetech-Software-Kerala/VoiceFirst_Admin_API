using AutoMapper;
using VoiceFirst_Admin.Utilities.DTOs.Features.ApplicationVersion;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping
{
    public class ApplicationVersionProfile : Profile
    {
        public ApplicationVersionProfile()
        {
            CreateMap<PlatformVersionCreateDto, ApplicationVersion>()
                .ForMember(d => d.Version, o => o.MapFrom(s => s.Version))
                .ForMember(d => d.Type, o => o.MapFrom(s => s.ClientType))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => true));
        }
    }
}
