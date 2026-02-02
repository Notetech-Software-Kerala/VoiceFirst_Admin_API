using System.Collections.Generic;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Menu;

public class WebMenuMoveDto
{
    public int WebMenuId { get; set; }
    public int? ParentWebMenuId { get; set; }
    public List<int>? NewOrderUnderToParent { get; set; }
}

public class WebMenuReorderDto
{
    public int ParentWebMenuId { get; set; }
    public List<int> OrderedIds { get; set; } = new List<int>();
}

public class WebMenuStatusDto
{
    public int WebMenuId { get; set; }
    public bool Active { get; set; }
}

public class WebMenuBulkUpdateDto
{
    public List<WebMenuMoveDto>? MoveAndReorder { get; set; }
    public List<WebMenuReorderDto>? Reorders { get; set; }
    public List<WebMenuStatusDto>? StatusUpdate { get; set; }
}
