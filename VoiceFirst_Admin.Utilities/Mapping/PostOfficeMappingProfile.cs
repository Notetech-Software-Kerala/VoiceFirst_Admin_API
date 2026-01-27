using AutoMapper;
using VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping;

public class PostOfficeMappingProfile : Profile
{
    public PostOfficeMappingProfile()
    {
        CreateMap<PostOffice, PostOfficeDto>()
            .ForMember(d => d.PostOfficeId, opt => opt.MapFrom(s => s.PostOfficeId))
                .ForMember(d => d.PostOfficeName, opt => opt.MapFrom(s => s.PostOfficeName))
                .ForMember(d => d.CountryName, opt => opt.MapFrom(s => s.CountryName))
                .ForMember(d => d.Active, opt => opt.MapFrom(s => s.IsActive))
                .ForMember(d => d.Deleted, opt => opt.MapFrom(s => s.IsDeleted))
                .ForMember(d => d.CreatedUser, opt => opt.MapFrom(s => s.CreatedUserName))
                .ForMember(d => d.ModifiedUser, opt => opt.MapFrom(s => s.UpdatedUserName))
                .ForMember(d => d.DeletedUser, opt => opt.MapFrom(s => s.DeletedUserName))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.DeletedDate, opt => opt.MapFrom(s => s.DeletedAt))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(s => s.UpdatedAt));

        CreateMap<PostOffice, PostOfficeLookupDto>();

        CreateMap<PostOfficeZipCode, ZipCodeDto>()
            .ForMember(d => d.ZipCodeId, opt => opt.MapFrom(s => s.PostOfficeZipCodeId))
            .ForMember(d => d.ZipCode, opt => opt.MapFrom(s => s.ZipCode))
            .ForMember(d => d.Active, opt => opt.MapFrom(s => s.IsActive))
            .ForMember(d => d.Deleted, opt => opt.MapFrom(s => s.IsDeleted))
                .ForMember(d => d.CreatedUser, opt => opt.MapFrom(s => s.CreatedUserName))
                .ForMember(d => d.ModifiedUser, opt => opt.MapFrom(s => s.UpdatedUserName))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(s => s.UpdatedAt)); ;
    }
}
