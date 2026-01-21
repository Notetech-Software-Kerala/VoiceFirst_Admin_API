namespace VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;

using System.Collections.Generic;

public class PostOfficeUpdateDto
{
    public string? PostOfficeName { get; set; } 
    public int? CountryId { get; set; }
    public bool? IsActive { get; set; }
    public IEnumerable<ZipCodeUpdateDto> ZipCodes { get; set; } = new List<ZipCodeUpdateDto>();
}
