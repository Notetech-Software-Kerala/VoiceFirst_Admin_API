namespace VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;

public class PostOfficeLookupDto
{
    public int PostOfficeId { get; set; }
    public string PostOfficeName { get; set; } = string.Empty;
}
public class PostOfficeDetailLookupDto
{
    public int PostOfficeId { get; set; }
    public string PostOfficeName { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public int? CountryId { get; set; }
    public int DivOneId { get; set; }
    public string DivOneName { get; set; } = string.Empty;
    public int DivTwoId { get; set; }
    public string DivTwoName { get; set; } = string.Empty;
    public int DivThreeId { get; set; }
    public string DivThreeName { get; set; } = string.Empty;
}
