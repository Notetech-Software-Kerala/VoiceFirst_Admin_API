using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services;

public class PostOfficeService : IPostOfficeService
{
    private readonly IMapper _mapper;
    private readonly IPostOfficeRepo _repo;

    public PostOfficeService(IMapper mapper, IPostOfficeRepo repo)
    {
        _mapper = mapper;
        _repo = repo;
    }

    public async Task<ApiResponse<PostOfficeDto>> CreateAsync(PostOfficeCreateDto dto, int loginId, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            return ApiResponse<PostOfficeDto>.Fail(Messages.PayloadRequired, StatusCodes.Status400BadRequest);

        var existingEntity = await _repo.ExistsByNameAsync(dto.PostOfficeName, null, cancellationToken);

        if (existingEntity != null && existingEntity.IsDeleted == true)
            return ApiResponse<PostOfficeDto>.Fail(Messages.PostOfficeNameExistsInTrash, StatusCodes.Status422UnprocessableEntity);

        if (existingEntity != null)
            return ApiResponse<PostOfficeDto>.Fail(Messages.PostOfficeNameAlreadyExists, StatusCodes.Status409Conflict);

        var entity = new PostOffice
        {
            PostOfficeName = dto.PostOfficeName,
            CountryId = dto.CountryId,
            CreatedBy = loginId
        };

        var created = await _repo.CreateAsync(entity, cancellationToken);

        var createdEntity = await _repo.GetByIdAsync(created.PostOfficeId, cancellationToken);
        if (createdEntity == null)
            return ApiResponse<PostOfficeDto>.Fail(Messages.SomethingWentWrong, StatusCodes.Status500InternalServerError);
        // insert zip codes for this post office, if any
        var zipEntities = new List<PostOfficeZipCode>();
        foreach (var z in dto.ZipCodes)
        {
            if (string.IsNullOrWhiteSpace(z)) continue;
            zipEntities.Add(new PostOfficeZipCode
            {
                PostOfficeId = createdEntity.PostOfficeId,
                ZipCode = z.Trim(),
                CreatedBy = loginId
            });
        }

        if (zipEntities.Count > 0)
        {
            await _repo.BulkUpsertZipCodesAsync(createdEntity.PostOfficeId, zipEntities, cancellationToken);
        }

        var createdDto = await MapWithZipCodesAsync(createdEntity, cancellationToken);

        return ApiResponse<PostOfficeDto>.Ok(createdDto, Messages.PostOfficeCreated, StatusCodes.Status201Created);
    }

