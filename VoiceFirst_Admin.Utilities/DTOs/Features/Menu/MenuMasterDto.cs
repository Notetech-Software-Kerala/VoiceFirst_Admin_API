using System;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Menu;

public class MenuMasterDto
{
    public int MenuId { get; set; }
    public string MenuName { get; set; } 
    public string Icon { get; set; } 
    public string Route { get; set; }
    public string PlateForm { get; set; } 
    public int PlateFormId { get; set; }
    public bool Active { get; set; }
    public bool Deleted { get; set; }
    public string CreatedUser { get; set; }
    public DateTime CreatedDate { get; set; }
    public string ModifiedUser { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string DeletedUser { get; set; }
    public DateTime? DeletedDate { get; set; }

    public bool Web { get; set; }
    public bool App { get; set; }
}
public class MenuMasterDetailDto : MenuMasterDto
{
    public List<MenuProgramLinkDto> menuProgramLinks { get; set; }
}
