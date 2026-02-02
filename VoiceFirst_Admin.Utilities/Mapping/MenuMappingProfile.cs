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

            CreateMap<MenuMaster, MenuMasterDto>()
                .ForMember(d => d.MenuId, opt => opt.MapFrom(s => s.MenuMasterId))
                .ForMember(d => d.MenuName, opt => opt.MapFrom(s => s.MenuName))
                .ForMember(d => d.Route, opt => opt.MapFrom(s => s.MenuRoute))
                .ForMember(d => d.PlateFormId, opt => opt.MapFrom(s => s.ApplicationId))
                .ForMember(d => d.Icon, opt => opt.MapFrom(s => s.MenuIcon))
                .ForMember(d => d.Active, opt => opt.MapFrom(s => s.IsActive))
                .ForMember(d => d.Deleted, opt => opt.MapFrom(s => s.IsDeleted))
                .ForMember(d => d.CreatedUser, opt => opt.MapFrom(s => s.CreatedUserName))
                .ForMember(d => d.ModifiedUser, opt => opt.MapFrom(s => s.UpdatedUserName))
                .ForMember(d => d.DeletedUser, opt => opt.MapFrom(s => s.DeletedUserName))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.DeletedDate, opt => opt.MapFrom(s => s.DeletedAt))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(s => s.UpdatedAt)); 

            CreateMap<WebMenu, WebMenuDto>()
                .ForMember(d => d.WebMenuId, opt => opt.MapFrom(s => s.WebMenuId))
                .ForMember(d => d.MenuId, opt => opt.MapFrom(s => s.MenuMasterId))
                .ForMember(d => d.ParentId, opt => opt.MapFrom(s => s.ParentAppMenuId))
                .ForMember(d => d.MenuName, opt => opt.MapFrom(s => s.MenuName))
                .ForMember(d => d.Route, opt => opt.MapFrom(s => s.MenuRoute))
                .ForMember(d => d.Icon, opt => opt.MapFrom(s => s.MenuIcon))
                .ForMember(d => d.SortOrder, opt => opt.MapFrom(s => s.SortOrder))
                .ForMember(d => d.Active, opt => opt.MapFrom(s => s.IsActive))
                .ForMember(d => d.Deleted, opt => opt.MapFrom(s => s.IsDeleted))
                .ForMember(d => d.CreatedUser, opt => opt.MapFrom(s => s.CreatedUserName))
                .ForMember(d => d.ModifiedUser, opt => opt.MapFrom(s => s.UpdatedUserName))
                .ForMember(d => d.DeletedUser, opt => opt.MapFrom(s => s.DeletedUserName))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.DeletedDate, opt => opt.MapFrom(s => s.DeletedAt))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(s => s.UpdatedAt)); 
            CreateMap<AppMenus, AppMenuDto>()
                .ForMember(d => d.WebMenuId, opt => opt.MapFrom(s => s.AppMenuId))
                .ForMember(d => d.MenuId, opt => opt.MapFrom(s => s.MenuMasterId))
                .ForMember(d => d.ParentId, opt => opt.MapFrom(s => s.ParentAppMenuId))
                .ForMember(d => d.MenuName, opt => opt.MapFrom(s => s.MenuName))
                .ForMember(d => d.Route, opt => opt.MapFrom(s => s.MenuRoute))
                .ForMember(d => d.Icon, opt => opt.MapFrom(s => s.MenuIcon))
                .ForMember(d => d.SortOrder, opt => opt.MapFrom(s => s.SortOrder))
                .ForMember(d => d.Active, opt => opt.MapFrom(s => s.IsActive))
                .ForMember(d => d.Deleted, opt => opt.MapFrom(s => s.IsDeleted))
                .ForMember(d => d.CreatedUser, opt => opt.MapFrom(s => s.CreatedUserName))
                .ForMember(d => d.ModifiedUser, opt => opt.MapFrom(s => s.UpdatedUserName))
                .ForMember(d => d.DeletedUser, opt => opt.MapFrom(s => s.DeletedUserName))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.DeletedDate, opt => opt.MapFrom(s => s.DeletedAt))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(s => s.UpdatedAt));
        }
    }
}
