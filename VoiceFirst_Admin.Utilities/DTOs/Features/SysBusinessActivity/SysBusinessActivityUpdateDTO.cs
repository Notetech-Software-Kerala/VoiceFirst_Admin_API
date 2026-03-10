using System;
using VoiceFirst_Admin.Utilities.DTOs.Features.BusinessActivityUserCustomFieldLink;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;

public class SysBusinessActivityUpdateDTO
{

    public string? ActivityName { get; set; }
    public bool? Active { get; set; }
    public List<int>? addCustomFieldIds { get; set; }
    public List<UpdateBusinessActivityUserCustomFieldDto>? updateCustomField { get; set; }

}
