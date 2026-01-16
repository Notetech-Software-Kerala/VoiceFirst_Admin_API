using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class UserDevice : BaseModel
{
    public int UserDeviceId { get; set; }

    public string DeviceID { get; set; } = string.Empty; // column name: DeviceID
    public int ApplicationVersionId { get; set; }

    public string? DeviceName { get; set; }

    public string DeviceType { get; set; } = string.Empty;
    public string OS { get; set; } = string.Empty;
    public string OSVersion { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
}

