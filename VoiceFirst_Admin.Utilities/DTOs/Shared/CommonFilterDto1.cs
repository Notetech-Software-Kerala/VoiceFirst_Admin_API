using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Shared
{
    public class CommonFilterDto1
    {
        // 🔍 Searching
        public string? Search { get; set; }

        // 🔃 Sorting (logical field names)
        public string? SortBy { get; set; }
        public SortDirection SortOrder { get; set; } = SortDirection.Desc;

        // 📄 Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // 🔐 Status filters
        public bool? Active { get; set; }
        public bool? Delete { get; set; }

    }

    public enum SortDirection
    {
        Asc,
        Desc
    }
}

