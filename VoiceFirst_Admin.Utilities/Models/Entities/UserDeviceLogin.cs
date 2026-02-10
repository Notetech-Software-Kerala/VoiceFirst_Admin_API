namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class UserDeviceLogin
{
    public int UserDeviceLoginId { get; set; }
    public int UserId { get; set; }
    public int UserDeviceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsCurrentSession { get; set; }
}
