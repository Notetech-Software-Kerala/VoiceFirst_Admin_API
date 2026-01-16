using System;

namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity
{
    public class SysBusinessActivityDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
