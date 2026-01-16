using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class MenuMaster : BaseModel
{
    public int MenuMasterId { get; set; }
    public string MenuName { get; set; } = string.Empty;
    public string MenuIcon { get; set; } = string.Empty;
    public string MenuRoute { get; set; } = string.Empty;
    public int ApplicationId { get; set; }
}
