using AutoMapper;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Features.Users;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<EmployeeCreateDto, Users>()
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName))
                .ForMember(d => d.Gender, o => o.MapFrom(s => s.Gender))
                .ForMember(d => d.MobileNo, o => o.MapFrom(s => s.MobileNo))
                .ForMember(d => d.BirthYear, o => o.MapFrom(s => s.BirthYear))
                .ForMember(d => d.MobileCountryId, o => o.MapFrom(s => s.MobileCountryCodeId))
                .ForMember(d => d.CreatedBy, o => o.MapFrom((src, dest, destMember, ctx) => (int)ctx.Items["CreatedBy"]))
                .ForMember(d => d.HashKey, o => o.MapFrom((src, dest, destMember, ctx) => (byte[])ctx.Items["HashKey"]))
                .ForMember(d => d.SaltKey, o => o.MapFrom((src, dest, destMember, ctx) => (byte[])ctx.Items["SaltKey"]))
                ;


            CreateMap<(EmployeeUpdateDto, int userId, int updatingUserId), Users>()
                .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.userId))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(s => s.Item1.FirstName ?? string.Empty))
                .ForMember(d => d.LastName, opt => opt.MapFrom(s => s.Item1.LastName ?? string.Empty))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Item1.Email ?? string.Empty))
                .ForMember(d => d.Gender, opt => opt.MapFrom(s => s.Item1.Gender ?? string.Empty))
                .ForMember(d => d.BirthYear, opt => opt.MapFrom(s => s.Item1.BirthYear))
                .ForMember(d => d.MobileNo, opt => opt.MapFrom(s => s.Item1.MobileNo ?? string.Empty))
                .ForMember(d => d.MobileCountryId, opt => opt.MapFrom(s => s.Item1.MobileCountryCodeId ?? 0))
                .ForMember(d => d.UpdatedBy, opt => opt.MapFrom(s => s.updatingUserId))
                .ForMember(d => d.IsActive, opt => opt.MapFrom(s => s.Item1.Active));
        }
    }
}
