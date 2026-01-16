using System.Collections.Generic;

namespace VoiceFirst_Admin.Utilities.DTOs.Shared
{
    public class PagedResultDto<T>
    {
        public IReadOnlyList<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
