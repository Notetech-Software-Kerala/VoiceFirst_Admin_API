using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;

public class ZipCodeDto : CommonDto
{
    public int? ZipCodeId { get; set; }
    public string ZipCode { get; set; } = string.Empty;

}
