using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Shared;

public class CommonFilterDto
{
    public string? SortBy { get; set; }          // e.g. "UserId", "FirstName", "CreatedAt"
    public string? SortOrder { get; set; }       // "asc" or "desc"
    public string? SearchText { get; set; }       // "asc" or "desc"
    public int PageNumber { get; set; } = 1;           // default
    public int Limit { get; set; } = 10;         // default
    public bool? Active { get; set; }         // null=both, false=0, true=1
    public bool? Deleted { get; set; }         // null=both, false=0, true=1
}