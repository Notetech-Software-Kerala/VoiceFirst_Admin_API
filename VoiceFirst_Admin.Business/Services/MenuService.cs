using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Menu;
using VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;
using VoiceFirst_Admin.Utilities.DTOs.Features.Role;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    public async Task<ApiResponse<object>> BulkUpdateWebMenusAsync(VoiceFirst_Admin.Utilities.DTOs.Features.Menu.WebMenuBulkUpdateDto dto, int loginId, CancellationToken cancellationToken = default)
    {
        if (dto == null) return ApiResponse<object>.Fail(Messages.PayloadRequired);

        var ok = await _repo.BulkUpdateWebMenusAsync(dto, loginId, cancellationToken);
        if (ok!=null) 
            return ApiResponse<object>.Fail(ok.Message, ok.StatuaCode);
        return ApiResponse<object>.Ok(null!, Messages.Updated);
    }

    public async Task<ApiResponse<MenuMasterDto>> CreateAsync(MenuCreateDto dto, int loginId, CancellationToken cancellationToken = default)
    {
        if (dto == null) return ApiResponse<MenuMasterDto>.Fail(Messages.PayloadRequired);
        // If Route is empty => this is a main menu. Main menus don't have program links.
        var isMainMenu = string.IsNullOrWhiteSpace(dto.Route);
        if (!isMainMenu && dto.ProgramIds != null && dto.ProgramIds.Any())
        {
            var programIds = dto.ProgramIds
                     .Select(x => x.ProgramId)
                     .Where(id => id.HasValue && id.Value > 0)
                     .Select(id => id!.Value)
                     .Distinct()
                     .ToList();

            if (programIds.Count == 0)
            {
                return ApiResponse<MenuMasterDto>.Fail(
                    Messages.BadRequest,
                    StatusCodes.Status400BadRequest);
            }
            var invalidIds = await _sysProgramRepo.GetInvalidProgramIdsForApplicationAsync(dto.PlateFormId, programIds, cancellationToken);
            if (invalidIds.Any())
            {
                return ApiResponse<MenuMasterDto>.Fail(
                    string.Format(Messages.InvalidActionLinksForApplication, string.Join(", ", invalidIds)),
                    StatusCodes.Status400BadRequest);
            }
        }
            

        // basic entity mapping
        var entity = _mapper.Map<MenuMaster>(dto);
        entity.CreatedBy = loginId;

        // If this is a main menu (no route) there should be no program links and no application id
        var programIdsModel = isMainMenu ? null : _mapper.Map<List<MenuProgramLink>>(dto.ProgramIds);
        

        var createdId = await _repo.CreateMenuAsync(entity, programIdsModel, dto.Web, dto.App, loginId, cancellationToken);
        if(createdId<=0)
            return ApiResponse<MenuMasterDto>.Fail(Messages.SomethingWentWrong, StatusCodes.Status500InternalServerError);
        var menuMaster = await _repo.GetMenuMastersByIdAsync(createdId, cancellationToken);
        var list = _mapper.Map<MenuMasterDto>(menuMaster);

        return ApiResponse<MenuMasterDto>.Ok(list, Messages.Created, StatusCodes.Status201Created);
    }

    public async Task<PagedResultDto<MenuMasterDto>> GetAllMenuMastersAsync(MenuFilterDto filter, CancellationToken cancellationToken = default)
    {
        var data = await _repo.GetAllMenuMastersAsync(filter, cancellationToken);
        var list = _mapper.Map<List<MenuMasterDto>>(data.Items);
        return new PagedResultDto<MenuMasterDto>
        {
            Items = list,
            TotalCount = data.TotalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.Limit
        };
    }

    public async Task<List<WebMenuDto>> GetAllWebMenusAsync(CancellationToken cancellationToken = default)
    {
        var data = await _repo.GetAllWebMenusAsync(cancellationToken);
        var list = _mapper.Map<List<WebMenuDto>>(data);
        return list;
    }

    public async Task<List<AppMenuDto>> GetAllAppMenusAsync(CancellationToken cancellationToken = default)
    {
        var data = await _repo.GetAllAppMenusAsync(cancellationToken);
        var list = _mapper.Map<List<AppMenuDto>>(data);
        return list;
    }
    public async Task<MenuMasterDto> GetAllMenuMastersByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var data = await _repo.GetMenuMastersByIdAsync(id,cancellationToken);
        return _mapper.Map<MenuMasterDto>(data);
        
    }
    public async Task<ApiResponse<MenuMasterDto>> UpdateMenuMasterAsync(int id, MenuMasterUpdateDto dto, int loginId, CancellationToken cancellationToken = default)
    {
        if (dto == null) return ApiResponse<MenuMasterDto>.Fail(Messages.PayloadRequired);

        var isMainMenu = string.IsNullOrWhiteSpace(dto.Route);

        var entity = new Utilities.Models.Entities.MenuMaster
        {
            MenuMasterId = id,
            MenuName = dto.MenuName ?? string.Empty,
            MenuIcon = dto.Icon ?? string.Empty,
            MenuRoute = dto.Route ?? string.Empty,
            ApplicationId = dto.PlateFormId?? 0,
            UpdatedBy = loginId,
            IsActive = dto.Active ?? true
        };

        var ok = await _repo.UpdateMenuMasterAsync(entity, cancellationToken);
        if (!ok) return ApiResponse<MenuMasterDto>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);

        // If this was changed/cleared to be a main menu, remove any program links
        //if (isMainMenu)
        //{
        //    await _repo.DeleteMenuProgramLinksAsync(id, cancellationToken);
        //}
        var menuMaster = await _repo.GetMenuMastersByIdAsync(id, cancellationToken);
        var list = _mapper.Map<MenuMasterDto>(menuMaster);
        return ApiResponse<MenuMasterDto>.Ok(null!, Messages.Updated);
    }



}
