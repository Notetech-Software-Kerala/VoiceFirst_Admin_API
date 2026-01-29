using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Place;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class PlaceService: IPlaceService
    {
        private readonly IPlaceRepo _repo;
        private readonly IMapper _mapper;
        public PlaceService(
            IPlaceRepo repository,
            IMapper mapper)
        {
            _repo = repository;
            _mapper = mapper;
        }


        public async Task<ApiResponse<PlaceDTO>> CreateAsync(
          PlaceCreateDTO dto,
          int loginId,
          CancellationToken cancellationToken)
        {

            var existingEntity = await _repo.PlaceExistsAsync(
                dto.PlaceName,
                null,
                cancellationToken);

            if (existingEntity.PlaceId <= 0)
            {
                if (!existingEntity.Deleted)
                {
                    // ❌ Active duplicate

                    return ApiResponse<PlaceDTO>.Fail
                       (Messages.PlaceAlreadyExists,
                       StatusCodes.Status409Conflict,
                       ErrorCodes.PlaceAlreadyExists
                       );
                }
                return ApiResponse<PlaceDTO>.Fail(
                     Messages.PlaceAlreadyExistsRecoverable,
                     StatusCodes.Status422UnprocessableEntity,
                      ErrorCodes.PlaceAlreadyExistsRecoverable,
                      new PlaceDTO
                      {
                          PlaceId = existingEntity.PlaceId
                      }
                 );
            }


            var entity = _mapper.Map<Place>(dto);
            entity.CreatedBy = loginId;

            entity.PlaceId =
                await _repo.CreateAsync(entity, cancellationToken);

            if (entity.PlaceId <= 0)
            {
                return ApiResponse<PlaceDTO>.Fail(
                    Messages.SomethingWentWrong,
                    StatusCodes.Status500InternalServerError,
                    ErrorCodes.InternalServerError);
            }

            var createdDto =
                await _repo.GetByIdAsync
                (entity.PlaceId,
                cancellationToken);

            return ApiResponse<PlaceDTO>.Ok(
                createdDto,
                Messages.PlaceCreated,
                StatusCodes.Status201Created);
        }


        public async Task<ApiResponse<PlaceDTO>?> GetByIdAsync(
          int id,
          CancellationToken cancellationToken = default)
        {
            var dto = await _repo.GetByIdAsync(id, cancellationToken);

            if (dto == null)
            {

                return ApiResponse<PlaceDTO>.Fail(
                   Messages.PlaceNotFoundById,
                   StatusCodes.Status404NotFound,
                    ErrorCodes.PlaceNotFoundById);
            }
            return ApiResponse<PlaceDTO>.Ok(
                dto,
                Messages.PlaceRetrieved,
                StatusCodes.Status200OK);
        }


        public async Task<ApiResponse<PagedResultDto<PlaceDTO>>>
        GetAllAsync(PlaceFilterDTO filter,
            CancellationToken cancellationToken = default)
        {
            var result = await _repo.GetAllAsync(filter, cancellationToken);

            return ApiResponse<PagedResultDto<PlaceDTO>>.Ok(
                result,
                result.TotalCount == 0
                    ? Messages.PlacesNotFound
                    : Messages.PlacesRetrieved,
                 statusCode: StatusCodes.Status200OK
            );
        }



        public async Task<ApiResponse<List<PlaceLookUpDTO>>>
        GetLookUpAsync(CancellationToken cancellationToken)
        {
            var result = await _repo.GetActiveAsync(cancellationToken)
                         ?? new List<PlaceLookUpDTO>();

            return ApiResponse<List<PlaceLookUpDTO>>.Ok(
                result,
                result.Count == 0
                    ? Messages.NoActivePlaces
                    : Messages.PlacesRetrieved,
                statusCode: StatusCodes.Status200OK
            );
        }


        public async Task<ApiResponse<PlaceDTO>> UpdateAsync(
          PlaceUpdateDTO dto,
          int placeId,
          int loginId,
          CancellationToken cancellationToken = default)
        {
            var existDto =
                await _repo.IsIdExistAsync(placeId,
                cancellationToken);


            if (existDto == null)
            {

                return ApiResponse<PlaceDTO>.Fail(
                   Messages.PlaceNotFoundById,
                   StatusCodes.Status404NotFound,
                    ErrorCodes.PlaceNotFoundById);
            }
            else if (existDto.Deleted)
            {


                return ApiResponse<PlaceDTO>.Fail(
                 Messages.PlaceNotFound,
                 StatusCodes.Status409Conflict,
                  ErrorCodes.PlaceNotFound);

            }

            // uniqueness check ONLY if name is patched
            if (!string.IsNullOrWhiteSpace(dto.PlaceName))
            {
                var existingEntity = await _repo.PlaceExistsAsync(
               dto.PlaceName,
               placeId,
               cancellationToken);

                if (existingEntity is not null)
                {
                    if (!existingEntity.Deleted)
                    {
                        // ❌ Active duplicate

                        return ApiResponse<PlaceDTO>.Fail
                           (Messages.PlaceAlreadyExists,
                           StatusCodes.Status409Conflict,
                           ErrorCodes.PlaceAlreadyExists
                           );
                    }
                    return ApiResponse<PlaceDTO>.Fail(
                         Messages.PlaceAlreadyExistsRecoverable,
                         StatusCodes.Status422UnprocessableEntity,
                          ErrorCodes.PlaceAlreadyExistsRecoverable,
                          new PlaceDTO
                          {
                              PlaceId = existingEntity.PlaceId
                          }
                     );
                }
            }

            var entity = _mapper.Map<Place>((dto,placeId,loginId));
            var updated = await _repo.UpdateAsync(entity, cancellationToken);

            if (!updated)
                return ApiResponse<PlaceDTO>.Fail(
                    Messages.PlaceUpdated,
                    StatusCodes.Status204NoContent,
                    ErrorCodes.NoRowAffected);


            var updatedEntity = await _repo.GetByIdAsync
                (placeId, cancellationToken);

            return ApiResponse<PlaceDTO>.Ok
                (updatedEntity,
                Messages.PlaceUpdated,
                statusCode: StatusCodes.Status200OK);
        }



        public async Task<ApiResponse<PlaceDTO>>
            RecoverAsync(
            int id,
            int loginId,
            CancellationToken cancellationToken = default)
        {

            var existDto =
              await _repo.IsIdExistAsync(id,
              cancellationToken);

            if (existDto == null)
            {
                return ApiResponse<PlaceDTO>.Fail(
                    Messages.PlaceNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.PlaceNotFoundById);
            }

            if (!existDto.Deleted)
            {
                return ApiResponse<PlaceDTO>.Fail(
                    Messages.PlaceAlreadyRecovered,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.PlaceAlreadyRecovered);
            }

            var rowAffect = await _repo.RecoverAsync
                (id, loginId, cancellationToken);
            if (rowAffect)
            {

                return ApiResponse<PlaceDTO>.Fail(
                     Messages.PlaceAlreadyRecovered,
                     StatusCodes.Status409Conflict,
                     ErrorCodes.PlaceAlreadyRecovered);
            }

            var dto =
               await _repo.GetByIdAsync
               (id,
               cancellationToken);
            return ApiResponse<PlaceDTO>.
               Ok(dto, Messages.PlaceRecovered, statusCode: StatusCodes.Status200OK);
        }



        public async Task<ApiResponse<int>>
           DeleteAsync(
            int id,
            int loginId,
            CancellationToken cancellationToken = default)
        {


            var existDto =
            await _repo.IsIdExistAsync(id,
            cancellationToken);

            if (existDto == null)
            {
                return ApiResponse<int>.Fail(
                    Messages.PlaceNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.PlaceNotFoundById);
            }

            if (existDto.Deleted)
            {
                return ApiResponse<int>.Fail(
                    Messages.PlaceAlreadyDeleted,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.PlaceAlreadyDeleted);
            }

            var rowAffect = await _repo.DeleteAsync
                (id, loginId, cancellationToken);
            if (rowAffect)
            {

                return ApiResponse<int>.Fail(
                     Messages.PlaceAlreadyDeleted,
                     StatusCodes.Status409Conflict,
                     ErrorCodes.PlaceAlreadyDeleted);
            }


            return ApiResponse<int>.
               Ok(id, Messages.PlaceDeleted, statusCode: StatusCodes.Status200OK);
        }
    }
}

