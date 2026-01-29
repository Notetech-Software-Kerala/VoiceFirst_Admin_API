using System.Collections.Generic;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Menu;

public class MenuProgramDto
{
    public int MenuProgramLinkId { get; set; }
    public int ProgramId { get; set; }
    public bool IsPrimaryProgram { get; set; }
    public bool Active { get; set; }
}

public class WebMenuDetailDto
{
    public int WebMenuId { get; set; }
    public int? ParentWebMenuId { get; set; }
    public int SortOrder { get; set; }
}

public class AppMenuDetailDto
{
    public int AppMenuId { get; set; }
    public int? ParentAppMenuId { get; set; }
    public int SortOrder { get; set; }
}

public class MenuDetailDto
{
    public MenuMasterDto Master { get; set; } = new MenuMasterDto();
    public List<MenuProgramDto> Programs { get; set; } = new List<MenuProgramDto>();
    public WebMenuDetailDto? Web { get; set; }
    public AppMenuDetailDto? App { get; set; }
}
