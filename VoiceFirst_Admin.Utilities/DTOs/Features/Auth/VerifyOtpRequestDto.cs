namespace VoiceFirst_Admin.Utilities.DTOs.Features.Auth;

public class VerifyOtpRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
    public string? ResetToken { get; set; }
}
