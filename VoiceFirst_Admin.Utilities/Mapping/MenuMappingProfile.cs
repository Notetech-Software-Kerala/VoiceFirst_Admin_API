using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.Menu;
using VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping
{
    public class MenuMappingProfile : Profile
    {
        public MenuMappingProfile()
        {
            CreateMap<MenuProgramCreaDto, MenuProgramLink>()
            .ForMember(d => d.ProgramId, opt => opt.MapFrom(s => s.ProgramId))
                .ForMember(d => d.IsPrimaryProgram, opt => opt.MapFrom(s => s.Primary));

            CreateMap<MenuCreateDto, MenuMaster>()
            .ForMember(d => d.MenuName, opt => opt.MapFrom(s => s.MenuName))
            .ForMember(d => d.MenuRoute, opt => opt.MapFrom(s => s.Route))
            .ForMember(d => d.ApplicationId, opt => opt.MapFrom(s => s.PlateFormId))
                .ForMember(d => d.MenuIcon, opt => opt.MapFrom(s => s.Icon));
        }
    }
}
