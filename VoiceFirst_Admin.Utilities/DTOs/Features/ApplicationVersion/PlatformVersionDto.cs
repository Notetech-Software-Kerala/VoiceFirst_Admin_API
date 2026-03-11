using VoiceFirst_Admin.Utilities.Enums;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.ApplicationVersion;

public class PlatformVersionDto
{
    public int PlatformVersionId { get; set; }
    public int PlatformId { get; set; }
    public string PlatformName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public   ClientType  ClientType { get; set; } 
    public bool Active { get; set; }
    public bool Deleted { get; set; }
    public string CreatedUser { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string ModifiedUser { get; set; } = string.Empty;
    public DateTime? ModifiedDate { get; set; }
    public string DeletedUser { get; set; } = string.Empty;
    public DateTime? DeletedDate { get; set; }
}
