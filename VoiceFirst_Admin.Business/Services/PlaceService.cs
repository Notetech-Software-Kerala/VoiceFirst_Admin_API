using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Place;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class PlaceService: IPlaceService
    {
        private readonly IPlaceRepo _repo;
        private readonly IPostOfficeRepo _postOfficeRepo;
        private readonly IMapper _mapper;
        private readonly IDapperContext _context;
        public PlaceService(
            IPlaceRepo repository,
            IMapper mapper,
            IPostOfficeRepo postOfficeRepo,
            IDapperContext dapperContext)
        {
            _repo = repository;
            _mapper = mapper;
            _postOfficeRepo = postOfficeRepo;
            _context = dapperContext;
        }



        public async Task<ApiResponse<PlaceDetailDTO>> CreateAsync(
          PlaceCreateDTO dto,
          int loginId,
          CancellationToken cancellationToken)
        {

            var existingEntity = await _repo.PlaceExistsAsync(
                dto.PlaceName,
                null,
                cancellationToken);

            if (existingEntity != null)
            {
                if (!existingEntity.Deleted)
                {
                    // ❌ Active duplicate

                    return ApiResponse<PlaceDetailDTO>.Fail
                       (Messages.PlaceAlreadyExists,
                       StatusCodes.Status409Conflict,
                       ErrorCodes.PlaceAlreadyExists
                       );
                }
                return ApiResponse<PlaceDetailDTO>.Fail(
                     Messages.PlaceAlreadyExistsRecoverable,
                     StatusCodes.Status422UnprocessableEntity,
                      ErrorCodes.PlaceAlreadyExistsRecoverable,
                      new PlaceDetailDTO
                      {
                          PlaceId = existingEntity.PlaceId
                      }
                 );
            }

            if (dto.PostOfficeIds != null && dto.PostOfficeIds.Any())
            {
                var exist = await _postOfficeRepo.
                    IsBulkIdsExistAsync(dto.PostOfficeIds,
               cancellationToken);
                if (exist["idNotFound"] == true)
                {
                    return ApiResponse<PlaceDetailDTO>.Fail(
                      Messages.PostOfficesNotFound,
                      StatusCodes.Status404NotFound,
                      ErrorCodes.PostOfficeNotFound
                      );
                }
                if (exist["deletedOrInactive"] == true)
                {
                    return ApiResponse<PlaceDetailDTO>.Fail
                       (Messages.PostOfficeNotFound,
                       StatusCodes.Status409Conflict,
                       ErrorCodes.PostOfficeNotFound);
                }

            }


            var entity = _mapper.Map<Place>(dto);
            entity.CreatedBy = loginId;

            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {

                entity.PlaceId =
                await _repo.CreateAsync
                (entity, connection,transaction, cancellationToken);




                await _repo.BulkInsertPlacePostOfficeLinksAsync
                    (
                    entity.PlaceId,
                    dto.PostOfficeIds,
                    loginId,
                    connection,
                    transaction,
                    cancellationToken);

                var dtoOut = await _repo.GetByIdAsync
                    (entity.PlaceId,
                     connection,
                    transaction,
                    cancellationToken);

               

                transaction.Commit();
                return ApiResponse<PlaceDetailDTO>.Ok(
                    dtoOut!,
                    Messages.PlaceCreated,
                   StatusCodes.Status201Created);

            }
            catch
            {
               transaction.Rollback();
                throw;
            }
        }


        public async Task<ApiResponse<PlaceDetailDTO>?> GetByIdAsync(
          int id,
          CancellationToken cancellationToken = default)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            var dto = await _repo.GetByIdAsync
                (id, connection,transaction, cancellationToken);

            if (dto == null)
            {

                return ApiResponse<PlaceDetailDTO>.Fail(
                   Messages.PlaceNotFoundById,
                   StatusCodes.Status404NotFound,
                    ErrorCodes.PlaceNotFoundById);
            }
            return ApiResponse<PlaceDetailDTO>.Ok(
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


            var updatedEntity = await GetByIdAsync
                (placeId, cancellationToken);

            return ApiResponse<PlaceDTO>.Ok
                (updatedEntity.Data,
                Messages.PlaceUpdated,
                statusCode: StatusCodes.Status200OK);
        }



        public async Task<ApiResponse<PlaceDetailDTO>>
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
                return ApiResponse<PlaceDetailDTO>.Fail(
                    Messages.PlaceNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.PlaceNotFoundById);
            }

            if (!existDto.Deleted)
            {
                return ApiResponse<PlaceDetailDTO>.Fail(
                    Messages.PlaceAlreadyRecovered,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.PlaceAlreadyRecovered);
            }

            var rowAffect = await _repo.RecoverAsync
                (id, loginId, cancellationToken);
            if (!rowAffect)
            {

                return ApiResponse<PlaceDetailDTO>.Fail(
                     Messages.PlaceAlreadyRecovered,
                     StatusCodes.Status409Conflict,
                     ErrorCodes.PlaceAlreadyRecovered);
            }

            var dto =
               await GetByIdAsync
               (id,
               cancellationToken);
            return ApiResponse<PlaceDetailDTO>.
               Ok(dto.Data, Messages.PlaceRecovered, statusCode: StatusCodes.Status200OK);
        }



        public async Task<ApiResponse<PlaceDetailDTO>>
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
                return ApiResponse<PlaceDetailDTO>.Fail(
                    Messages.PlaceNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.PlaceNotFoundById);
            }

            if (existDto.Deleted)
            {
                return ApiResponse<PlaceDetailDTO>.Fail(
                    Messages.PlaceAlreadyDeleted,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.PlaceAlreadyDeleted);
            }

            var rowAffect = await _repo.DeleteAsync
                (id, loginId, cancellationToken);
            if (!rowAffect)
            {

                return ApiResponse<PlaceDetailDTO>.Fail(
                     Messages.PlaceAlreadyDeleted,
                     StatusCodes.Status409Conflict,
                     ErrorCodes.PlaceAlreadyDeleted);
            }

            var dto =
             await GetByIdAsync
             (id,
             cancellationToken);
            return ApiResponse<PlaceDetailDTO>.
               Ok(dto.Data, Messages.PlaceDeleted, statusCode: StatusCodes.Status200OK);

          
        }
    }
}

