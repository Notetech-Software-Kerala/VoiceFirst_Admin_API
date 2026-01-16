using VoiceFirst_Admin.Utilities.Validators;
using VfValidationResult = VoiceFirst_Admin.Utilities.Models.Common.ValidationResult;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.UserDevice;

public class UserDeviceCreateDto
{
    public string DeviceID { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
    public string DeviceType { get; set; } = string.Empty;
    public string OS { get; set; } = string.Empty;
    public string OSVersion { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;

    public VfValidationResult Validate()
        => UserDeviceCreateDtoValidator.Validate(this);
}
