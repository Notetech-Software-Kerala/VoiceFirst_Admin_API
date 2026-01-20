using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class ProgramActionService : IProgramActionService
    {
        private readonly IMapper _mapper;
        private readonly IProgramActionRepo _repo;

        public ProgramActionService(IMapper mapper, IProgramActionRepo repo)
        {
            _mapper = mapper;
            _repo = repo;
        }

        public async Task<ApiResponse<ProgramActionDto>> CreateAsync(
                    ProgramActionCreateDto dto, int loginId, CancellationToken cancellationToken = default)
        {
            if (dto == null)
                return ApiResponse<ProgramActionDto>.Fail(Messages.PayloadRequired, StatusCodes.Status400BadRequest);

            // Check existing by name
            var existingEntity = await _repo.ExistsByNameAsync(dto.ProgramActionName, null, cancellationToken);

            if (existingEntity != null && existingEntity.IsDeleted==true)
                return ApiResponse<ProgramActionDto>.Fail(Messages.NameExistsInTrash, StatusCodes.Status422UnprocessableEntity);

            if (existingEntity != null)
                return ApiResponse<ProgramActionDto>.Fail(Messages.NameAlreadyExists, StatusCodes.Status409Conflict);

            // Create
            var entity = new SysProgramActions
            {
                ProgramActionName = dto.ProgramActionName,
                CreatedBy = loginId
            };

            var created = await _repo.CreateAsync(entity, cancellationToken);

            // Read back (if you need mapped fields)
            var createdEntity = await _repo.GetByIdAsync(created.SysProgramActionId, cancellationToken);
            if (createdEntity == null)
                return ApiResponse<ProgramActionDto>.Fail(Messages.SomethingWentWrong, StatusCodes.Status500InternalServerError);

            var createdDto = _mapper.Map<ProgramActionDto>(createdEntity);

            return ApiResponse<ProgramActionDto>.Ok(createdDto, Messages.ProgramActionCreated, StatusCodes.Status201Created);
        }
        public async Task<ProgramActionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repo.GetByIdAsync(id, cancellationToken);
            if (entity == null) return null;
            var dto = _mapper.Map<ProgramActionDto>(entity);
            return dto;
        }

        public async Task<PagedResultDto<ProgramActionDto>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default)
        {
            var entities = await _repo.GetAllAsync(filter, cancellationToken);
            var list = _mapper.Map<IEnumerable<ProgramActionDto>>(entities.Items);
            return new PagedResultDto<ProgramActionDto>
            {
                Items = list,
                TotalCount = entities.TotalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.Limit
            };
        }
        public async Task<IEnumerable<ProgramActionLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _repo.GetLookupAsync( cancellationToken);
            if (entities == null) return null;
            var dto = _mapper.Map<IEnumerable<ProgramActionLookupDto>>(entities);
            return dto;

        }
        public async Task<ApiResponse<ProgramActionDto>> UpdateAsync(ProgramActionUpdateDto dto,int id,int loginId, CancellationToken cancellationToken = default)
        {
            // name uniqueness (exclude current id)
            var existing = await _repo.ExistsByNameAsync(dto.ActionName ?? string.Empty, id, cancellationToken);
            if (existing != null)
            {
                // if you want special message when existing is deleted:
                if (existing.IsDeleted==true)
                    return ApiResponse<ProgramActionDto>.Fail(Messages.NameExistsInTrash, StatusCodes.Status422UnprocessableEntity);

                return ApiResponse<ProgramActionDto>.Fail(Messages.NameAlreadyExists, StatusCodes.Status409Conflict);
            }

            var entity = new SysProgramActions
            {
                SysProgramActionId = id,
                ProgramActionName = dto.ActionName ?? string.Empty,
                IsActive = dto.Active,
                UpdatedBy = loginId
            };

            var ok = await _repo.UpdateAsync(entity, cancellationToken);
            if (!ok)
                return ApiResponse<ProgramActionDto>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);

            var updatedEntity = await _repo.GetByIdAsync(id, cancellationToken);
            if (updatedEntity == null)
                return ApiResponse<ProgramActionDto>.Fail(Messages.SomethingWentWrong, StatusCodes.Status500InternalServerError);

            var updatedDto = _mapper.Map<ProgramActionDto>(updatedEntity);
            return ApiResponse<ProgramActionDto>.Ok(updatedDto, Messages.Updated, StatusCodes.Status200OK);
        }
        public async Task<ApiResponse<object>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {

            var entity = await _repo.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                return ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);

            if (entity.IsDeleted==true)
                return ApiResponse<object>.Fail(Messages.ProgramActionAlreadyDeleted, StatusCodes.Status400BadRequest);

            var ok = await _repo.DeleteAsync(new SysProgramActions
            {
                SysProgramActionId = id,
                DeletedBy = loginId
            }, cancellationToken);

            if (!ok)
                return ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);

            return ApiResponse<object>.Ok(null!, Messages.ProgramActionDeleteSucessfully, StatusCodes.Status200OK);
        }
        public async Task<ApiResponse<object>> RestoreAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {

            var entity = await _repo.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                return ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);

            if (!entity.IsDeleted==true)
                return ApiResponse<object>.Fail(Messages.ProgramActionAlreadyRestored, StatusCodes.Status400BadRequest);

            var ok = await _repo.RestoreAsync(new SysProgramActions
            {
                SysProgramActionId = id,
                UpdatedBy = loginId
            }, cancellationToken);

            if (!ok)
                return ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);

            return ApiResponse<object>.Ok(null!, Messages.ProgramActionRestoreSucessfully, StatusCodes.Status200OK);
        }

        public async Task<ProgramActionDto> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var entity = await _repo.ExistsByNameAsync(name, excludeId, cancellationToken);
            if (entity == null) return null;
            var dto = _mapper.Map<ProgramActionDto>(entity);
            return dto;
        }

    }
}
