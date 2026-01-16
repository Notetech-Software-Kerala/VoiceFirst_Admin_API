using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.UserDevice;
using VoiceFirst_Admin.Utilities.Models.Common;
using V = VoiceFirst_Admin.Utilities.Common.Validate;  // ✅ alias

namespace VoiceFirst_Admin.Utilities.Validators;

public static class UserDeviceCreateDtoValidator
{
    public static ValidationResult Validate(UserDeviceCreateDto dto)
    {
        var v = V.New();

        V.Required(v, nameof(dto.DeviceID), dto.DeviceID);
        V.Required(v, nameof(dto.Version), dto.Version);
        V.Required(v, nameof(dto.DeviceType), dto.DeviceType);
        V.Required(v, nameof(dto.OS), dto.OS);
        V.Required(v, nameof(dto.OSVersion), dto.OSVersion);
        V.Required(v, nameof(dto.Manufacturer), dto.Manufacturer);
        V.Required(v, nameof(dto.Model), dto.Model);

        V.MaxLength(v, nameof(dto.DeviceID), dto.DeviceID, 100);
        V.MaxLength(v, nameof(dto.DeviceName), dto.DeviceName, 100);
        V.MaxLength(v, nameof(dto.DeviceType), dto.DeviceType, 50);
        V.MaxLength(v, nameof(dto.OS), dto.OS, 50);
        V.MaxLength(v, nameof(dto.OSVersion), dto.OSVersion, 50);
        V.MaxLength(v, nameof(dto.Manufacturer), dto.Manufacturer, 50);
        V.MaxLength(v, nameof(dto.Model), dto.Model, 50);

        return v;
    }
}
