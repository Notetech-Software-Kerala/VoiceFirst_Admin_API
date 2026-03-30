using AutoMapper;
using Microsoft.AspNetCore.Http;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.BusinessActivityUserCustomFieldLink;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Enums;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class SysBusinessActivityService : ISysBusinessActivityService
    {
        private readonly ISysBusinessActivityRepo _repo;
        private readonly ISysUserCustomFieldRepo _sysUserCustomFieldRepo;
        private readonly IMapper _mapper;
        public SysBusinessActivityService(
            ISysBusinessActivityRepo repository, ISysUserCustomFieldRepo sysUserCustomFieldRepo,
            IMapper mapper)
        {
            _repo = repository;
            _sysUserCustomFieldRepo = sysUserCustomFieldRepo;
            _mapper = mapper;
        }


        public async Task<ApiResponse<SysBusinessActivityDetailsDTO>> CreateAsync(
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
                    
                    return ApiResponse<SysBusinessActivityDetailsDTO>.Fail
                       (Messages.BusinessActivityAlreadyExists,
                       StatusCodes.Status409Conflict,
                       ErrorCodes.BusinessActivityAlreadyExists
                       );
                }
                return ApiResponse<SysBusinessActivityDetailsDTO>.Fail(                    
                     Messages.BusinessActivityAlreadyExistsRecoverable,
                     StatusCodes.Status422UnprocessableEntity,
                      ErrorCodes.BusinessActivityAlreadyExistsRecoverable,
                      new SysBusinessActivityDetailsDTO
                      {
                          ActivityId = existingEntity.ActivityId
                      }
                 );
            }
            if(dto.addCustomFieldLinkIds != null && dto.addCustomFieldLinkIds.Count() > 0)
            {
                foreach (var item in dto.addCustomFieldLinkIds)
                {
                    var customfield = await _sysUserCustomFieldRepo.GetByLinkIdAsync(item, cancellationToken);
                    if (customfield == null)
                    {
                        return ApiResponse<SysBusinessActivityDetailsDTO>.Fail(
                            Messages.CustomFieldNotFound,
                            StatusCodes.Status409Conflict);
                    }
                    if (customfield.IsDeleted != false)
                    {
                        return ApiResponse<SysBusinessActivityDetailsDTO>.Fail(
                            Messages.CustomFieldAlreadyDeleted,
                            StatusCodes.Status409Conflict);
                    }
                    if (customfield.IsActive != true)
                    {
                        return ApiResponse<SysBusinessActivityDetailsDTO>.Fail(
                            Messages.CustomFieldNotActive,
                            StatusCodes.Status409Conflict);
                    }
                }
            }

            var entity = _mapper.Map<SysBusinessActivity>(dto);
            entity.CreatedBy = loginId;

            entity.SysBusinessActivityId =
                await _repo.CreateAsync(entity, dto.addCustomFieldLinkIds, cancellationToken);

            if(entity.SysBusinessActivityId <= 0)
            {
                return ApiResponse<SysBusinessActivityDetailsDTO>.Fail(
                    Messages.SomethingWentWrong,
                    StatusCodes.Status500InternalServerError,
                    ErrorCodes.InternalServerError);
            }

            var createdResponse = await GetByIdAsync(entity.SysBusinessActivityId, cancellationToken);

            if (createdResponse.Data == null)
            {
                return ApiResponse<SysBusinessActivityDetailsDTO>.Fail(
                    Messages.SomethingWentWrong,
                    StatusCodes.Status500InternalServerError,
                    ErrorCodes.InternalServerError);
            }

            return ApiResponse<SysBusinessActivityDetailsDTO>.Ok(
                createdResponse.Data,
                Messages.BusinessActivityCreated,
                StatusCodes.Status201Created);
        }


        public async Task<ApiResponse<SysBusinessActivityDetailsDTO>> GetByIdAsync(
          int id,
          CancellationToken cancellationToken = default)
        {
            var dto = await _repo.GetByIdAsync(id, cancellationToken);
            var dtoDetails = _mapper.Map<SysBusinessActivityDetailsDTO>(dto);
            var customFields = await _repo.GetCustomFieldByIdAsync(id, cancellationToken);

            

            if (customFields != null)
            {
                var customFieldDtos = _mapper.Map<IEnumerable<BusinessActivityUserCustomFieldDto>>(customFields);
                dtoDetails.activityCustomFieldLinks = customFieldDtos.ToList();
            }
            else
            {
                dtoDetails.activityCustomFieldLinks = new List<BusinessActivityUserCustomFieldDto>();
            }
            if (customFields == null)
            {
               
                return ApiResponse<SysBusinessActivityDetailsDTO>.Fail(                   
                   Messages.BusinessActivityNotFoundById,
                   StatusCodes.Status404NotFound,
                    ErrorCodes.BusinessActivityNotFoundById);
            }
            return ApiResponse <SysBusinessActivityDetailsDTO>.Ok(
                dtoDetails,
                Messages.BusinessActivityRetrieved,
                StatusCodes.Status200OK);
        }

     
        public async Task<ApiResponse<PagedResultDto<SysBusinessActivityDTO>>>
        GetAllAsync(BusinessActivityFilterDTO filter, 
            CancellationToken cancellationToken = default)
        {
            filter.PageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            filter.Limit = filter.Limit <= 0 ? 10 : filter.Limit;
            filter.Limit = Math.Min(filter.Limit, 30);
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


        public async Task<ApiResponse<SysBusinessActivityDetailsDTO>> UpdateAsync(
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

                return ApiResponse<SysBusinessActivityDetailsDTO>.Fail(
                   Messages.BusinessActivityNotFoundById,
                   StatusCodes.Status404NotFound,
                    ErrorCodes.BusinessActivityNotFoundById);              
            }
            else if (existDto.Deleted)
            {

               
                 return ApiResponse<SysBusinessActivityDetailsDTO>.Fail(
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

                        return ApiResponse<SysBusinessActivityDetailsDTO>.Fail
                           (Messages.BusinessActivityAlreadyExists,
                           StatusCodes.Status409Conflict,
                           ErrorCodes.BusinessActivityAlreadyExists
                           );
                    }
                    return ApiResponse<SysBusinessActivityDetailsDTO>.Fail(
                         Messages.BusinessActivityAlreadyExistsRecoverable,
                         StatusCodes.Status422UnprocessableEntity,
                          ErrorCodes.BusinessActivityAlreadyExistsRecoverable,
                          new SysBusinessActivityDetailsDTO
                          {
                              ActivityId = existingEntity.ActivityId
                          }
                     );
                }
            }
            if (dto.addCustomFieldIds != null && dto.addCustomFieldIds.Count() > 0)
            {
                foreach (var item in dto.addCustomFieldIds)
                {
                    var customfield = await _sysUserCustomFieldRepo.GetByIdAsync(item, cancellationToken);
                    if (customfield == null)
                    {
                        return ApiResponse<SysBusinessActivityDetailsDTO>.Fail(
                            Messages.CustomFieldNotFound,
                            StatusCodes.Status404NotFound);
                    }
                    if (customfield.IsDeleted != false)
                    {
                        return ApiResponse<SysBusinessActivityDetailsDTO>.Fail(
                            Messages.CustomFieldAlreadyDeleted,
                            StatusCodes.Status409Conflict);
                    }
                    if (customfield.IsActive != true)
                    {
                        return ApiResponse<SysBusinessActivityDetailsDTO>.Fail(
                            Messages.CustomFieldNotActive,
                            StatusCodes.Status409Conflict);
                    }
                    var alreadyExsit = await _repo.IsCustomFieldExistByActivityAsync(sysBusinessActivityId,item, cancellationToken);
                    if (alreadyExsit != null)
                    {
                        return ApiResponse<SysBusinessActivityDetailsDTO>.Fail(
                            Messages.CustomFieldAlreadyExistsByActivity,
                            StatusCodes.Status404NotFound);
                    }
                }
            }
            if (dto.updateCustomField != null && dto.updateCustomField.Count() > 0)
            {
                foreach (var item in dto.updateCustomField)
                {
                    var customfield = await _repo.GetCustomFieldLinkByIdAsync(item.ActivityCustomFieldLinkId, sysBusinessActivityId, cancellationToken);
                    if (customfield == null)
                    {
                        return ApiResponse<SysBusinessActivityDetailsDTO>.Fail(
                            Messages.CustomFieldNotFound,
                            StatusCodes.Status404NotFound);
                    }
                    
                    if (customfield.IsActive == item.Active)
                    {
                        return ApiResponse<SysBusinessActivityDetailsDTO>.Fail(
                            Messages.CustomFieldAlreadyRequestedStatus,
                            StatusCodes.Status409Conflict);
                    }
                }
            }

            var entity = _mapper.Map<SysBusinessActivity>((dto,sysBusinessActivityId,loginId));
            var updateCustomField = _mapper.Map<List<SysBusinessActivityUserCustomFieldLink>>(dto.updateCustomField);
            var updated = await _repo.UpdateAsync(entity, dto.addCustomFieldIds, updateCustomField, cancellationToken);

            if (!updated)
                return ApiResponse<SysBusinessActivityDetailsDTO>.Fail(
                    Messages.BusinessActivityUpdated,
                    StatusCodes.Status204NoContent,
                    ErrorCodes.NoRowAffected);


            var createdResponse = await GetByIdAsync(entity.SysBusinessActivityId, cancellationToken);

            if (createdResponse.Data == null)
            {
                return ApiResponse<SysBusinessActivityDetailsDTO>.Fail(
                    Messages.SomethingWentWrong,
                    StatusCodes.Status500InternalServerError,
                    ErrorCodes.InternalServerError);
            }

            return ApiResponse <SysBusinessActivityDetailsDTO>.Ok
                (createdResponse.Data,
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


        //public async Task<ApiResponse<SysBusinessActivityDTO>>
        //  DeleteAsync(
        //   int id,
        //   int loginId,
        //   CancellationToken cancellationToken = default)
        //{

        //    var deletedResult =
        //        await _repo.DeleteAsync(id, loginId, cancellationToken);

        //    switch (deletedResult)
        //    {
        //        case DeleteResult.NotFound:
        //            return ApiResponse<SysBusinessActivityDTO>.Fail(
        //                Messages.BusinessActivityNotFoundById,
        //                StatusCodes.Status404NotFound,
        //                ErrorCodes.BusinessActivityNotFoundById);

        //        case DeleteResult.AlreadyDeleted:
        //            return ApiResponse<SysBusinessActivityDTO>.Fail(
        //                Messages.BusinessActivityAlreadyDeleted,
        //                StatusCodes.Status409Conflict,
        //                ErrorCodes.BusinessActivityAlreadyDeleted);
        //    }

        //    var dto =
        //       await _repo.GetByIdAsync
        //       (id,
        //       cancellationToken);

        //    return ApiResponse<SysBusinessActivityDTO>.
        //       Ok(dto,
        //       Messages.BusinessActivityDeleted,
        //       statusCode: StatusCodes.Status200OK);
        //}

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
