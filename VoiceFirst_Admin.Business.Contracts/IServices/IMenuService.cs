using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.DTOs.Features.Menu;

namespace VoiceFirst_Admin.Business.Contracts.IServices;

public interface IMenuService
{
    Task<ApiResponse<object>> CreateAsync(MenuCreateDto dto, int loginId, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> UpdateAsync(int id, MenuUpdateDto dto, int loginId, CancellationToken cancellationToken = default);
    Task<MenuMasterDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuMasterDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MenuDetailDto?> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default);
}
