using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Exceptions;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class SysBusinessActivityService : ISysBusinessActivityService
    {
        private readonly ISysBusinessActivityRepo _repo;
        private readonly IMapper _mapper;
        public SysBusinessActivityService(
            ISysBusinessActivityRepo repository,
            IMapper mapper)
        {
            _repo = repository;
            _mapper = mapper;
        }


        public async Task<ApiResponse<SysBusinessActivityDTO>> CreateAsync(
          SysBusinessActivityCreateDTO dto,
          int loginId,
          CancellationToken cancellationToken)
            {

            var existingEntity = await _repo.BusinessActivityExistsAsync(
                dto.ActivityName,
                null,
                cancellationToken);

            if (existingEntity != null)
            {
                if (!existingEntity.Deleted)
                {
                    // ❌ Active duplicate
                    
                    return ApiResponse<SysBusinessActivityDTO>.Fail
                       (Messages.BusinessActivityAlreadyExists,
                       StatusCodes.Status409Conflict,
                       ErrorCodes.BusinessActivityAlreadyExists
                       );
                }
                return ApiResponse<SysBusinessActivityDTO>.Fail(                    
                     Messages.BusinessActivityAlreadyExistsRecoverable,
                     StatusCodes.Status422UnprocessableEntity,
                      ErrorCodes.BusinessActivityAlreadyExistsRecoverable,
                      new SysBusinessActivityDTO
                      {
                          ActivityId = existingEntity.ActivityId
                      }
                 );
            }


            var entity = _mapper.Map<SysBusinessActivity>(dto);
            entity.CreatedBy = loginId;

            entity.SysBusinessActivityId =
                await _repo.CreateAsync(entity, cancellationToken);

            if(entity.SysBusinessActivityId <= 0)
            {
                return ApiResponse<SysBusinessActivityDTO>.Fail(
                    Messages.SomethingWentWrong,
                    StatusCodes.Status500InternalServerError,
                    ErrorCodes.InternalServerError);
            }

            var createdDto =
                await _repo.GetByIdAsync
                (entity.SysBusinessActivityId, 
                cancellationToken);

            return ApiResponse<SysBusinessActivityDTO>.Ok(
                createdDto,
                Messages.BusinessActivityCreated,
                StatusCodes.Status201Created);
        }


        public async Task<ApiResponse<SysBusinessActivityDTO>?> GetByIdAsync(
          int id,
          CancellationToken cancellationToken = default)
        {
            var dto = await _repo.GetByIdAsync(id, cancellationToken);

            if (dto == null)
            {
               
                return ApiResponse<SysBusinessActivityDTO>.Fail(                   
                   Messages.BusinessActivityNotFoundById,
                   StatusCodes.Status404NotFound,
                    ErrorCodes.BusinessActivityNotFoundById);
            }
            return ApiResponse <SysBusinessActivityDTO>.Ok(
                dto,
                Messages.BusinessActivityRetrieved,
                StatusCodes.Status200OK);
        }

     
        public async Task<ApiResponse<PagedResultDto<SysBusinessActivityDTO>>>
        GetAllAsync(BusinessActivityFilterDTO filter, 
            CancellationToken cancellationToken = default)
        {
            var result = await _repo.GetAllAsync(filter, cancellationToken);

            return ApiResponse<PagedResultDto<SysBusinessActivityDTO>>.Ok(
                result,              
                result.TotalCount == 0
                    ? Messages.BusinessActivitiesNotFound
                    : Messages.BusinessActivitiesRetrieved,
                 statusCode: StatusCodes.Status200OK
            );
        }

 

        public async Task<ApiResponse<List<SysBusinessActivityActiveDTO>>>
        GetActiveAsync(CancellationToken cancellationToken)
        {
            var result = await _repo.GetActiveAsync(cancellationToken)
                         ?? new List<SysBusinessActivityActiveDTO>();

            return ApiResponse<List<SysBusinessActivityActiveDTO>>.Ok(
                result,
                result.Count == 0
                    ? Messages.NoActiveBusinessActivities
                    : Messages.BusinessActivitiesRetrieved,
                statusCode: StatusCodes.Status200OK
            );
        }


        public async Task<ApiResponse<SysBusinessActivityDTO>> UpdateAsync(
          SysBusinessActivityUpdateDTO dto,
          int sysBusinessActivityId,
          int loginId,
          CancellationToken cancellationToken = default)
        {
            var existDto =
                await _repo.IsIdExistAsync(sysBusinessActivityId,
                cancellationToken);


            if (existDto == null )
            {

                return ApiResponse<SysBusinessActivityDTO>.Fail(
                   Messages.BusinessActivityNotFoundById,
                   StatusCodes.Status404NotFound,
                    ErrorCodes.BusinessActivityNotFoundById);              
            }
            else if (existDto.Deleted)
            {

               
                 return ApiResponse<SysBusinessActivityDTO>.Fail(
                  Messages.BusinessActivityNotFound,
                  StatusCodes.Status409Conflict,
                   ErrorCodes.BusinessActivityNotFound);
                
            }

            // uniqueness check ONLY if name is patched
            if (!string.IsNullOrWhiteSpace(dto.ActivityName))
            {
                var existingEntity = await _repo.BusinessActivityExistsAsync(
               dto.ActivityName,
               sysBusinessActivityId,
               cancellationToken);

                if (existingEntity is not null)
                {
                    if (!existingEntity.Deleted)
                    {
                        // ❌ Active duplicate

                        return ApiResponse<SysBusinessActivityDTO>.Fail
                           (Messages.BusinessActivityAlreadyExists,
                           StatusCodes.Status409Conflict,
                           ErrorCodes.BusinessActivityAlreadyExists
                           );
                    }
                    return ApiResponse<SysBusinessActivityDTO>.Fail(
                         Messages.BusinessActivityAlreadyExistsRecoverable,
                         StatusCodes.Status422UnprocessableEntity,
                          ErrorCodes.BusinessActivityAlreadyExistsRecoverable,
                          new SysBusinessActivityDTO
                          {
                              ActivityId = existingEntity.ActivityId
                          }
                     );
                }
            }

            var entity = _mapper.Map<SysBusinessActivity>((dto,sysBusinessActivityId,loginId));
            var updated = await _repo.UpdateAsync(entity, cancellationToken);

            if (!updated)
                return ApiResponse<SysBusinessActivityDTO>.Fail(
                    Messages.BusinessActivityUpdated,
                    StatusCodes.Status204NoContent,
                    ErrorCodes.NoRowAffected);
       

            var updatedEntity = await _repo.GetByIdAsync
                (sysBusinessActivityId, cancellationToken);

            return ApiResponse <SysBusinessActivityDTO>.Ok
                ( updatedEntity,
                Messages.BusinessActivityUpdated,
                statusCode: StatusCodes.Status200OK);
        }



        public async Task<ApiResponse<SysBusinessActivityDTO>>
            RecoverBusinessActivityAsync(
            int id,
            int loginId,
            CancellationToken cancellationToken = default)
        {

            var existDto =
              await _repo.IsIdExistAsync(id,
              cancellationToken);

            if (existDto == null)
            {
                return ApiResponse<SysBusinessActivityDTO>.Fail(
                    Messages.BusinessActivityNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.BusinessActivityNotFoundById);
            }

            if (!existDto.Deleted)
            {
                return ApiResponse<SysBusinessActivityDTO>.Fail(
                    Messages.BusinessActivityAlreadyRecovered,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.BusinessActivityAlreadyRecovered);
            }

            var rowAffect = await _repo.RecoverBusinessActivityAsync
                (id, loginId, cancellationToken);
            if (!rowAffect)
            {

                return ApiResponse<SysBusinessActivityDTO>.Fail(
                     Messages.BusinessActivityAlreadyRecovered,
                     StatusCodes.Status409Conflict,
                     ErrorCodes.BusinessActivityAlreadyRecovered);
            }
                     
            var dto =
               await _repo.GetByIdAsync
               (id,
               cancellationToken);
            return ApiResponse<SysBusinessActivityDTO>.
               Ok(dto, Messages.BusinessActivityRecovered, statusCode: StatusCodes.Status200OK);
        }

            
        
         public async Task<ApiResponse<SysBusinessActivityDTO>> 
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
                return ApiResponse<SysBusinessActivityDTO>.Fail(
                    Messages.BusinessActivityNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.BusinessActivityNotFoundById);
            }

            if (existDto.Deleted)
            {
                return ApiResponse<SysBusinessActivityDTO>.Fail(
                    Messages.BusinessActivityAlreadyDeleted,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.BusinessActivityAlreadyDeleted);
            }

            var rowAffect = await _repo.DeleteAsync
                (id, loginId, cancellationToken);
            if (!rowAffect)
            {

                return ApiResponse<SysBusinessActivityDTO>.Fail(
                     Messages.BusinessActivityAlreadyDeleted,
                     StatusCodes.Status409Conflict,
                     ErrorCodes.BusinessActivityAlreadyDeleted);
            }

            var dto =
               await _repo.GetByIdAsync
               (id,
               cancellationToken);
            return ApiResponse<SysBusinessActivityDTO>.
               Ok(dto, Messages.BusinessActivityDeleted,statusCode:StatusCodes.Status200OK);
        }
    }
}
