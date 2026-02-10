using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Menu;


public class AppMenuMoveDto
{
    public int AppMenuId { get; set; }
    public int? ParentAppMenuId { get; set; }
    public List<int>? NewOrderUnderToParent { get; set; }
}

public class AppMenuReorderDto
{
    public int ParentAppMenuId { get; set; }
    public List<int> OrderedIds { get; set; } = new List<int>();
}

public class AppMenuStatusDto
{
    public int AppMenuId { get; set; }
    public bool Active { get; set; }
}

public class AppMenuBulkUpdateDto
{
    public List<AppMenuMoveDto>? MoveAndReorder { get; set; }
    public List<AppMenuReorderDto>? Reorders { get; set; }
    public List<AppMenuStatusDto>? StatusUpdate { get; set; }
}

