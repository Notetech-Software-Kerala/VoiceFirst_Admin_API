using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs;

public class CommonFilterDto
{
    public int? Id { get; set; }                 // optional: filter by UserId
    public string? SortBy { get; set; }          // e.g. "UserId", "FirstName", "CreatedAt"
    public string? SortOrder { get; set; }       // "asc" or "desc"
    public int Page { get; set; } = 1;           // default
    public int Limit { get; set; } = 10;         // default
    public bool? IsDeleted { get; set; }         // null=both, false=0, true=1
}