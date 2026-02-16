namespace VoiceFirst_Admin.Utilities.DTOs.Features.Role;

public class RoleUpdateDto
{
    public string? RoleName { get; set; }
    public string? RolePurpose { get; set; }
    public int? PlatformId { get; set; }

    // Add these links (insert if not exists)
    public List<PlanActionLinkCreateDto>? CreatePlanActionLink { get; set; }

    // Update existing links to the desired state
    public List<PlanRoleActionLinkUpdateDto>? UpdatePlanActionLinks { get; set; }
}


