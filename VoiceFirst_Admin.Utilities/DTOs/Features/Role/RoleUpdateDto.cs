namespace VoiceFirst_Admin.Utilities.DTOs.Features.Role;

public class RoleUpdateDto
{
    public string? RoleName { get; set; }
    public string? RolePurpose { get; set; }
    public int? PlatformId { get; set; }

    // Add these links (insert if not exists)
    public List<PlanActionLinkCreateDto>? PlanActionLinkCreateDto { get; set; }

    // Update existing links to the desired state
    public List<PlanRoleActionLinkUpdateDto>? UpdateActionLinks { get; set; }
}
public class ActionLinkDto
{
    
    public int ActionLinkId { get; set; }
    public bool Active { get; set; }
}

public class PlanRoleActionLinkUpdateDto
{
    public int RolePlanLinkId { get; set; }
    public List<ActionLinkDto>? UpdateActionLinks { get; set; }
}