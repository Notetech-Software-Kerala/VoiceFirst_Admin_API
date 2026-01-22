using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Division;

public class DivisionThreeFilterDto : CommonBasicFilterDto
{
    public int? DivisionTwoId { get; set; }
    public DivThreeSearchBy? SearchBy { get; set; }
}
public enum DivThreeSearchBy
{
    DivTwoName,
    DivThreeName

}