using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.UserDevice;

public class UserDeviceCreateDto
{
    [Required, MaxLength(100)]
    public string DeviceID { get; set; } = string.Empty;

    [Required]
    public string Version { get; set; }

    [MaxLength(100)]
    public string? DeviceName { get; set; }

    [Required, MaxLength(50)]
    public string DeviceType { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string OS { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string OSVersion { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Manufacturer { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Model { get; set; } = string.Empty;
}
