using VoiceFirst_Admin.Utilities.DTOs.Features.UserDevice;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Auth;

public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserDeviceCreateDto Device { get; set; } = new();
}
