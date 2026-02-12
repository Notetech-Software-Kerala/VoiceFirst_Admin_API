using VoiceFirst_Admin.Utilities.DTOs.Features.UserDevice;
using VoiceFirst_Admin.Utilities.Enums;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Auth;

public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public ClientType ClientType { get; set; } = ClientType.Web;
    public UserDeviceCreateDto Device { get; set; } = new();
}
