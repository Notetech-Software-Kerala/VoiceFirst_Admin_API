namespace VoiceFirst_Admin.Utilities.DTOs.Features.Role;

public class RoleCreateDto
{
    public string RoleName { get; set; } 
    public string? RolePurpose { get; set; }
    public int ApplicationId { get; set; }
    public List<int> ActionLinkIds { get; set; } = new List<int>();
}
