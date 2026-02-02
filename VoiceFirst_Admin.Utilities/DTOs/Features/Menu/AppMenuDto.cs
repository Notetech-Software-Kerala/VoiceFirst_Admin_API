using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Menu
{
    public class AppMenuDto :CommonDto
    {
        public int WebMenuId { get; set; }
        public int? ParentId { get; set; }
        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