    public async Task<PostOfficeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken);
        if (entity == null) return null;
        var dto = await MapWithZipCodesAsync(entity, cancellationToken);
        return dto;
    }

    public async Task<PagedResultDto<PostOfficeDto>> GetAllAsync(PostOfficeFilterDto filter, CancellationToken cancellationToken = default)
    {
        var entities = await _repo.GetAllAsync(filter, cancellationToken);
        var list = new List<PostOfficeDto>();
        foreach (var e in entities.Items)
        {
            list.Add(await MapWithZipCodesAsync(e, cancellationToken));
        }
        return new PagedResultDto<PostOfficeDto>
        {
            Items = list,
            TotalCount = entities.TotalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.Limit
        };
    }

    public async Task<IEnumerable<PostOfficeLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repo.GetLookupAsync(cancellationToken);
        if (entities == null) return null;
        var dto = _mapper.Map<IEnumerable<PostOfficeLookupDto>>(entities);
        return dto;
    }

    public async Task<ApiResponse<PostOfficeDto>> UpdateAsync(PostOfficeUpdateDto dto, int id, int loginId, CancellationToken cancellationToken = default)
    {
        if (dto.PostOfficeName != null || dto.Active!=null || dto.CountryId!=null)
        {

            var existing = await _repo.ExistsByNameAsync(dto.PostOfficeName ?? string.Empty, id, cancellationToken);
            if (existing != null)
            {
                if (existing.IsDeleted == true)
                    return ApiResponse<PostOfficeDto>.Fail(Messages.PostOfficeNameExistsInTrash, StatusCodes.Status422UnprocessableEntity);

                return ApiResponse<PostOfficeDto>.Fail(Messages.PostOfficeNameAlreadyExists, StatusCodes.Status409Conflict);
            }
            var entity = new PostOffice
            {
                PostOfficeId = id,
                PostOfficeName = dto.PostOfficeName ?? string.Empty,
                CountryId = dto.CountryId,
                IsActive = dto.Active,
                UpdatedBy = loginId
            };

            var ok = await _repo.UpdateAsync(entity, cancellationToken);
            if (!ok)
                return ApiResponse<PostOfficeDto>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);
        }
        else if (dto.ZipCodes.Count() > 0)
        {
            var zipEntities = new List<PostOfficeZipCode>();
            foreach (var z in dto.ZipCodes)
            {
                
                zipEntities.Add(new PostOfficeZipCode
                {
                    PostOfficeZipCodeId = z.ZipCodeId ?? 0,
                    PostOfficeId = id,
                    IsActive = z.Active,
                    ZipCode = z.ZipCode.Trim()?? "",
                    UpdatedBy = loginId,
                    CreatedBy = loginId,
                });
            }

            var result=await _repo.BulkUpsertZipCodesAsync(id, zipEntities, cancellationToken);
            if (result != null)
            {
                return ApiResponse<PostOfficeDto>.Fail(result.Message, result.StatuaCode);
            }
        }


        var updatedEntity = await _repo.GetByIdAsync(id, cancellationToken);
        if (updatedEntity == null)
            return ApiResponse<PostOfficeDto>.Fail(Messages.SomethingWentWrong, StatusCodes.Status500InternalServerError);
        // sync zip codes: add new, update existing, remove missing
        

        var updatedDto = await MapWithZipCodesAsync(updatedEntity, cancellationToken);
        return ApiResponse<PostOfficeDto>.Ok(updatedDto, Messages.PostOfficeUpdatedSucessfully, StatusCodes.Status200OK);
    }

    public async Task<ApiResponse<object>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);

        if (entity.IsDeleted == true)
            return ApiResponse<object>.Fail(Messages.PostOfficeAlreadyDeleted, StatusCodes.Status400BadRequest);

        var ok = await _repo.DeleteAsync(new PostOffice
        {
            PostOfficeId = id,
            DeletedBy = loginId
        }, cancellationToken);

        if (!ok)
            return ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);

        return ApiResponse<object>.Ok(null!, Messages.PostOfficeDeleteSucessfully, StatusCodes.Status200OK);
    }

    public async Task<ApiResponse<object>> RestoreAsync(int id, int loginId, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);

        if (!entity.IsDeleted == true)
            return ApiResponse<object>.Fail(Messages.PostOfficeAlreadyRestored, StatusCodes.Status400BadRequest);

        var ok = await _repo.RestoreAsync(new PostOffice
        {
            PostOfficeId = id,
            UpdatedBy = loginId
        }, cancellationToken);

        if (!ok)
            return ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);

        return ApiResponse<object>.Ok(null!, Messages.PostOfficeRestoreSucessfully, StatusCodes.Status200OK);
    }
    public async Task<ApiResponse<object>> DeleteZipCodeAsync(int id, int loginId, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetZipCodeByIdAsync(id, cancellationToken);
        if (entity == null)
            return ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);

        if (entity.IsDeleted == true)
            return ApiResponse<object>.Fail(Messages.PostOfficeAlreadyDeleted, StatusCodes.Status400BadRequest);

        var ok = await _repo.DeleteZipCodeAsync(new PostOfficeZipCode
        {
            PostOfficeZipCodeId = id,
            DeletedBy = loginId
        }, cancellationToken);

        if (!ok)
            return ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);

        return ApiResponse<object>.Ok(null!, Messages.PostOfficeZipCodeDeleteSucessfully, StatusCodes.Status200OK);
    }

    public async Task<ApiResponse<object>> RestoreZipCodeAsync(int id, int loginId, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetZipCodeByIdAsync(id, cancellationToken);
        if (entity == null)
            return ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);

        if (!entity.IsDeleted == true)
            return ApiResponse<object>.Fail(Messages.PostOfficeAlreadyRestored, StatusCodes.Status400BadRequest);

        var ok = await _repo.RestoreZipCodeAsync(new PostOfficeZipCode
        {
            PostOfficeZipCodeId = id,
            UpdatedBy = loginId
        }, cancellationToken);

        if (!ok)
            return ApiResponse<object>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);

        return ApiResponse<object>.Ok(null!, Messages.PostOfficeZipCodeRestoreSucessfully, StatusCodes.Status200OK);
    }

    public async Task<PostOfficeDto?> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.ExistsByNameAsync(name, excludeId, cancellationToken);
        if (entity == null) return null;
        var dto = await MapWithZipCodesAsync(entity, cancellationToken);
        return dto;
    }

    private async Task<PostOfficeDto> MapWithZipCodesAsync(PostOffice entity, CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<PostOfficeDto>(entity);
        var zips = await _repo.GetZipCodesByPostOfficeIdAsync(entity.PostOfficeId, cancellationToken);
        dto.ZipCodes = _mapper.Map<IEnumerable<ZipCodeDto>>(zips);
        return dto;
    }
}
