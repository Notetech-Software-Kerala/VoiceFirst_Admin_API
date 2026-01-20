using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction;
using VoiceFirst_Admin.Utilities.DTOs.Features.UserDevice;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping
{
    public class ProgramActionMappingProfile :Profile
    {
        public ProgramActionMappingProfile()
        {
            CreateMap<SysProgramActions, ProgramActionDto>()
                .ForMember(d => d.ActionId, opt => opt.MapFrom(s => s.SysProgramActionId))
                .ForMember(d => d.ActionName, opt => opt.MapFrom(s => s.ProgramActionName))
                .ForMember(d => d.Active, opt => opt.MapFrom(s => s.IsActive))
                .ForMember(d => d.Deleted, opt => opt.MapFrom(s => s.IsDeleted))
                .ForMember(d => d.CreatedUser, opt => opt.MapFrom(s => s.CreatedUserName))
                .ForMember(d => d.ModifiedUser, opt => opt.MapFrom(s => s.UpdatedUserName))
                .ForMember(d => d.DeletedUser, opt => opt.MapFrom(s => s.DeletedUserName))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.DeletedDate, opt => opt.MapFrom(s => s.DeletedAt))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(s => s.UpdatedAt));

            CreateMap<SysProgramActions, ProgramActionLookupDto>()
               .ForMember(d => d.ActionId, opt => opt.MapFrom(s => s.SysProgramActionId))
               .ForMember(d => d.ActionName, opt => opt.MapFrom(s => s.ProgramActionName));

        }
    }
}
