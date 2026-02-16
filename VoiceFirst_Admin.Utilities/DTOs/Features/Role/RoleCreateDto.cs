namespace VoiceFirst_Admin.Utilities.DTOs.Features.Role;

public class RoleCreateDto
{
    public string RoleName { get; set; } 
    public string? RolePurpose { get; set; }
    public int PlatformId { get; set; }
    public List<PlanActionLinkCreateDto> CreatePlanActionLink { get; set; }

}

