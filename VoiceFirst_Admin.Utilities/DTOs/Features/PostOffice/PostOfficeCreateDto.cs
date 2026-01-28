namespace VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;

using System.Collections.Generic;

public class PostOfficeCreateDto
{
    public string PostOfficeName { get; set; } = string.Empty;
    public int? CountryId { get; set; }
    public int? DivOneId { get; set; }
    public int? DivTwoId { get; set; }
    public int? DivThreeId { get; set; }
    public List<string> ZipCodes { get; set; } = new List<string>();
}
