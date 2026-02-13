using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class MenuMaster : BaseModel
{
    public int MenuMasterId { get; set; }
    public string MenuName { get; set; } 
    public string MenuIcon { get; set; } 
    public string MenuRoute { get; set; }
    public int ApplicationId { get; set; }
    public string ApplicationName { get; set; }
}
