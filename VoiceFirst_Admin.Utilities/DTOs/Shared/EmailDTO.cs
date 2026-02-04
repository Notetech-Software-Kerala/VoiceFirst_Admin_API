namespace VoiceFirst_Admin.Utilities.DTOs.Shared
{
    public class EmailDTO
    {
        public string t8_from_email { get; set; } = string.Empty;
        public string? t8_from_email_password { get; set; }
        public string t8_to_email { get; set; } = string.Empty;
        public string t8_email_subject { get; set; } = string.Empty;
        public string? t8_email_html_body { get; set; }
        public string? t8_signature_content { get; set; }
    }
}
