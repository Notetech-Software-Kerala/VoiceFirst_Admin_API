using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Division;

public class DivisionOneFilterDto : CommonBasicFilterDto
{
    public int? CountryId { get; set; }
    public DivOneSearchBy? SearchBy { get; set; }
}
public enum DivOneSearchBy
{
    CountryName,
    DivOneName

}