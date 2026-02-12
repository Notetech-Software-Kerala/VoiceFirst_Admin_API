using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
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
    private readonly ICountryRepo _countryRepo;

    public PostOfficeService(IMapper mapper, IPostOfficeRepo repo, ICountryRepo countryRepo)
    {
        _mapper = mapper;
        _repo = repo;
        _countryRepo = countryRepo;
    }

    public async Task<ApiResponse<PostOfficeDto>> CreateAsync(PostOfficeCreateDto dto, int loginId, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            return ApiResponse<PostOfficeDto>.Fail(Messages.PayloadRequired, StatusCodes.Status400BadRequest);
        // validate divisions and country
        if (!dto.CountryId.HasValue)
            return ApiResponse<PostOfficeDto>.Fail(Messages.CountryRequired, StatusCodes.Status400BadRequest);

        // Division hierarchy checks
        if (dto.DivTwoId.HasValue && !dto.DivOneId.HasValue)
            return ApiResponse<PostOfficeDto>.Fail(Messages.DivisionOneRequiredForDivisionTwo, StatusCodes.Status400BadRequest);
        if (dto.DivThreeId.HasValue && !dto.DivTwoId.HasValue)
            return ApiResponse<PostOfficeDto>.Fail(Messages.DivisionTwoRequiredForDivisionThree, StatusCodes.Status400BadRequest);

        // check existence using country repo
        var countryDivs = await _countryRepo.ExistsCountryAndDivisionsAsync(dto.CountryId.Value, dto.DivOneId, dto.DivTwoId, dto.DivThreeId, cancellationToken);
        if (!countryDivs.CountryExists)
            return ApiResponse<PostOfficeDto>.Fail(Messages.CountryNotFound, StatusCodes.Status404NotFound);
        if (dto.DivOneId.HasValue && !countryDivs.DivOneExists)
            return ApiResponse<PostOfficeDto>.Fail(Messages.DivisionOneNotFound, StatusCodes.Status404NotFound);
        if (dto.DivTwoId.HasValue && !countryDivs.DivTwoExists)
            return ApiResponse<PostOfficeDto>.Fail(Messages.DivisionTwoNotFound, StatusCodes.Status404NotFound);
        if (dto.DivThreeId.HasValue && !countryDivs.DivThreeExists)
            return ApiResponse<PostOfficeDto>.Fail(Messages.DivisionThreeNotFound, StatusCodes.Status404NotFound);

        var existingEntity = await _repo.ExistsByNameAsync(dto.PostOfficeName, null, cancellationToken);

        if (existingEntity != null && existingEntity.IsDeleted == true)
            return ApiResponse<PostOfficeDto>.Fail(Messages.PostOfficeNameExistsInTrash, StatusCodes.Status422UnprocessableEntity);

        if (existingEntity != null)
            return ApiResponse<PostOfficeDto>.Fail(Messages.PostOfficeNameAlreadyExists, StatusCodes.Status409Conflict);

        var entity = new PostOffice
        {
            PostOfficeName = dto.PostOfficeName,
            CountryId = dto.CountryId,
            DivisionOneId = dto.DivOneId,
            DivisionTwoId = dto.DivTwoId,
            DivisionThreeId = dto.DivThreeId,
            CreatedBy = loginId
        };

        var created = await _repo.CreateAsync(entity, dto.ZipCodes, cancellationToken);

        var createdEntity = await _repo.GetByIdAsync(created.PostOfficeId, cancellationToken);
        if (createdEntity == null)
            return ApiResponse<PostOfficeDto>.Fail(Messages.SomethingWentWrong, StatusCodes.Status500InternalServerError);
        
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
    public async Task<ApiResponse<IEnumerable<ZipCodeLookUp>?>> GetZipCodesByPostOfficeIdAsync(int id,
    int? placeId = null, CancellationToken cancellationToken = default)
    {
        var postOffice = await _repo.GetByIdAsync(id, cancellationToken);
        if (postOffice == null || postOffice.IsDeleted==true)
            return ApiResponse<IEnumerable<ZipCodeLookUp>?>.Fail(Messages.PostOfficeNotFound, StatusCodes.Status404NotFound);

        var entity = await _repo.GetZipCodesByPostOfficeIdAsync(id, placeId, cancellationToken);
        if (entity == null) return null;
        var dto = _mapper.Map<IEnumerable<ZipCodeLookUp>>(entity);
        return ApiResponse<IEnumerable<ZipCodeLookUp>?>.Ok(dto, Messages.ZipCodesRetrieveSucessfully, StatusCodes.Status200OK);
     
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

    
    public async Task<ApiResponse<IEnumerable<PostOfficeLookupDto>>> GetLookupAsync(PostOfficeLookUpFilterDto filter, CancellationToken cancellationToken = default)
    {
        // Division hierarchy checks
        if (filter.DivOneId.HasValue && !filter.CountryId.HasValue)
            return ApiResponse<IEnumerable<PostOfficeLookupDto>>.Fail(Messages.CountryRequiredForDivisionOne, StatusCodes.Status400BadRequest);
        if (filter.DivTwoId.HasValue && !filter.DivOneId.HasValue)
            return ApiResponse<IEnumerable<PostOfficeLookupDto>>.Fail(Messages.DivisionOneRequiredForDivisionTwo, StatusCodes.Status400BadRequest);
        if (filter.DivThreeId.HasValue && !filter.DivTwoId.HasValue)
            return ApiResponse<IEnumerable<PostOfficeLookupDto>>.Fail(Messages.DivisionTwoRequiredForDivisionThree, StatusCodes.Status400BadRequest);

        // check existence using country repo
        if (filter.CountryId.HasValue)
        {
            var countryDivs = await _countryRepo.ExistsCountryAndDivisionsAsync(filter.CountryId.Value, filter.DivOneId, filter.DivTwoId, filter.DivThreeId, cancellationToken);
            if (!countryDivs.CountryExists)
                return ApiResponse<IEnumerable<PostOfficeLookupDto>>.Fail(Messages.CountryNotFound, StatusCodes.Status404NotFound);
            if (filter.DivOneId.HasValue && !countryDivs.DivOneExists)
                return ApiResponse<IEnumerable<PostOfficeLookupDto>>.Fail(Messages.DivisionOneNotFound, StatusCodes.Status404NotFound);
            if (filter.DivTwoId.HasValue && !countryDivs.DivTwoExists)
                return ApiResponse<IEnumerable<PostOfficeLookupDto>>.Fail(Messages.DivisionTwoNotFound, StatusCodes.Status404NotFound);
            if (filter.DivThreeId.HasValue && !countryDivs.DivThreeExists)
                return ApiResponse<IEnumerable<PostOfficeLookupDto>>.Fail(Messages.DivisionThreeNotFound, StatusCodes.Status404NotFound);
        }

        PostOfficeLookUpWithZipCodeFilterDto filterInput = _mapper.Map<PostOfficeLookUpWithZipCodeFilterDto>(filter);

        var entities = await _repo.GetLookupAsync(filterInput, cancellationToken);
        if (entities == null) return null;
        var dto = _mapper.Map<IEnumerable<PostOfficeLookupDto>>(entities);
        return ApiResponse<IEnumerable<PostOfficeLookupDto>>.Ok(dto, Messages.PostOfficeRetrieveSucessfully, StatusCodes.Status200OK);
       
    }
    public async Task<ApiResponse<IEnumerable<PostOfficeDetailLookupDto>>> GetPostOfficeDetailsByZipCodeAsync(string zipCode, CancellationToken cancellationToken = default)
    {
        

        PostOfficeLookUpWithZipCodeFilterDto filterInput = new PostOfficeLookUpWithZipCodeFilterDto();
        filterInput.ZipCode = zipCode;

        var entities = await _repo.GetLookupAsync(filterInput, cancellationToken);
        if (entities == null) return null;
        var dto = _mapper.Map<IEnumerable<PostOfficeDetailLookupDto>>(entities);
        return ApiResponse<IEnumerable<PostOfficeDetailLookupDto>>.Ok(dto, Messages.PostOfficeRetrieveSucessfully, StatusCodes.Status200OK);
       
    }

    public async Task<ApiResponse<PostOfficeDto>> UpdateAsync(PostOfficeUpdateDto dto, int id, int loginId, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            return ApiResponse<PostOfficeDto>.Fail(Messages.PayloadRequired, StatusCodes.Status400BadRequest);
        // validate divisions and country when any of those fields are present
        if (dto.PostOfficeName != null || dto.Active!=null || dto.CountryId!=null || dto.DivOneId.HasValue || dto.DivTwoId.HasValue || dto.DivThreeId.HasValue)
        {

            // when updating divisions, ensure hierarchy rules
            if (dto.DivTwoId.HasValue && !dto.DivOneId.HasValue)
                return ApiResponse<PostOfficeDto>.Fail(Messages.DivisionOneRequiredForDivisionTwo, StatusCodes.Status400BadRequest);
            if (dto.DivThreeId.HasValue && !dto.DivTwoId.HasValue)
                return ApiResponse<PostOfficeDto>.Fail(Messages.DivisionTwoRequiredForDivisionThree, StatusCodes.Status400BadRequest);

            if (dto.CountryId.HasValue || dto.DivOneId.HasValue || dto.DivTwoId.HasValue || dto.DivThreeId.HasValue)
            {
                if (!dto.CountryId.HasValue)
                    return ApiResponse<PostOfficeDto>.Fail(Messages.CountryRequired, StatusCodes.Status400BadRequest);

                var countryDivs = await _countryRepo.ExistsCountryAndDivisionsAsync(dto.CountryId, dto.DivOneId, dto.DivTwoId, dto.DivThreeId, cancellationToken);
                if (!countryDivs.CountryExists)
                    return ApiResponse<PostOfficeDto>.Fail(Messages.CountryNotFound, StatusCodes.Status404NotFound);
                if (dto.DivOneId.HasValue && !countryDivs.DivOneExists)
                    return ApiResponse<PostOfficeDto>.Fail(Messages.DivisionOneNotFound, StatusCodes.Status404NotFound);
                if (dto.DivTwoId.HasValue && !countryDivs.DivTwoExists)
                    return ApiResponse<PostOfficeDto>.Fail(Messages.DivisionTwoNotFound, StatusCodes.Status404NotFound);
                if (dto.DivThreeId.HasValue && !countryDivs.DivThreeExists)
                    return ApiResponse<PostOfficeDto>.Fail(Messages.DivisionThreeNotFound, StatusCodes.Status404NotFound);
            }

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
            DivisionOneId = dto.DivOneId,
            DivisionTwoId = dto.DivTwoId,
            DivisionThreeId = dto.DivThreeId,
                IsActive = dto.Active,
                UpdatedBy = loginId
            };

            var ok = await _repo.UpdateAsync(entity, cancellationToken);
            if (!ok)
                return ApiResponse<PostOfficeDto>.Fail(Messages.NotFound, StatusCodes.Status404NotFound);
        }
        if (dto.UpdateZipCodes.Count() > 0)
        {
            var zipEntities = new List<PostOfficeZipCode>();
            foreach (var z in dto.UpdateZipCodes)
            {
                
                zipEntities.Add(new PostOfficeZipCode
                {
                    PostOfficeZipCodeLinkId = z.ZipCodeLinkId ?? 0,
                    PostOfficeId = id,
                    IsActive = z.Active,
                    ZipCode = z.ZipCode.Trim()?? "",
                    UpdatedBy = loginId,
                });
            }

            var result=await _repo.BulkUpdateZipCodesAsync(id, zipEntities, cancellationToken);
            if (result != null)
            {
                return ApiResponse<PostOfficeDto>.Fail(result.Message, result.StatuaCode);
            }
        }
        if (dto.AddZipCodes.Count() > 0)
        {
            

            var result = await _repo.BulkInsertZipCodesAsync(id, dto.AddZipCodes, loginId, cancellationToken);
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
        var zips = await _repo.GetZipCodesByPostOfficeIdAsync(entity.PostOfficeId,null, cancellationToken);
        dto.ZipCodes = _mapper.Map<IEnumerable<ZipCodeDto>>(zips);
        return dto;
    }
}
