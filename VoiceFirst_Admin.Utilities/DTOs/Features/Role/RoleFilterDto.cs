using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Role;

public class RoleFilterDto : CommonFilterDto
{
    public RoleSearchBy? SearchBy { get; set; }
}

public enum RoleSearchBy
{
    RoleName,
    RolePurpose,
    CreatedUser,
    UpdatedUser,
    DeletedUser
}
