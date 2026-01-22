using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysProgramActionLink
{
    public class SysProgramActionLinkUpdateDTO
    {
        public int ActionId { get; set; }
        public bool Active { get; set; } = true;
    }
}
