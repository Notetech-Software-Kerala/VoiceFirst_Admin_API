using System;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Models.Entities;

public class PostOfficeZipCode : BaseModel
{
    public int PostOfficeZipCodeLinkId { get; set; }
    public int PostOfficeId { get; set; }
    // ZipCode now comes from master ZipCode table via PostOfficeZipCodeLink
    public string? ZipCode { get; set; }
}
