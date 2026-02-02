using VoiceFirst_Admin.Utilities.DTOs.Features.Menu;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories;

public interface IMenuRepo
{
    Task<int> CreateMenuAsync(MenuMaster menu, List<MenuProgramLink> programIds, bool web, bool app, int loginId, CancellationToken cancellationToken = default);

    Task<int> AddWebMenuAsync(
    int menuMasterId,
    int createdBy,
    int parentWebMenuId = 0,
    CancellationToken cancellationToken = default);

    Task<int> AddAppMenuAsync(
    int menuMasterId,
    int createdBy,
    int parentAppMenuId = 0,
    CancellationToken cancellationToken = default);

    Task<PagedResultDto<MenuMaster>> GetAllMenuMastersAsync(MenuFilterDto filter, CancellationToken cancellationToken = default);
    Task<bool> UpdateMenuMasterAsync(MenuMaster entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<WebMenu>> GetAllWebMenusAsync( CancellationToken cancellationToken = default);
    Task<IEnumerable<AppMenus>> GetAllAppMenusAsync( CancellationToken cancellationToken = default);
}
