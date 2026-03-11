using VoiceFirst_Admin.Utilities.DTOs.Features.ApplicationVersion;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices;

public interface IApplicationVersionService
{
    Task<ApiResponse<PlatformVersionDto>> CreateAsync(PlatformVersionCreateDto dto, int loginId, CancellationToken cancellationToken = default);
}
