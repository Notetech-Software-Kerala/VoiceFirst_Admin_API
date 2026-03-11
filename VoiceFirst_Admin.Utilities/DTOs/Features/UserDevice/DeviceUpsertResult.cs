using VoiceFirst_Admin.Utilities.Enums;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.UserDevice;

public class DeviceUpsertResult
{
    public int UserDeviceId { get; set; }
    public string ClientType { get; set; } = string.Empty;
}
