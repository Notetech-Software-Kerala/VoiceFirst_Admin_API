using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Role;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services;

public class RoleService : IRoleService
{
    private readonly IMapper _mapper;
    private readonly IRoleRepo _repo;
    private readonly IPlanService _planService;
    private readonly ISysProgramRepo _sysProgramRepo;

    public RoleService(IMapper mapper, IRoleRepo repo, IPlanService planService, ISysProgramRepo sysProgramRepo)
    {
        _mapper = mapper;
        _repo = repo;
        _planService = planService;
        _sysProgramRepo = sysProgramRepo;
    }

    public async Task<ApiResponse<RoleDto>> CreateAsync(RoleCreateDto dto, int loginId, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            return ApiResponse<RoleDto>.Fail(Messages.PayloadRequired, StatusCodes.Status400BadRequest);

        var existing = await _repo.ExistsByNameAsync(dto.RoleName, null, cancellationToken);
        if (existing != null)
        {
            // System roles (protected)
            if (existing.SysRoleId is >= 1 and <= 5)
                return ApiResponse<RoleDto>.Fail(
                    Messages.RoleNameDefault,
                    StatusCodes.Status403Forbidden);

            if (existing.IsDeleted == true)
                return ApiResponse<RoleDto>.Fail(
                    Messages.RoleNameExistsInTrash,
                    StatusCodes.Status422UnprocessableEntity);

            return ApiResponse<RoleDto>.Fail(
                Messages.RoleNameAlreadyExists,
                StatusCodes.Status409Conflict);
        }
        foreach(var plan in dto.CreatePlanActionLink)
        {
            if (plan.ActionLinkIds != null && plan.ActionLinkIds.Count() > 0)
            {
                var list = await _sysProgramRepo.GetInvalidProgramActionLinkIdsForApplicationAsync(
                    dto.PlatformId,
                    plan.ActionLinkIds,
                    cancellationToken);
                if (list.Any())
                {
                    return ApiResponse<RoleDto>.Fail(
                        string.Format(Messages.InvalidActionLinksForApplication, string.Join(", ", list)),
                        StatusCodes.Status400BadRequest);
                }
            }
        }
        
        var entity = new SysRoles
        {
            RoleName = dto.RoleName,
            IsMandatory =false,
            RolePurpose = dto.RolePurpose,
            ApplicationId = dto.PlatformId,
            CreatedBy = loginId
        };

        var created = await _repo.CreateAsync(entity, dto.CreatePlanActionLink, cancellationToken);
        if (created == null)
            return ApiResponse<RoleDto>.Fail(Messages.SomethingWentWrong, StatusCodes.Status500InternalServerError);

        var createdEntity = await _repo.GetByIdAsync(created.SysRoleId, cancellationToken);
        if (createdEntity == null)
            return ApiResponse<RoleDto>.Fail(Messages.SomethingWentWrong, StatusCodes.Status500InternalServerError);
        

        var dtoOut = _mapper.Map<RoleDto>(createdEntity);
      
   
        return ApiResponse<RoleDto>.Ok(dtoOut, Messages.RoleCreated, StatusCodes.Status201Created);
    }
    public async Task<PlanRoleActionLinkDetailsDto?> GetByPlanIdAsync(PlanRoleDto planRoleDto, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(planRoleDto.RoleId, cancellationToken);
        if (entity == null) return null;
        var links = await _repo.GetActionIdsByRoleIdAsync(planRoleDto.RoleId ,cancellationToken);
        
        PlanRoleActionLinkDetailsDto planRoleActionLinkDetails = new PlanRoleActionLinkDetailsDto();
        if (links.Count() > 0)
        {
            var actionLinksMap = _mapper.Map<IEnumerable<PlanRoleActionLinkDto>>(links);
            planRoleActionLinkDetails.PlanRoleLinkId = links.FirstOrDefault().PlanRoleLinkId;
            planRoleActionLinkDetails.PlanActionLink = actionLinksMap.ToList();
        }

        return planRoleActionLinkDetails;
    }
    public async Task<RoleDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken);
        if (entity == null) return null;
        var dto = _mapper.Map<RoleDetailDto>(entity);
        var links = await _repo.GetActionIdsByRoleIdAsync(id, cancellationToken);
        List<PlanRoleActionLinkDetailsDto> planRoleActionLinkDetails =
    links?
    .GroupBy(x => new { x.PlanId, x.PlanRoleLinkId })
    .Select(g => new PlanRoleActionLinkDetailsDto
    {
        PlanId = g.Key.PlanId,
        PlanRoleLinkId = g.Key.PlanRoleLinkId,
        PlanName = g.FirstOrDefault()?.PlanName,
        PlanActionLink = _mapper.Map<List<PlanRoleActionLinkDto>>(g.ToList())
    })
    .ToList()
    ?? new List<PlanRoleActionLinkDetailsDto>();
        dto.PlanRoleActionLink = planRoleActionLinkDetails;
        return dto;
    }

    public async Task<PagedResultDto<RoleDto>> GetAllAsync(RoleFilterDto filter, CancellationToken cancellationToken = default)
    {
        var entities = await _repo.GetAllAsync(filter, cancellationToken);
        var list = _mapper.Map<IEnumerable<RoleDto>>(entities.Items);
        // load actions for each? skip for performance
        return new PagedResultDto<RoleDto>
        {
            Items = list,
            TotalCount = entities.TotalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.Limit
        };
    }

    public async Task<IEnumerable<RoleLookUpDto>> GetLookUpAllAsync( 
        int AppplicationId,
        CancellationToken cancellationToken = default)
    {
        var dto = await _repo.GetLookUpAllAsync(AppplicationId, cancellationToken);
        return dto;
    }

    public async Task<ApiResponse<RoleDto>> UpdateAsync(RoleUpdateDto dto, int id, int loginId, CancellationToken cancellationToken = default)
    {
        var roleDetails = await _repo.GetByIdAsync(id, cancellationToken);
        if (dto.RoleName!=null || dto.RolePurpose!=null || dto.PlatformId != null)
        {
            if (dto.RoleName != null)
            {
                var existing = await _repo.ExistsByNameAsync(dto.RoleName ?? string.Empty, id, cancellationToken);
                if (existing != null)
                {
                    // System roles (protected)
                    if (existing.SysRoleId is >= 1 and <= 5)
                        return ApiResponse<RoleDto>.Fail(
                            Messages.RoleNameDefault,
                            StatusCodes.Status403Forbidden);

                    if (existing.IsDeleted == true)
                        return ApiResponse<RoleDto>.Fail(
                            Messages.RoleNameExistsInTrash,
                            StatusCodes.Status422UnprocessableEntity);

                    return ApiResponse<RoleDto>.Fail(
                        Messages.RoleNameAlreadyExists,
                        StatusCodes.Status409Conflict);
                }
            }

            var entity = new SysRoles
            {
                SysRoleId = id,
                RoleName = dto.RoleName ?? string.Empty,
                IsMandatory = false,
                RolePurpose = dto.RolePurpose,
                ApplicationId = dto.PlatformId ?? default,
                UpdatedBy = loginId
            };

            var ok = await _repo.UpdateAsync(entity, cancellationToken);
            if (!ok)
                return ApiResponse<RoleDto>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);
        }
       
        // update action links
        if (dto.CreatePlanActionLink != null && dto.CreatePlanActionLink.Count()>0)
        {
            foreach (var item in dto.CreatePlanActionLink)
            {
                var invalidIds = await _sysProgramRepo.GetInvalidProgramActionLinkIdsForApplicationAsync(
                roleDetails.ApplicationId ,
                item.ActionLinkIds,
                cancellationToken);

                if (invalidIds.Any())
                {
                    return ApiResponse<RoleDto>.Fail(
                        string.Format(Messages.InvalidActionLinksForApplication, string.Join(", ", invalidIds)),
                        StatusCodes.Status400BadRequest);
                }
                var addError = await _repo.AddRoleActionLinksAsync(
                    id,
                    roleDetails.ApplicationId,
                    dto.CreatePlanActionLink,
                    loginId,
                    cancellationToken);

                if (addError != null)
                    return ApiResponse<RoleDto>.Fail(addError.Message, addError.StatuaCode);
            }
            
        }
        if (dto.UpdatePlanActionLinks != null)
        {
            //var actionLinksIds = dto.UpdateActionLinks.Select(x => x.ActionLinkId).ToList();
            //var list = await _sysProgramRepo.GetInvalidProgramActionLinkIdsForApplicationAsync(
            //    dto.PlatformId ?? 0,
            //    actionLinksIds,
            //    cancellationToken);

            //if (list.Any())
            //{
            //    return ApiResponse<RoleDetailDto>.Fail(
            //        string.Format(Messages.InvalidActionLinksForApplication, string.Join(", ", list)),
            //        StatusCodes.Status400BadRequest);
            //}
            var updateError = await _repo.UpdateRoleActionLinksAsync(
               id,
               dto.PlatformId ?? 0,
               dto.UpdatePlanActionLinks,
               loginId,
               cancellationToken);

            if (updateError != null)
                return ApiResponse<RoleDto>.Fail(updateError.Message, updateError.StatuaCode);
        }
       
        var updated = await _repo.GetByIdAsync(id, cancellationToken);
        if (updated == null) return null;
        var updatedData = _mapper.Map<RoleDto>(updated);
       
        //var actionLinksMap = _mapper.Map<IEnumerable<PlanRoleActionLinkDto>>(links);
        //updatedData.ActionLinks = actionLinksMap.ToList();
        return ApiResponse<RoleDto>.Ok(updatedData,Messages.Success, StatusCodes.Status200OK); ;
    }

    public async Task<ApiResponse<object>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);
        if (entity.IsDeleted == true)
            return ApiResponse<object>.Fail(Messages.RoleAlreadyDeleted, StatusCodes.Status400BadRequest);
        var ok = await _repo.DeleteAsync(new SysRoles { SysRoleId = id, DeletedBy = loginId }, cancellationToken);
        if (!ok)
            return ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);
        return ApiResponse<object>.Ok(null!, Messages.RoleDeleteSucessfully, StatusCodes.Status200OK);
    }

    public async Task<ApiResponse<object>> RestoreAsync(int id, int loginId, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);
        if (!entity.IsDeleted == true)
            return ApiResponse<object>.Fail(Messages.RoleAlreadyRestored, StatusCodes.Status400BadRequest);
        var ok = await _repo.RestoreAsync(new SysRoles { SysRoleId = id, UpdatedBy = loginId }, cancellationToken);
        if (!ok)
            return ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);
        return ApiResponse<object>.Ok(null!, Messages.RoleRestoreSucessfully, StatusCodes.Status200OK);
    }

    public async Task<RoleDto?> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.ExistsByNameAsync(name, excludeId, cancellationToken);
        if (entity == null) return null;
        var dto = _mapper.Map<RoleDto>(entity);
        return dto;
    }

    
}
