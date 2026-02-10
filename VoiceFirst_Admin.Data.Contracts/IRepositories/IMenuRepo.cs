using System.Data;
using VoiceFirst_Admin.Utilities.DTOs.Features.Menu;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories;

public interface IMenuRepo
{
    Task<int> CreateMenuAsync(MenuMaster menu, List<MenuProgramLink> programIds, bool web, bool app, int loginId, CancellationToken cancellationToken = default);
    Task<int> AddWebMenuAsync(int menuMasterId,int createdBy,int parentWebMenuId = 0,CancellationToken cancellationToken = default);
    Task<int> AddAppMenuAsync(int menuMasterId,int createdBy,int parentAppMenuId = 0,CancellationToken cancellationToken = default);
    Task<PagedResultDto<MenuMaster>> GetAllMenuMastersAsync(MenuFilterDto filter, CancellationToken cancellationToken = default);
    Task<MenuMaster> GetMenuMastersByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<BulkUpsertError> UpdateMenuMasterAsync(MenuMaster entity, List<MenuProgramLink>? addProgramIds, List<MenuProgramLink>? updateProgramIds, CancellationToken cancellationToken = default);
    Task<IEnumerable<WebMenu>> GetAllWebMenusAsync( CancellationToken cancellationToken = default);
    Task<IEnumerable<AppMenus>> GetAllAppMenusAsync( CancellationToken cancellationToken = default);
    Task<BulkUpsertError> BulkUpdateWebMenusAsync(WebMenuBulkUpdateDto dto, int loginId, CancellationToken cancellationToken = default);
    Task<BulkUpsertError> BulkUpdateAppMenusAsync(AppMenuBulkUpdateDto dto, int loginId, CancellationToken cancellationToken = default);
    Task<bool> DeleteMenuProgramLinksAsync(IDbConnection connection, IDbTransaction tx, int menuMasterId, int loginId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuProgramLink>> GetAllMenuProrgamByMenuMastersIdAsync(int menuMastersId, CancellationToken cancellationToken = default);
    Task<MenuMaster?> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<WebMenu?> ExistsMenuMasterByWebAsync(int menuMasterId, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<AppMenus?> ExistsMenuMasterByAppAsync(int menuMasterId, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<int> CreateAppMenuAsync(int menuMasterId, int createdBy, CancellationToken cancellationToken);
    Task<int> CreateWebMenuAsync(int menuMasterId, int createdBy, CancellationToken cancellationToken);
}
