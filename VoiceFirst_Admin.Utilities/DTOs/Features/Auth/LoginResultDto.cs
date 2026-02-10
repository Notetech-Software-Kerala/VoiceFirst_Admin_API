namespace VoiceFirst_Admin.Utilities.DTOs.Features.Auth;

public class LoginResultDto
{
    public LoginResponseDto Response { get; set; } = new();
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiresAtUtc { get; set; }
}
