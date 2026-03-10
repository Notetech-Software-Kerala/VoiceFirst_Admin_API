using VoiceFirst_Admin.Utilities.Enums;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.ApplicationVersion;

public class PlatformVersionCreateDto
{
    public string Version { get; set; } = string.Empty;
    public ClientType ClientType { get; set; } = ClientType.Web;
}
