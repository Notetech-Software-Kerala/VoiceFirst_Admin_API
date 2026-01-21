using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Shared;

public enum SortOrder { Asc, Desc }
public class CommonFilterDto
{
   
    public string? SortBy { get; set; }                     // e.g. "UserId", "FirstName", "CreatedAt"
    public SortOrder SortOrder { get; set; } = SortOrder.Asc;    // "asc" or "desc"

    public string? SearchText { get; set; }         
    public int PageNumber { get; set; } = 1;        // default
    public int Limit { get; set; } = 10;            // default
    public string? CreatedFromDate { get; set; }
    public string? CreatedToDate { get; set; }

    public string? UpdatedFromDate { get; set; }
    public string? UpdatedToDate { get; set; }

    public string? DeletedFromDate { get; set; }
    public string? DeletedToDate { get; set; }

    public bool? Active { get; set; }               // null=both, false=0, true=1
    public bool? Deleted { get; set; }              // null=both, false=0, true=1
}
