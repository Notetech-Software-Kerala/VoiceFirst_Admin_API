using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities
{
    public class WebMenu : BaseModel
    {
        public int WebMenuId { get; set; }
        public int ParentWebMenuId { get; set; } = 0;
        public int MenuMasterId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string MenuIcon { get; set; } = string.Empty;
        public string MenuRoute { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
