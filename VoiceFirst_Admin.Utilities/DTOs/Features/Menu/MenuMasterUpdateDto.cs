namespace VoiceFirst_Admin.Utilities.DTOs.Features.Menu;

public class MenuMasterUpdateDto
{
    public string? MenuName { get; set; }
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public int? PlateFormId { get; set; }
    public bool? Active { get; set; }
}
