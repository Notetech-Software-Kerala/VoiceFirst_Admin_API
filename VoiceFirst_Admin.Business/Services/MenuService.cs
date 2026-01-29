using System.Collections.Generic;
using System.Linq;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Menu;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services;

public class MenuService : IMenuService
{
    private readonly IMenuRepo _repo;

    public MenuService(IMenuRepo repo)
    {
        _repo = repo;
    }

    public async Task<ApiResponse<object>> CreateAsync(MenuCreateDto dto, int loginId, CancellationToken cancellationToken = default)
    {
        if (dto == null) return ApiResponse<object>.Fail(Messages.PayloadRequired);

        // basic entity mapping
        var entity = new MenuMaster
        {
            MenuName = dto.MenuName,
            MenuIcon = dto.Icon,
            MenuRoute = dto.Route,
            ApplicationId = dto.PlateFormId,
            CreatedBy = loginId
        };

        var createdId = await _repo.CreateMenuAsync(entity, dto.ProgramId ?? new List<int>(), dto.Web, dto.App, loginId, cancellationToken);
        return ApiResponse<object>.Ok(new { MenuMasterId = createdId }, Messages.Created, Microsoft.AspNetCore.Http.StatusCodes.Status201Created);
    }

    public async Task<ApiResponse<object>> UpdateAsync(int id, MenuUpdateDto dto, int loginId, CancellationToken cancellationToken = default)
    {
        if (dto == null) return ApiResponse<object>.Fail(Messages.PayloadRequired);

        var entity = new MenuMaster
        {
            MenuMasterId = id,
            MenuName = dto.MenuName ?? string.Empty,
            MenuIcon = dto.Icon ?? string.Empty,
            MenuRoute = dto.Route ?? string.Empty,
            ApplicationId = dto.PlateFormId ?? default,
            UpdatedBy = loginId
        };

        var ok = await _repo.UpdateMenuAsync(entity, dto.AddProgramId ?? new List<int>(), dto.UpdateProgramId ?? new List<ProgramStatusDto>(), dto.Web, dto.App, loginId, cancellationToken);
        if (!ok) return ApiResponse<object>.Fail(Messages.NotFound, Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound);
        return ApiResponse<object>.Ok(null!, Messages.Updated);
    }

    public async Task<MenuMasterDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var m = await _repo.GetByIdAsync(id, cancellationToken);
        if (m == null) return null;
        return new MenuMasterDto
        {
            MenuMasterId = m.MenuMasterId,
            MenuName = m.MenuName,
            MenuIcon = m.MenuIcon,
            MenuRoute = m.MenuRoute,
            ApplicationId = m.ApplicationId,
            Active = m.IsActive ?? false,
            Deleted = m.IsDeleted ?? false,
            CreatedUser = m.CreatedUserName ?? string.Empty,
            CreatedDate = m.CreatedAt ?? System.DateTime.MinValue,
            ModifiedUser = m.UpdatedUserName ?? string.Empty,
            ModifiedDate = m.UpdatedAt,
            DeletedUser = m.DeletedUserName ?? string.Empty,
            DeletedDate = m.DeletedAt
        };
    }

    public async Task<IEnumerable<MenuMasterDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _repo.GetAllAsync(cancellationToken);
        return rows.Select(m => new MenuMasterDto
        {
            MenuMasterId = m.MenuMasterId,
            MenuName = m.MenuName,
            MenuIcon = m.MenuIcon,
            MenuRoute = m.MenuRoute,
            ApplicationId = m.ApplicationId,
            Active = m.IsActive ?? false,
            Deleted = m.IsDeleted ?? false,
            CreatedUser = m.CreatedUserName ?? string.Empty,
            CreatedDate = m.CreatedAt ?? System.DateTime.MinValue,
            ModifiedUser = m.UpdatedUserName ?? string.Empty,
            ModifiedDate = m.UpdatedAt,
            DeletedUser = m.DeletedUserName ?? string.Empty,
            DeletedDate = m.DeletedAt
        }).ToList();
    }

    public async Task<MenuDetailDto?> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repo.GetDetailByIdAsync(id, cancellationToken);
    }
}
