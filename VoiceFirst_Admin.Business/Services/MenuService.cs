using AutoMapper;
using Microsoft.AspNetCore.Http;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Menu;
using VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;
using VoiceFirst_Admin.Utilities.DTOs.Features.Role;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
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
    public async Task<ApiResponse<object>> BulkUpdateWebMenusAsync(WebMenuBulkUpdateDto dto, int loginId, CancellationToken cancellationToken = default)
    {
        if (dto == null) return ApiResponse<object>.Fail(Messages.PayloadRequired);

        var ok = await _repo.BulkUpdateWebMenusAsync(dto, loginId, cancellationToken);
        if (ok!=null) 
            return ApiResponse<object>.Fail(ok.Message, ok.StatuaCode);
        return ApiResponse<object>.Ok(null!, Messages.WebMenuUpdatedSucessfully);
    }
    public async Task<ApiResponse<object>> BulkUpdateAppMenusAsync(AppMenuBulkUpdateDto dto, int loginId, CancellationToken cancellationToken = default)
    {
        if (dto == null) return ApiResponse<object>.Fail(Messages.PayloadRequired);

        var ok = await _repo.BulkUpdateAppMenusAsync(dto, loginId, cancellationToken);
        if (ok!=null) 
            return ApiResponse<object>.Fail(ok.Message, ok.StatuaCode);
        return ApiResponse<object>.Ok(null, Messages.AppMenuUpdatedSucessfully);
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
                     .Where(id => id > 0)
                     .Select(id => id!)
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

        return ApiResponse<MenuMasterDto>.Ok(list, Messages.MenuCreated, StatusCodes.Status201Created);
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
    public async Task<MenuMasterDetailDto> GetAllMenuMastersByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var data = await _repo.GetMenuMastersByIdAsync(id,cancellationToken);
        if (data == null)
            return null;

        var menuDetails= _mapper.Map<MenuMasterDetailDto>(data);
        var menuPrograms = await _repo.GetAllMenuProrgamByMenuMastersIdAsync(id, cancellationToken);
        if (menuPrograms.Count() > 0)
        {
            var menuProgramsList= _mapper.Map<List<MenuProgramLinkDto>>(menuPrograms);
            menuDetails.menuProgramLinks = menuProgramsList;

        }
        return menuDetails;


    }
    public async Task<ApiResponse<MenuMasterDto>> UpdateMenuMasterAsync(int id,MenuMasterUpdateDto dto,int loginId,CancellationToken cancellationToken = default)
    {
        if (dto == null)
            return ApiResponse<MenuMasterDto>.Fail(Messages.PayloadRequired);

        var existingEntity = await _repo.GetMenuMastersByIdAsync(id, cancellationToken);
        if (existingEntity == null)
            return ApiResponse<MenuMasterDto>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);

        // platform id fallback (do this early)
        var plateFormId = (dto.PlateFormId.HasValue && dto.PlateFormId.Value > 0)
            ? dto.PlateFormId.Value
            : existingEntity.ApplicationId;

        var hasProgramsToAdd = dto.ProgramIds != null && dto.ProgramIds.Any();
        var hasProgramsToUpdate = dto.UpdateProgramIds != null && dto.UpdateProgramIds.Any();

        var hasMenuChanges =
            dto.MenuName != null ||
            dto.Icon != null ||
            dto.Route != null ||          // allow "" to clear
            dto.Active.HasValue ||
            (dto.PlateFormId.HasValue && dto.PlateFormId.Value > 0) ||
            hasProgramsToAdd ||
            hasProgramsToUpdate;

        // -------------------------
        // 1) Update MenuMaster + Program links
        // -------------------------
        if (hasMenuChanges)
        {
            var entity = _mapper.Map<MenuMaster>(dto);
            entity.MenuMasterId = id;
            entity.UpdatedBy = loginId;

            // ensure platform id is set if your repo expects it
            entity.ApplicationId = plateFormId;

            var programIdsModel = hasProgramsToAdd
                ? _mapper.Map<List<MenuProgramLink>>(dto.ProgramIds)
                : new List<MenuProgramLink>();

            var updateProgramIdsModel = hasProgramsToUpdate
                ? _mapper.Map<List<MenuProgramLink>>(dto.UpdateProgramIds)
                : new List<MenuProgramLink>();

            // Validate ProgramIds (ADD)
            if (hasProgramsToAdd)
            {
                var programIds = dto.ProgramIds
                    .Select(x => x.ProgramId)
                    .Where(x =>  x > 0)
                    .Select(x => x!)
                    .Distinct()
                    .ToList();

                if (programIds.Count == 0)
                    return ApiResponse<MenuMasterDto>.Fail(Messages.BadRequest, StatusCodes.Status400BadRequest);

                var invalidIds = await _sysProgramRepo
                    .GetInvalidProgramIdsForApplicationAsync(plateFormId, programIds, cancellationToken);

                if (invalidIds.Any())
                {
                    return ApiResponse<MenuMasterDto>.Fail(
                        string.Format(Messages.InvalidActionLinksForApplication, string.Join(", ", invalidIds)),
                        StatusCodes.Status400BadRequest);
                }
            }

            // Validate UpdateProgramIds (UPDATE)
            if (hasProgramsToUpdate)
            {
                var programIds = dto.UpdateProgramIds
                    .Select(x => x.ProgramId)
                    .Where(x =>  x > 0)
                    .Select(x => x!)
                    .Distinct()
                    .ToList();

                if (programIds.Count == 0)
                    return ApiResponse<MenuMasterDto>.Fail(Messages.BadRequest, StatusCodes.Status400BadRequest);

                var invalidIds = await _sysProgramRepo
                    .GetInvalidProgramIdsForApplicationAsync(plateFormId, programIds, cancellationToken);

                if (invalidIds.Any())
                {
                    return ApiResponse<MenuMasterDto>.Fail(
                        string.Format(Messages.InvalidActionLinksForApplication, string.Join(", ", invalidIds)),
                        StatusCodes.Status400BadRequest);
                }
            }

            var err = await _repo.UpdateMenuMasterAsync(entity, programIdsModel, updateProgramIdsModel, cancellationToken);
            if (err != null)
                return ApiResponse<MenuMasterDto>.Fail(err.Message, err.StatuaCode);
        }

        // -------------------------
        // 2) Create Web menu (if requested)
        // -------------------------
        if (dto.Web == true)
        {
            var existingWbEntity = await _repo.ExistsMenuMasterByWebAsync(id, null, cancellationToken);

            if (existingWbEntity != null && existingWbEntity.IsDeleted == true)
                return ApiResponse<MenuMasterDto>.Fail(Messages.WebMenuExistsInTrash, StatusCodes.Status422UnprocessableEntity);

            if (existingWbEntity != null)
                return ApiResponse<MenuMasterDto>.Fail(Messages.MenuMasterAlreadyExistsInWeb, StatusCodes.Status409Conflict);

            var webMenuId = await _repo.CreateWebMenuAsync(id, loginId, cancellationToken);
            if (webMenuId <= 0)
                return ApiResponse<MenuMasterDto>.Fail(Messages.SomethingWentWrong, StatusCodes.Status500InternalServerError);
        }

        // -------------------------
        // 3) Create App menu (if requested)
        // -------------------------
        if (dto.App == true)
        {
            var existingAppEntity = await _repo.ExistsMenuMasterByAppAsync(id, null, cancellationToken);

            if (existingAppEntity != null && existingAppEntity.IsDeleted == true)
                return ApiResponse<MenuMasterDto>.Fail(Messages.AppMenuExistsInTrash, StatusCodes.Status422UnprocessableEntity);

            if (existingAppEntity != null)
                return ApiResponse<MenuMasterDto>.Fail(Messages.MenuMasterAlreadyExistsInApp, StatusCodes.Status409Conflict);

            var appMenuId = await _repo.CreateAppMenuAsync(id, loginId, cancellationToken);
            if (appMenuId <= 0)
                return ApiResponse<MenuMasterDto>.Fail(Messages.SomethingWentWrong, StatusCodes.Status500InternalServerError);
        }

        // -------------------------
        // 4) Return updated menu
        // -------------------------
        var menuMaster = await _repo.GetMenuMastersByIdAsync(id, cancellationToken);
        var result = _mapper.Map<MenuMasterDto>(menuMaster);

        return ApiResponse<MenuMasterDto>.Ok(result, Messages.MenuUpdatedSucessfully);
    }
}
