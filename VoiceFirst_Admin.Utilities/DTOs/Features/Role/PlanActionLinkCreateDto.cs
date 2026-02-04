using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.Role;

public class PlanActionLinkCreateDto
{
    public int PlanId { get; set; }
    public List<int> ActionLinkIds { get; set; } = new List<int>();
}