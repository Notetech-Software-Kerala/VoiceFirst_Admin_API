using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping
{
    public class SysBusinessActivityProfile:Profile
    {
        public SysBusinessActivityProfile()
        {
            CreateMap<SysBusinessActivityCreateDTO, SysBusinessActivity>()
                .ForMember(d => d.BusinessActivityName, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => true));

            CreateMap<(SysBusinessActivityUpdateDTO,int Id), SysBusinessActivity>()
                .ForMember(d => d.BusinessActivityName, o => o.MapFrom(s => s.Item1.Name))
                .ForMember(d=>d.SysBusinessActivityId,o=>o.MapFrom(s => s.Id))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => s.Item1.Status));

        

            CreateMap<SysBusinessActivity, SysBusinessActivityDTO>()
          .ForMember(d => d.Id, o => o.MapFrom(s => s.SysBusinessActivityId))
          .ForMember(d => d.Name, o => o.MapFrom(s => s.BusinessActivityName))
          .ForMember(d => d.Active, o => o.MapFrom(s => s.IsActive))
          .ForMember(d => d.Delete, o => o.MapFrom(s => s.IsDeleted))
          .ForMember(d => d.CreatedUser, o => o.MapFrom(s => s.CreatedUser))
          .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedAt))
          .ForMember(d => d.ModifiedUser, o => o.MapFrom(s => s.UpdatedUser))
          .ForMember(d => d.ModifiedDate, o => o.MapFrom(s => s.UpdatedAt))
          .ForMember(d => d.DeletedUser, o => o.MapFrom(s => s.DeletedUser))
          .ForMember(d => d.DeletedDate, o => o.MapFrom(s => s.DeletedAt))
          ;

            CreateMap<SysBusinessActivity, SysBusinessActivityActiveDTO>()
           .ForMember(d => d.Id, o => o.MapFrom(s => s.SysBusinessActivityId))
           .ForMember(d => d.Name, o => o.MapFrom(s => s.BusinessActivityName))     
           ;

        }
    }
}
