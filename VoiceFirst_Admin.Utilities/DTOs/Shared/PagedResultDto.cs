using System.Collections.Generic;

namespace VoiceFirst_Admin.Utilities.DTOs.Shared
{
    public class PagedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        // ⭐ Enterprise standard
        public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
