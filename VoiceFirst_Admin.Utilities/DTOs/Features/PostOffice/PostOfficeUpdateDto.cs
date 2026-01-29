namespace VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;

using System.Collections.Generic;

public class PostOfficeUpdateDto
{
    public string? PostOfficeName { get; set; } 
    public int? CountryId { get; set; }

    public int? DivOneId { get; set; }
    public int? DivTwoId { get; set; }
    public int? DivThreeId { get; set; }

    public bool? Active { get; set; }
    public List<string> AddZipCodes { get; set; } = new List<string>();
    public IEnumerable<ZipCodeUpdateDto> UpdateZipCodes { get; set; } = new List<ZipCodeUpdateDto>();
}
