using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class Users : BaseModel
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }

    public string Gender { get; set; } = string.Empty; // 'Male' | 'Female' | 'Other'

    public string? LinkedinId { get; set; }
    public string? FacebookId { get; set; }
    public string? GoogleId { get; set; }

    public string Email { get; set; } = string.Empty;
    public int MobileCountryId { get; set; }
    public string MobileNo { get; set; } = string.Empty;

    public byte[] HashKey { get; set; } = Array.Empty<byte>();
    public byte[] SaltKey { get; set; } = Array.Empty<byte>();

    public short BirthYear { get; set; }
}
