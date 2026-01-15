using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.UserDevice;
using VoiceFirst_Admin.Utilities.Models.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoiceFirst_Admin.Utilities.Mapping;

public class UserDeviceMappingProfile : Profile
{
    public UserDeviceMappingProfile()
    {
        CreateMap<UserDeviceCreateDto, UserDevice>()
            .ForMember(d => d.DeviceID, opt => opt.MapFrom(s => s.DeviceID))
            .ForMember(d => d.DeviceName, opt => opt.MapFrom(s => s.DeviceName))
            .ForMember(d => d.DeviceType, opt => opt.MapFrom(s => s.DeviceType))
            .ForMember(d => d.OS, opt => opt.MapFrom(s => s.OS))
            .ForMember(d => d.OSVersion, opt => opt.MapFrom(s => s.OSVersion))
            .ForMember(d => d.Manufacturer, opt => opt.MapFrom(s => s.Manufacturer))
            .ForMember(d => d.Model, opt => opt.MapFrom(s => s.Model))
            .ForAllMembers(opt => opt.Ignore());

    }
}
