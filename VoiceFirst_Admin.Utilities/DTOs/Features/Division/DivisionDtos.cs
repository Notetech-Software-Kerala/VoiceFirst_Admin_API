using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Division;

public class DivisionOneDto 
{
    public int DivOneId { get; set; }
    public string DivOneName { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public string CountryName { get; set; }

    public bool? Active { get; set; }
    public bool? Deleted { get; set; }
}

public class DivisionTwoDto 
{
    public int DivTwoId { get; set; }
    public string DivTwoName { get; set; } = string.Empty;
    public string DivOneName { get; set; } = string.Empty;
    public int DivOneId { get; set; }

    public bool? Active { get; set; }
    public bool? Deleted { get; set; }
}

public class DivisionThreeDto 
{
    public int DivThreeId { get; set; }
    public string DivThreeName { get; set; } = string.Empty;
    public string DivTwoName { get; set; } = string.Empty;
    public int DivTwoId { get; set; }

    public bool? Active { get; set; }
    public bool? Deleted { get; set; }
}
