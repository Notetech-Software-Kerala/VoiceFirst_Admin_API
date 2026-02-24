namespace VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueCharacterType
{
    public class SysIssueCharacterTypeDTO
    {
        public int IssueCharacterTypeId { get; set; }
        public string IssueCharacterType { get; set; } = string.Empty;
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string CreatedUser { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string ModifiedUser { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string DeletedUser { get; set; } = string.Empty;
        public DateTime? DeletedDate { get; set; }
    }
}
