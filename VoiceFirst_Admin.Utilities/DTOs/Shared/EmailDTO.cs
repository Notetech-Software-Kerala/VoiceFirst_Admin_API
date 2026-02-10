namespace VoiceFirst_Admin.Utilities.DTOs.Shared
{
    public class EmailDTO
    {
        public string from_email { get; set; } = string.Empty;
        public string? from_email_password { get; set; }
        public string to_email { get; set; } = string.Empty;
        public string email_subject { get; set; } = string.Empty;
        public string? email_html_body { get; set; }
        public string? signature_content { get; set; }
    }
}
