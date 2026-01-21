using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Country;

public class CountryFilterDto : CommonFilterDto
{
    public CountrySearchBy? SearchBy { get; set; }

}

public enum CountrySearchBy
{
    CountryName,
    DivisionOne,
    DivisionTwo,
    DivisionThree,
    DialCode,
    IsoAlphaTwo

}