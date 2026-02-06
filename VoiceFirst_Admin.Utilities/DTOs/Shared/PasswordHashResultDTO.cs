namespace VoiceFirst_Admin.Utilities.DTOs.Shared
{
    public class PasswordHashResultDTO
    {
        public string Hash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
    }
}
