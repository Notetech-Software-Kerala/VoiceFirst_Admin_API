using AutoMapper;
using VoiceFirst_Admin.Utilities.DTOs.Features.Division;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping;

public class DivisionMappingProfile : Profile
{
    public DivisionMappingProfile()
    {
        CreateMap<DivisionOne, DivisionOneDto>()
                .ForMember(d => d.CountryId, opt => opt.MapFrom(s => s.CountryId))
                .ForMember(d => d.DivOneId, opt => opt.MapFrom(s => s.DivisionOneId))
                .ForMember(d => d.CountryName, opt => opt.MapFrom(s => s.CountryName))
                .ForMember(d => d.DivOneName, opt => opt.MapFrom(s => s.DivisionOneName))
                .ForMember(d => d.Active, opt => opt.MapFrom(s => s.IsActive))
                .ForMember(d => d.Deleted, opt => opt.MapFrom(s => s.IsDeleted));

        CreateMap<DivisionTwo, DivisionTwoDto>()
                .ForMember(d => d.DivTwoId, opt => opt.MapFrom(s => s.DivisionTwoId))
                .ForMember(d => d.DivOneId, opt => opt.MapFrom(s => s.DivisionOneId))
                .ForMember(d => d.DivTwoName, opt => opt.MapFrom(s => s.DivisionTwoName))
                .ForMember(d => d.DivOneName, opt => opt.MapFrom(s => s.DivisionOneName))
                .ForMember(d => d.Active, opt => opt.MapFrom(s => s.IsActive))
                .ForMember(d => d.Deleted, opt => opt.MapFrom(s => s.IsDeleted));
        CreateMap<DivisionThree, DivisionThreeDto>()
                .ForMember(d => d.DivTwoId, opt => opt.MapFrom(s => s.DivisionTwoId))
                .ForMember(d => d.DivThreeId, opt => opt.MapFrom(s => s.DivisionThreeId))
                .ForMember(d => d.DivTwoName, opt => opt.MapFrom(s => s.DivisionTwoName))
                .ForMember(d => d.DivThreeName, opt => opt.MapFrom(s => s.DivisionThreeName))
                .ForMember(d => d.Active, opt => opt.MapFrom(s => s.IsActive))
                .ForMember(d => d.Deleted, opt => opt.MapFrom(s => s.IsDeleted));

        CreateMap<DivisionOne, DivisionOneLookUpDto>()
                .ForMember(d => d.DivOneId, opt => opt.MapFrom(s => s.DivisionOneId))
                .ForMember(d => d.DivOneName, opt => opt.MapFrom(s => s.DivisionOneName));

        CreateMap<DivisionTwo, DivisionTwoLookUpDto>()
                .ForMember(d => d.DivTwoId, opt => opt.MapFrom(s => s.DivisionTwoId))
                .ForMember(d => d.DivTwoName, opt => opt.MapFrom(s => s.DivisionTwoName));
        CreateMap<DivisionThree, DivisionThreeLookUpDto>()
                .ForMember(d => d.DivThreeId, opt => opt.MapFrom(s => s.DivisionThreeId))
                .ForMember(d => d.DivThreeName, opt => opt.MapFrom(s => s.DivisionThreeName));
    }
}
