using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Transactions;
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
        private readonly IUnitOfWork _uow;
        public PlaceService(
            IPlaceRepo repository,
            IMapper mapper,
            IPostOfficeRepo postOfficeRepo,
            IUnitOfWork uow)
        {
            _repo = repository;
            _mapper = mapper;
            _postOfficeRepo = postOfficeRepo;
            _uow = uow;
        }



        public async Task<ApiResponse<PlaceDetailDTO>> CreateAsync(
          PlaceCreateDTO dto,
          int loginId,
          CancellationToken cancellationToken)
        {

            // validate zip codes before anything else
            if (dto.ZipCodeLinkIds != null && dto.ZipCodeLinkIds.Any())
            {
                var exist = await _postOfficeRepo.
                    AreAllZipCodeLinksValidAsync(dto.ZipCodeLinkIds,
               cancellationToken);
                if (exist["idNotFound"] == true)
                {
                    return ApiResponse<PlaceDetailDTO>.Fail(
                      Messages.ZipCodesNotFound,
                      StatusCodes.Status404NotFound,
                      ErrorCodes.ZipCodesNotFound
                      );
                }
                if (exist["deletedOrInactive"] == true)
                {
                    return ApiResponse<PlaceDetailDTO>.Fail
                       (Messages.ZipCodesNotAvaliable,
                       StatusCodes.Status409Conflict,
                       ErrorCodes.ZipCodesNotAvaliable);
                }
            }

            var existingEntity =
                await _repo.PlaceExistsAsync(
                dto.PlaceName,
                null,
                cancellationToken);

            await _uow.BeginAsync();

            try
            {
                if (existingEntity != null)
                {
                    if (existingEntity.Deleted)
                    {
                        // Place exists but was soft-deleted — return recoverable
                        await _uow.RollbackAsync();
                        return ApiResponse<PlaceDetailDTO>.Fail(
                            Messages.PlaceAlreadyExistsRecoverable,
                            StatusCodes.Status422UnprocessableEntity,
                            ErrorCodes.PlaceAlreadyExistsRecoverable,
                            new PlaceDetailDTO
                            {
                                PlaceId = existingEntity.PlaceId
                            });
                    }

                    // place name already exists and is not deleted
                    // if inactive, reactivate it
                    if (!existingEntity.Active)
                    {
                        await _repo.ActivatePlaceAsync(
                            existingEntity.PlaceId,
                            loginId,
                            _uow.Connection,
                            _uow.Transaction,
                            cancellationToken);
                    }

                    // check if zip codes are already linked
                    if (dto.ZipCodeLinkIds != null && dto.ZipCodeLinkIds.Any())
                    {
                        var alreadyLinked =
                            await _repo.CheckAlreadyPlaceZipCodeLinkedAsync(
                                existingEntity.PlaceId,
                                dto.ZipCodeLinkIds,
                                _uow.Connection,
                                _uow.Transaction,
                                cancellationToken);

                        if (alreadyLinked)
                        {
                            await _uow.RollbackAsync();
                            return ApiResponse<PlaceDetailDTO>.Fail(
                                Messages.ZipCodesAlreadyLinkedWithPlace,
                                StatusCodes.Status409Conflict,
                                ErrorCodes.ZipCodesAlreadyLinkedWithPlace);
                        }

                        // zip codes are not linked yet – link them to existing place
                        await _repo.BulkInsertPlaceZipCodeLinksAsync(
                            existingEntity.PlaceId,
                            dto.ZipCodeLinkIds,
                            loginId,
                            _uow.Connection,
                            _uow.Transaction,
                            cancellationToken);
                    }
                    else
                    {
                        // no zip codes sent and name already exists
                        await _uow.RollbackAsync();
                        return ApiResponse<PlaceDetailDTO>.Fail(
                            Messages.PlaceAlreadyExists,
                            StatusCodes.Status409Conflict,
                            ErrorCodes.PlaceAlreadyExists);
                    }

                    var dtoOut = await _repo.GetByIdAsync(
                        existingEntity.PlaceId,
                        _uow.Connection,
                        _uow.Transaction,
                        cancellationToken);

                    await _uow.CommitAsync();
                    return ApiResponse<PlaceDetailDTO>.Ok(
                        dtoOut!,
                        Messages.PlaceWasAlreadyExistZipCodeLinked,
                        StatusCodes.Status201Created);
                }

                // place name is new – insert into Place table then link zip codes
                var entity = _mapper.Map<Place>(dto);
                entity.CreatedBy = loginId;

                entity.PlaceId =
                    await _repo.CreateAsync(
                        entity, _uow.Connection, _uow.Transaction, cancellationToken);

                await _repo.BulkInsertPlaceZipCodeLinksAsync(
                    entity.PlaceId,
                    dto.ZipCodeLinkIds,
                    loginId,
                    _uow.Connection,
                    _uow.Transaction,
                    cancellationToken);

                var result = await _repo.GetByIdAsync(
                    entity.PlaceId,
                    _uow.Connection,
                    _uow.Transaction,
                    cancellationToken);

                await _uow.CommitAsync();
                return ApiResponse<PlaceDetailDTO>.Ok(
                    result!,
                    Messages.PlaceCreated,
                    StatusCodes.Status201Created);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }


        public async Task<ApiResponse<PlaceDetailDTO>?> GetByIdAsync(
          int id,
          CancellationToken cancellationToken = default)
        {
            await _uow.BeginAsync();

            var dto = await _repo.GetByIdAsync
                (id, _uow.Connection, _uow.Transaction, cancellationToken);

            await _uow.CommitAsync();

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
        GetLookUpAsync(int zipCodeId, CancellationToken cancellationToken)
        {
            var result = await _repo.GetActiveAsync(zipCodeId, cancellationToken)
                         ?? new List<PlaceLookUpDTO>();

            return ApiResponse<List<PlaceLookUpDTO>>.Ok(
                result,
                result.Count == 0
                    ? Messages.NoActivePlaces
                    : Messages.PlacesRetrieved,
                statusCode: StatusCodes.Status200OK
            );
        }





        public async Task<ApiResponse<PlaceDetailDTO>> UpdateAsync(
          PlaceUpdateDTO dto,
          int placeId,
          int loginId,
          CancellationToken cancellationToken = default)
        {
            var postOfficeLink = false;

            var existDto =
                await _repo.IsIdExistAsync(placeId,
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
            }
            await _uow.BeginAsync();

            var entity = _mapper.Map<Place>((dto, placeId, loginId));
            try
            {


                var placeUpdation = await _repo.UpdateAsync(entity, _uow.Connection,
                _uow.Transaction, cancellationToken);




                if (dto.UpdateZipCodeLinkIds != null)
                {
                    var isPostOfficeLinked =
                     await _repo.CheckPlaceZipCodeLinksExistAsync(
                         placeId,
                         dto.UpdateZipCodeLinkIds
                             .Select(x => x.ZipCodeLinkId)
                             .ToList(),
                         true,
                         _uow.Connection,
                         _uow.Transaction,
                         cancellationToken);
                    if (!isPostOfficeLinked)
                    {
                        await _uow.RollbackAsync();
                        return ApiResponse<PlaceDetailDTO>.Fail(
                          Messages.ZipCodesNotFound,
                          StatusCodes.Status404NotFound,
                          ErrorCodes.ZipCodesNotFound
                          );
                    }

                    postOfficeLink = await _repo.BulkUpdatePlaceZipCodeLinksAsync(
                             placeId,
                             dto.UpdateZipCodeLinkIds,
                             loginId,
                             _uow.Connection,
                             _uow.Transaction,
                             cancellationToken);
                }



                if (dto.InsertZipCodeLinkIds != null)
                {
                    var linkedPostOfficeFound =
                    await _repo.CheckPlaceZipCodeLinksExistAsync(
                        placeId,
                        dto.InsertZipCodeLinkIds,
                        false,
                        _uow.Connection,
                        _uow.Transaction,
                        cancellationToken);
                    if (linkedPostOfficeFound)
                    {
                        await _uow.RollbackAsync();
                        return ApiResponse<PlaceDetailDTO>.Fail(
                          Messages.ZipCodesAreAlreadyLinked,
                          StatusCodes.Status409Conflict,
                          ErrorCodes.ZipCodesAreAlreadyLinked
                          );
                    }

                    var exist = await _postOfficeRepo.
                     AreAllZipCodeLinksValidAsync(dto.InsertZipCodeLinkIds,
                      cancellationToken);
                    if (exist["idNotFound"] == true)
                    {
                        return ApiResponse<PlaceDetailDTO>.Fail(
                       Messages.ZipCodesNotFound,
                       StatusCodes.Status404NotFound,
                       ErrorCodes.ZipCodesNotFound
                       );
                    }
                    if (exist["deletedOrInactive"] == true)
                    {
                        return ApiResponse<PlaceDetailDTO>.Fail
                           (Messages.ZipCodesNotAvaliable,
                           StatusCodes.Status409Conflict,
                           ErrorCodes.ZipCodesNotAvaliable);
                    }

                    postOfficeLink = await _repo.BulkInsertPlaceZipCodeLinksAsync(
                               placeId,
                               dto.InsertZipCodeLinkIds,
                               loginId,
                               _uow.Connection,
                               _uow.Transaction,
                               cancellationToken);
                }


                if (!postOfficeLink && !placeUpdation)
                {
                    return ApiResponse<PlaceDetailDTO>.Fail(
                    Messages.PlaceUpdated,
                    StatusCodes.Status204NoContent,
                    ErrorCodes.NoRowAffected);
                }

                var dtoOut = await _repo.GetByIdAsync(placeId,
                    _uow.Connection, _uow.Transaction, cancellationToken);

                await _uow.CommitAsync();

                return ApiResponse<PlaceDetailDTO>.Ok(
                    dtoOut!,
                    Messages.PlaceUpdated,
                    statusCode: StatusCodes.Status200OK);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }

           
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

            await _uow.BeginAsync();

            try
            {
                var rowAffect = await _repo.RecoverAsync(
                    id, loginId, _uow.Connection, _uow.Transaction, cancellationToken);

                if (!rowAffect)
                {
                    await _uow.RollbackAsync();
                    return ApiResponse<PlaceDetailDTO>.Fail(
                         Messages.PlaceAlreadyRecovered,
                         StatusCodes.Status409Conflict,
                         ErrorCodes.PlaceAlreadyRecovered);
                }

                var dtoOut = await _repo.GetByIdAsync(
                    id, _uow.Connection, _uow.Transaction, cancellationToken);

                await _uow.CommitAsync();
                return ApiResponse<PlaceDetailDTO>.Ok(
                    dtoOut!,
                    Messages.PlaceRecovered,
                    statusCode: StatusCodes.Status200OK);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
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

            await _uow.BeginAsync();

            try
            {
                var rowAffect = await _repo.DeleteAsync(
                    id, 
                    loginId,
                    _uow.Connection, 
                    _uow.Transaction, 
                    cancellationToken);

                if (!rowAffect)
                {
                    await _uow.RollbackAsync();
                    return ApiResponse<PlaceDetailDTO>.Fail(
                         Messages.PlaceAlreadyDeleted,
                         StatusCodes.Status409Conflict,
                         ErrorCodes.PlaceAlreadyDeleted);
                }

                var dtoOut = await _repo.GetByIdAsync(
                    id, 
                    _uow.Connection,
                    _uow.Transaction,
                    cancellationToken);

                await _uow.CommitAsync();
                return ApiResponse<PlaceDetailDTO>.Ok(
                    dtoOut!,
                    Messages.PlaceDeleted,
                    statusCode: StatusCodes.Status200OK);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }
    }
}

