using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.DTOs.Features.Menu;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Business.Contracts.IServices;

public interface IMenuService
{
    Task<ApiResponse<object>> CreateAsync(MenuCreateDto dto, int loginId, CancellationToken cancellationToken = default);
    Task<PagedResultDto<MenuMasterDto>> GetAllMenuMastersAsync(MenuFilterDto filter, CancellationToken cancellationToken = default);
    Task<List<WebMenuDto>> GetAllWebMenusAsync( CancellationToken cancellationToken = default); 
    Task<ApiResponse<object>> UpdateMenuMasterAsync(int id, MenuMasterUpdateDto dto, int loginId, CancellationToken cancellationToken = default);
    Task<List<AppMenuDto>> GetAllAppMenusAsync( CancellationToken cancellationToken = default);
}
