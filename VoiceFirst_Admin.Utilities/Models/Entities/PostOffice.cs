using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class PostOffice : BaseModel
{
    public int PostOfficeId { get; set; }
    public string PostOfficeName { get; set; } = string.Empty;
    public int? CountryId { get; set; }
}
