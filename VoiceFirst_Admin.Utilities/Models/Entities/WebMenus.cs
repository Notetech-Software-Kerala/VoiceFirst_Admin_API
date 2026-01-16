using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class WebMenus : BaseModel
{
    public int WebMenuId { get; set; }
    public int? ParentWebMenuId { get; set; }
    public int MenuMasterId { get; set; }
    public int SortOrder { get; set; }
}
