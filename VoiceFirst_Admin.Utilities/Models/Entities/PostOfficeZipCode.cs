using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class PostOfficeZipCode : BaseModel
{
    public int PostOfficeZipCodeId { get; set; }
    public int? PostOfficeId { get; set; }
    public string ZipCode { get; set; } = string.Empty;
}
