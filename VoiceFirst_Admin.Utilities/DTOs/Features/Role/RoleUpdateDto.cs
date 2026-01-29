namespace VoiceFirst_Admin.Utilities.DTOs.Features.Role;

public class RoleUpdateDto
{
    public string? RoleName { get; set; }
    public string? RolePurpose { get; set; }
    public int? ApplicationId { get; set; }

    // Add these links (insert if not exists)
    public IEnumerable<int> AddActionLinkIds { get; set; } = new List<int>();

    // Update existing links to the desired state
    public IEnumerable<ActionLinkStatusDto> UpdateActionLinks { get; set; } = new List<ActionLinkStatusDto>();
}
public class ActionLinkStatusDto
{
    public int ActionLinkId { get; set; }
    public bool Active { get; set; }
}

