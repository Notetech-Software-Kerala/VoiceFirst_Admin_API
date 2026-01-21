using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

            if (existingEntity is not null)
            {
                if (existingEntity.IsDeleted== false)
                {
                    // ❌ Active duplicate
                    
                    return ApiResponse<SysBusinessActivityDTO>.Fail
                       (Messages.BusinessActivityAlreadyExists,
                       StatusCodes.Status409Conflict,
                       ErrorCodes.BusinessActivityAlreadyExists
                       );
                }
                return ApiResponse<SysBusinessActivityDTO>.Fail
                    (Messages.BusinessActivityAlreadyExistsRecoverable,
                    StatusCodes.Status422UnprocessableEntity,
                    ErrorCodes.BusinessActivityAlreadyExistsRecoverable
                    );
            
            }

            var entity = _mapper.Map<SysBusinessActivity>(dto);
            entity.CreatedBy = loginId;

            entity.SysBusinessActivityId =
                await _repo.CreateAsync(entity, cancellationToken);

            var createdEntity =
                await _repo.GetByIdAsync(entity.SysBusinessActivityId, cancellationToken);

             var createdDto = _mapper.Map<SysBusinessActivityDTO>(createdEntity);

            return ApiResponse<SysBusinessActivityDTO>.Ok(createdDto, Messages.BusinessActivityCreated,StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<SysBusinessActivityDTO>> RecoverBusinessActivityAsync(
            int id,
            int loginId,
            CancellationToken cancellationToken = default)
        {
            var rowAffect = await _repo.RecoverBusinessActivityAsync(id, loginId, cancellationToken);
            if (rowAffect <= 0)
            {
                throw new BusinessNotFoundException(
                    Messages.NotFound,
                    ErrorCodes.BusinessActivityNotFound);
            }
            var dto = await GetByIdAsync(id, cancellationToken);
            return ApiResponse<SysBusinessActivityDTO>.
                Ok(dto, Messages.ProgramRecovered);

        }

        public async Task<SysBusinessActivityDTO?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _repo.GetByIdAsync(id, cancellationToken);

            if (entity == null)
            {
                throw new BusinessNotFoundException(
                    Messages.NotFound,
                    ErrorCodes.BusinessActivityNotFound);
            }
            return _mapper.Map<SysBusinessActivityDTO>(entity);
        }


        

                public async Task<SysBusinessActivityDTO> UpdateAsync(
            SysBusinessActivityUpdateDTO dto,
            int sysBusinessActivityId,
            int loginId,
            CancellationToken cancellationToken = default)
        {
            // uniqueness check ONLY if name is patched
            if (!string.IsNullOrWhiteSpace(dto.ActivityName))
            {
                var existingEntity = await _repo.BusinessActivityExistsAsync(
               dto.ActivityName,
               null,
               cancellationToken);

                if (existingEntity is not null)
                {
                    if (existingEntity.IsDeleted == false)
                    {
                        // ❌ Active duplicate
                        throw new BusinessConflictException(
                            Messages.BusinessActivityAlreadyExists,
                            ErrorCodes.BusinessActivityAlreadyExists);
                    }


                    // ♻ RECOVERABLE (Soft Deleted)
                    throw new BusinessRecoverableException(
                        Messages.BusinessActivityAlreadyExistsRecoverable,
                        ErrorCodes.BusinessActivityAlreadyExistsRecoverable);
                }
            }



            var entity = new SysBusinessActivity
            {
                SysBusinessActivityId = sysBusinessActivityId,
                BusinessActivityName = dto.ActivityName,
                IsActive = dto.Active,
                UpdatedBy = loginId
            };

            var updated = await _repo.UpdateAsync(entity, cancellationToken);

            if (!updated)
                throw new BusinessNotFoundException(
                    Messages.BusinessActivityNotFound,
                    ErrorCodes.BusinessActivityNotFound);

            var updatedEntity = await _repo.GetByIdAsync(sysBusinessActivityId, cancellationToken);

            return _mapper.Map<SysBusinessActivityDTO>(updatedEntity);
        }


     //   public async Task<PagedResultDto<SysBusinessActivityDTO>> GetAllAsync(
     //BusinessActivityFilterDTO filter,
     //CancellationToken cancellationToken)
     //   {
     //       var pagedEntities = await _repo.GetAllAsync(filter, cancellationToken);

     //       return new PagedResultDto<SysBusinessActivityDTO>
     //       {
     //           Items = _mapper.Map<IEnumerable<SysBusinessActivityDTO>>(pagedEntities.Items),
     //           TotalCount = pagedEntities.TotalCount,
     //           PageNumber = pagedEntities.PageNumber,
     //           PageSize = pagedEntities.PageSize
     //       };
     //   }

        public async Task<PagedResultDto<SysBusinessActivityDTO>> GetAllAsync(BusinessActivityFilterDTO filter, CancellationToken cancellationToken = default)
        {
            var entities = await _repo.GetAllAsync(filter, cancellationToken);
            var list = _mapper.Map<IEnumerable<SysBusinessActivityDTO>>(entities.Items);
            return new PagedResultDto<SysBusinessActivityDTO>
            {
                Items = list,
                TotalCount = entities.TotalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.Limit
            };
        }

        public async Task<List<SysBusinessActivityActiveDTO>> GetActiveAsync(
    CancellationToken cancellationToken)
        {
            var entities = await _repo.GetActiveAsync(cancellationToken);

            return _mapper.Map<IEnumerable<SysBusinessActivityActiveDTO>>(entities).ToList();
        }


        public async Task<bool> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {
            var deleted = await _repo.DeleteAsync(id,loginId,  cancellationToken);

            if (!deleted)
            {
                throw new BusinessNotFoundException(
                    Messages.NotFound,
                    ErrorCodes.BusinessActivityNotFound);
            }
            return true;
        }
    }
}
