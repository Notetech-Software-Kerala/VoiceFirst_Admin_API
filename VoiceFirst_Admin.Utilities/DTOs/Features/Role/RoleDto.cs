using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Role;

public class RoleDto : CommonDto
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsMandatory { get; set; }
    public string? RolePurpose { get; set; }
    public int PlatformId { get; set; }
    public string? Platform { get; set; }
}
public class RoleDetailDto : RoleDto
{

    public List<PlanRoleActionLinkDetailsDto>? PlanRoleActionLink { get; set; }
}       