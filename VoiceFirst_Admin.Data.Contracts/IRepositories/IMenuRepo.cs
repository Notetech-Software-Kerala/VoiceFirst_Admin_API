using VoiceFirst_Admin.Utilities.DTOs.Features.Menu;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories;

public interface IMenuRepo
{
    Task<int> CreateMenuAsync(MenuMaster menu, IEnumerable<int> programIds, MenuWebDto? web, MenuAppDto? app, int loginId, CancellationToken cancellationToken = default);

    Task<bool> UpdateMenuAsync(MenuMaster menu, IEnumerable<int> addProgramIds, IEnumerable<ProgramStatusDto> updateProgramIds, MenuWebDto? web, MenuAppDto? app, int loginId, CancellationToken cancellationToken = default);
    Task<MenuMaster?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuMaster>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MenuDetailDto?> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default);
}
