using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping
{
    public class SysBusinessActivityProfile:Profile
    {
        public SysBusinessActivityProfile()
        {
            CreateMap<SysBusinessActivityCreateDTO, SysBusinessActivity>()
                .ForMember(d => d.BusinessActivityName, o => o.MapFrom(s => s.ActivityName))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => true));

            CreateMap<(SysBusinessActivityUpdateDTO,int Id,int UserId), SysBusinessActivity>()
                .ForMember(d => d.BusinessActivityName, o => o.MapFrom(s => s.Item1.ActivityName))
                .ForMember(d=>d.SysBusinessActivityId,o=>o.MapFrom(s => s.Id))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => s.Item1.Active))
                .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UserId));

        

            CreateMap<SysBusinessActivity, SysBusinessActivityDTO>()
          .ForMember(d => d.ActivityId, o => o.MapFrom(s => s.SysBusinessActivityId))
          .ForMember(d => d.ActivityName, o => o.MapFrom(s => s.BusinessActivityName))
          .ForMember(d => d.Active, o => o.MapFrom(s => s.IsActive))
          .ForMember(d => d.Deleted, o => o.MapFrom(s => s.IsDeleted))
          .ForMember(d => d.CreatedUser, o => o.MapFrom(s => s.CreatedUserName))
          .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedAt))
          .ForMember(d => d.ModifiedUser, o => o.MapFrom(s => s.UpdatedUserName))
          .ForMember(d => d.ModifiedDate, o => o.MapFrom(s => s.UpdatedAt))
          .ForMember(d => d.DeletedUser, o => o.MapFrom(s => s.DeletedUserName))
          .ForMember(d => d.DeletedDate, o => o.MapFrom(s => s.DeletedAt))
          ;

         

            CreateMap<SysBusinessActivity, SysBusinessActivityActiveDTO>()
           .ForMember(d => d.ActivityId, o => o.MapFrom(s => s.SysBusinessActivityId))
           .ForMember(d => d.ActivityName, o => o.MapFrom(s => s.BusinessActivityName))     
           ;

        }
    }
}
