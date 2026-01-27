using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;

public class PostOfficeDto : CommonDto
{
    public int PostOfficeId { get; set; }
    public string PostOfficeName { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public int? CountryId { get; set; }

    public IEnumerable<ZipCodeDto> ZipCodes { get; set; } = new List<ZipCodeDto>();
}
