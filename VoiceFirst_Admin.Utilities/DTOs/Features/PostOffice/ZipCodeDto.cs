namespace VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;

public class ZipCodeDto
{
    public int? ZipCodeId { get; set; }
    public string ZipCode { get; set; } = string.Empty;
    public bool? Active { get; set; }
    public bool? Deleted { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? CreatedUser { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedUser { get; set; }
}
