namespace VoiceFirst_Admin.Utilities.DTOs.Features.Menu;

public class ProgramStatusDto
{
    public int ProgramId { get; set; }
    public bool Status { get; set; }
}

public class MenuUpdateDto
{
    public string? MenuName { get; set; }
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public int? PlateFormId { get; set; }

    public List<int>? AddProgramId { get; set; }
    public List<ProgramStatusDto>? UpdateProgramId { get; set; }

    public MenuWebDto? Web { get; set; }
    public MenuAppDto? App { get; set; }
}
