using System;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Menu;

public class MenuMasterDto
{
    public int MenuId { get; set; }
    public string MenuName { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public int PlateFormId { get; set; }
    public bool Active { get; set; }
    public bool Deleted { get; set; }
    public string CreatedUser { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string ModifiedUser { get; set; } = string.Empty;
    public DateTime? ModifiedDate { get; set; }
    public string DeletedUser { get; set; } = string.Empty;
    public DateTime? DeletedDate { get; set; }
}
