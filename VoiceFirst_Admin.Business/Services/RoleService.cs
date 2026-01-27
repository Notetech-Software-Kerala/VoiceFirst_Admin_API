using AutoMapper;
using Microsoft.AspNetCore.Http;
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

    public RoleService(IMapper mapper, IRoleRepo repo)
    {
        _mapper = mapper;
        _repo = repo;
    }

    public async Task<ApiResponse<RoleDto>> CreateAsync(RoleCreateDto dto, int loginId, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            return ApiResponse<RoleDto>.Fail(Messages.PayloadRequired, StatusCodes.Status400BadRequest);

        var existing = await _repo.ExistsByNameAsync(dto.RoleName, null, cancellationToken);
        if (existing != null && existing.IsDeleted == true)
            return ApiResponse<RoleDto>.Fail(Messages.RoleNameExistsInTrash, StatusCodes.Status422UnprocessableEntity);
        if (existing != null)
            return ApiResponse<RoleDto>.Fail(Messages.RoleNameAlreadyExists, StatusCodes.Status409Conflict);

        var entity = new SysRoles
        {
            RoleName = dto.RoleName,
            IsMandatory =false,
            RolePurpose = dto.RolePurpose,
            ApplicationId = dto.ApplicationId,
            CreatedBy = loginId
        };

        var created = await _repo.CreateAsync(entity, dto.ActionLinkIds, cancellationToken);
        var createdEntity = await _repo.GetByIdAsync(created.SysRoleId, cancellationToken);
        if (createdEntity == null)
            return ApiResponse<RoleDto>.Fail(Messages.SomethingWentWrong, StatusCodes.Status500InternalServerError);

        

        var dtoOut = _mapper.Map<RoleDto>(createdEntity);
        var createdLinks = await _repo.GetActionIdsByRoleIdAsync(createdEntity.SysRoleId, cancellationToken);
   
        return ApiResponse<RoleDto>.Ok(dtoOut, Messages.RoleCreated, StatusCodes.Status201Created);
    }

    public async Task<RoleDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken);
        if (entity == null) return null;
        var dto = _mapper.Map<RoleDetailDto>(entity);
        var links = await _repo.GetActionIdsByRoleIdAsync(id, cancellationToken);
        var actionLinksMap = _mapper.Map<IEnumerable<RoleActionLinkDto>>(links);
        dto.ActionLinks = actionLinksMap.ToList();
        return dto;
    }

    public async Task<PagedResultDto<RoleDto>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default)
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

    public async Task<ApiResponse<RoleDto>> UpdateAsync(RoleUpdateDto dto, int id, int loginId, CancellationToken cancellationToken = default)
    {
        if(dto.RoleName!=null || dto.RolePurpose!=null || dto.ApplicationId != null)
        {
            if (dto.RoleName != null)
            {
                var existing = await _repo.ExistsByNameAsync(dto.RoleName ?? string.Empty, id, cancellationToken);
                if (existing != null)
                {
                    if (existing.IsDeleted == true)
                        return ApiResponse<RoleDto>.Fail(Messages.RoleNameExistsInTrash, StatusCodes.Status422UnprocessableEntity);
                    return ApiResponse<RoleDto>.Fail(Messages.RoleNameAlreadyExists, StatusCodes.Status409Conflict);
                }
            }

            var entity = new SysRoles
            {
                SysRoleId = id,
                RoleName = dto.RoleName ?? string.Empty,
                IsMandatory = false,
                RolePurpose = dto.RolePurpose,
                ApplicationId = dto.ApplicationId ?? default,
                UpdatedBy = loginId
            };

            var ok = await _repo.UpdateAsync(entity, cancellationToken);
            if (!ok)
                return ApiResponse<RoleDto>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);
        }
       
        // update action links
        else if (dto.ActionLinkId != null)
        {
            await _repo.BulkUpsertRoleActionLinksAsync(id, dto.ApplicationId??0, dto.ActionLinkId, loginId, cancellationToken);
        }
        else
        {
            return ApiResponse<RoleDto>.Fail(Messages.BadRequest, StatusCodes.Status400BadRequest);
        }
        var updated = await _repo.GetByIdAsync(id, cancellationToken);
        if (updated == null)
            return ApiResponse<RoleDto>.Fail(Messages.SomethingWentWrong, StatusCodes.Status500InternalServerError);
        var outDto = _mapper.Map<RoleDto>(updated);
        return ApiResponse<RoleDto>.Ok(outDto, Messages.RoleUpdatedSucessfully, StatusCodes.Status200OK);
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
