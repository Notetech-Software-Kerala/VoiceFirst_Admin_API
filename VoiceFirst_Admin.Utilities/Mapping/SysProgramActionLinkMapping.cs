using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping
{
    public class SysProgramActionLinkMapping : Profile
    {
        public SysProgramActionLinkMapping()
        {
            CreateMap<SysProgramActionsLink, SysProgramActionLinkDTO>()
                .ForMember(d => d.ActionId, opt => opt.MapFrom(s => s.ProgramActionId))                
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(s => s.UpdatedAt))
                .ForMember(d => d.Active, opt => opt.MapFrom(s => s.IsActive ?? true));
        }
    }
  
}
