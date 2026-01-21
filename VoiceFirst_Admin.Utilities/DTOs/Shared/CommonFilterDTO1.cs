using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Shared
{
    public class CommonFilterDTO1
    {
        public string fliter { get; set; } = string.Empty; // reserved word
        public string? SortBy { get; set; }          // e.g. "UserId", "FirstName", "CreatedAt"
        public SortOrder SortOrder { get; set; } = SortOrder.Asc;
        public string? SearchText { get; set; }       // "asc" or "desc"
        public int PageNumber { get; set; } = 1;           // default
        public int Limit { get; set; } = 10;         // default
        public string? CreatedFromDate { get; set; }
        public string? CreatedToDate { get; set; }

        public string? UpdatedFromDate { get; set; }
        public string? UpdatedToDate { get; set; }

        public string? DeletedFromDate { get; set; }
        public string? DeletedToDate { get; set; }

        public bool? Active { get; set; }         // null=both, false=0, true=1
        public bool? Deleted { get; set; }         // null=both, false=0, true=1
    }
}
