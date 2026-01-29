using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.Place;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Utilities.Mapping
{
    public class PlaceMappingProfile:Profile
    {
        public PlaceMappingProfile()
        {
            CreateMap<PlaceCreateDTO, Place>()
                .ForMember(d => d.PlaceName, o => o.MapFrom(s => s.PlaceName))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => true));

            CreateMap<(PlaceUpdateDTO, int Id, int UserId), Place>()
               .ForMember(d => d.PlaceName, o => o.MapFrom(s => s.Item1.PlaceName))
               .ForMember(d => d.PlaceId, o => o.MapFrom(s => s.Id))
               .ForMember(d => d.IsActive, o => o.MapFrom(s => s.Item1.Active))
               .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UserId));

        }
    }
    }
