using System;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity
{
    public class SysBusinessActivityDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool? Status { get; set; }
    }
}
