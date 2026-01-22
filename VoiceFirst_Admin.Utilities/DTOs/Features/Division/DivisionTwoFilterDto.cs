using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Division;

public class DivisionTwoFilterDto : CommonBasicFilterDto
{
    public int? DivisionOneId { get; set; }
    public DivTwoSearchBy? SearchBy { get; set; }
}
public enum DivTwoSearchBy
{
    DivTwoName,
    DivOneName

}
