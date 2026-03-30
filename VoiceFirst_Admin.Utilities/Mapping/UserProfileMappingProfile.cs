using AutoMapper;
using VoiceFirst_Admin.Utilities.DTOs.Features.User;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping
{
    public class UserProfileMappingProfile : Profile
    {

        public UserProfileMappingProfile()
        {
            CreateMap<(UserProfileUpdateDto,int userId), Users>()

                // Direct mappings
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Item1.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Item1.LastName))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Item1.Gender))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.userId))

                // Nullable → Required safety
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Item1.Email ?? string.Empty))
                .ForMember(dest => dest.MobileNo, opt => opt.MapFrom(src => src.Item1.MobileNo ?? string.Empty))

                // Different property name
                .ForMember(dest => dest.MobileCountryId, opt => opt.MapFrom(src => src.Item1.DialCodeId))

                // String → short conversion
                .ForMember(dest => dest.BirthYear, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Item1.BirthYear)
                        ? (short?)null
                        : Convert.ToInt16(src.Item1.BirthYear)
                ))
 

                // Ignore properties not coming from DTO
                .ForMember(dest => dest.HashKey, opt => opt.Ignore())
                .ForMember(dest => dest.SaltKey, opt => opt.Ignore())
                .ForMember(dest => dest.LinkedinId, opt => opt.Ignore())
                .ForMember(dest => dest.FacebookId, opt => opt.Ignore())
                .ForMember(dest => dest.GoogleId, opt => opt.Ignore());
        }
    }
}
