namespace VoiceFirst_Admin.Utilities.DTOs.Features.Role;

public class RoleUpdateDto
{
    public string? RoleName { get; set; }
    public string? RolePurpose { get; set; }
    public int? ApplicationId { get; set; }
    public IEnumerable<int> ActionLinkId { get; set; } = new List<int>();
}
