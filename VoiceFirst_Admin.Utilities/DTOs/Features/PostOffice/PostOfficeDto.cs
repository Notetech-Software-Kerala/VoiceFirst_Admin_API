namespace VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;

public class PostOfficeDto
{
    public int PostOfficeId { get; set; }
    public string PostOfficeName { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public int? CountryId { get; set; }
    public bool Active { get; set; }
    public bool? Deleted { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? CreatedUser { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedUser { get; set; }
    public string? DeletedUser { get; set; }
    public DateTime? DeletedDate { get; set; }
    public IEnumerable<ZipCodeDto> ZipCodes { get; set; } = new List<ZipCodeDto>();
}
