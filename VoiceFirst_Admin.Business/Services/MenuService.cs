using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Menu;
using VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;
using VoiceFirst_Admin.Utilities.DTOs.Features.Role;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services;

public class MenuService : IMenuService
{
    private readonly IMenuRepo _repo;
    private readonly IMapper _mapper;
    private readonly ISysProgramRepo _sysProgramRepo;
    public MenuService(IMenuRepo repo, IMapper mapper, ISysProgramRepo sysProgramRepo)
    {
        _repo = repo;
        _mapper = mapper;
        _sysProgramRepo = sysProgramRepo;
    }

    public async Task<ApiResponse<object>> CreateAsync(MenuCreateDto dto, int loginId, CancellationToken cancellationToken = default)
    {
        if (dto == null) return ApiResponse<object>.Fail(Messages.PayloadRequired);

        if (dto.ProgramIds != null && dto.ProgramIds.Any())
        {
            var programIds = dto.ProgramIds
                     .Select(x => x.ProgramId)
                     .Where(id => id.HasValue && id.Value > 0)
                     .Select(id => id!.Value)
                     .Distinct()
                     .ToList();

            if (programIds.Count == 0)
            {
                return ApiResponse<object>.Fail(
                    Messages.BadRequest,
                    StatusCodes.Status400BadRequest);
            }
            var invalidIds = await _sysProgramRepo.GetInvalidProgramIdsForApplicationAsync(dto.PlateFormId, programIds, cancellationToken);
            if (invalidIds.Any())
            {
                return ApiResponse<object>.Fail(
                    string.Format(Messages.InvalidActionLinksForApplication, string.Join(", ", invalidIds)),
                    StatusCodes.Status400BadRequest);
            }
        }
            

        // basic entity mapping
        var entity = _mapper.Map<MenuMaster>(dto);
        entity.CreatedBy = loginId;

        var programIdsModel = _mapper.Map<List<MenuProgramLink>>(dto.ProgramIds);

        var createdId = await _repo.CreateMenuAsync(entity, programIdsModel, dto.Web, dto.App, loginId, cancellationToken);
        return ApiResponse<object>.Ok(new { MenuMasterId = createdId }, Messages.Created, StatusCodes.Status201Created);
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
