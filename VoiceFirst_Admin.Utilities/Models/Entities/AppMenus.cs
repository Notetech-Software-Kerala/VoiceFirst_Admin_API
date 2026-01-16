using System;
using VoiceFirst_Admin.Utilities.Models;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class AppMenus : BaseModel
{
    public int AppMenuId { get; set; }
    public int? ParentAppMenuId { get; set; }
    public int MenuMasterId { get; set; }
    public int SortOrder { get; set; }
}
