using AutoMapper;
using VoiceFirst_Admin.Utilities.DTOs.Features.Country;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping;

public class CountryMappingProfile : Profile
{
    public CountryMappingProfile()
    {
        CreateMap<Country, CountryDto>()
            .ForMember(d => d.CountryId, opt => opt.MapFrom(s => s.CountryId))
                .ForMember(d => d.CountryName, opt => opt.MapFrom(s => s.CountryName))
                .ForMember(d => d.DivisionOne, opt => opt.MapFrom(s => s.DivisionOneName))
                .ForMember(d => d.Active, opt => opt.MapFrom(s => s.IsActive))
                .ForMember(d => d.Deleted, opt => opt.MapFrom(s => s.IsDeleted))
                .ForMember(d => d.DivisionTwo, opt => opt.MapFrom(s => s.DivisionTwoName))
                .ForMember(d => d.DivisionThree, opt => opt.MapFrom(s => s.DivisionThreeName))
                .ForMember(d => d.DialCode, opt => opt.MapFrom(s => s.CountryDialCode))
                .ForMember(d => d.IsoAlphaTwo, opt => opt.MapFrom(s => s.CountryIsoAlphaTwo));
    }
}
