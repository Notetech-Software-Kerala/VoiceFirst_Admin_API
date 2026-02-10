namespace VoiceFirst_Admin.Utilities.DTOs.Features.Menu;

public class MenuCreateDto
{
    public string MenuName { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public int PlateFormId { get; set; }
    public List<MenuProgramCreateDto>? ProgramIds { get; set; }
    public bool Web { get; set; }
    public bool App { get; set; }
}

