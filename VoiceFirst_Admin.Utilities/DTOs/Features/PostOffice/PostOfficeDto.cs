using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;

public class PostOfficeDto : CommonDto
{
    public int PostOfficeId { get; set; }
    public string PostOfficeName { get; set; }
    public string? CountryName { get; set; } 
    public string? DivOneLabel { get; set; } 
    public string? DivTwoLabel { get; set; } 
    public string? DivThreeLabel { get; set; } 
    public int CountryId { get; set; }
    public int DivOneId { get; set; }
    public string DivOneName { get; set; } 
    public int DivTwoId { get; set; }
    public string DivTwoName { get; set; }
    public int DivThreeId { get; set; }
    public string DivThreeName { get; set; }
    public IEnumerable<ZipCodeDto> ZipCodes { get; set; } = new List<ZipCodeDto>();
}
