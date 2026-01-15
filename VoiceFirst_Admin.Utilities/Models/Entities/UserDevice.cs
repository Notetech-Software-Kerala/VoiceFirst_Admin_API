using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class UserDevice
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

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // DB column is "IsDelete"
    public bool IsDeleted { get; set; }
}

