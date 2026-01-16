using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class UserRoleLink : BaseModel
{
    public int UserRoleLinkId { get; set; }
    public int UserId { get; set; }
    public int? CompanyRoleId { get; set; }
    public int? SysRoleId { get; set; }
}
