namespace VoiceFirst_Admin.Utilities.DTOs.Features.Auth;

public class ResetPasswordDto
{
    public string NewPassword { get; set; } = string.Empty;
    public string? PasswordResetGrant { get; set; }
}
