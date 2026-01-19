using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Exceptions;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class SysBusinessActivityService : ISysBusinessActivityService
    {
        private readonly ISysBusinessActivityRepo _repo;

        public SysBusinessActivityService(ISysBusinessActivityRepo repository)
        {
            _repo = repository;
        }

        public async Task<SysBusinessActivityDetailsDTO> CreateAsync(
            SysBusinessActivityCreateDTO dto,
            int loginId,
            CancellationToken cancellationToken = default)
        {
            if (await _repo.BusinessActivityExistsAsync
                (dto.Name, null, cancellationToken))
            {
                throw new BusinessConflictException(
                    Messages.SysBusinessActivityAlreadyExists,
                    ErrorCodes.BusinessActivityAlreadyExists);
            }

            var entity = new SysBusinessActivity
            {
                BusinessActivityName = dto.Name,
                CreatedBy = loginId
            };
            int createdId
                = await _repo.CreateAsync(entity, cancellationToken);

            return new SysBusinessActivityDetailsDTO
            {
                Id = createdId,
                Name = entity.BusinessActivityName,
                Status = true
            };
        }

        public async Task<SysBusinessActivityDetailsDTO?> GetByIdAsync(
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

            return new SysBusinessActivityDetailsDTO
            {
                Id = entity.SysBusinessActivityId,
                Name = entity.BusinessActivityName,
                Status = entity.IsActive
            };
        }


        public async Task<SysBusinessActivityDetailsDTO> UpdateAsync(
            SysBusinessActivityUpdateDTO dto,
            int sysBusinessActivityId,
            int loginId,
            CancellationToken cancellationToken = default)
        {
            if (await _repo.BusinessActivityExistsAsync(dto.Name ?? string.Empty, sysBusinessActivityId, cancellationToken))
            {
                throw new BusinessConflictException(
                    Messages.SysBusinessActivityAlreadyExists,
                    ErrorCodes.BusinessActivityAlreadyExists);
            }

            var entity = new SysBusinessActivity
            {
                SysBusinessActivityId = sysBusinessActivityId,
                BusinessActivityName = dto.Name ?? string.Empty,
                IsActive = dto.Status,
                UpdatedBy = loginId
            };

            var updated = await _repo.UpdateAsync(entity, cancellationToken);

            if (!updated)
            {
                throw new BusinessNotFoundException(
                    Messages.NotFound,
                    ErrorCodes.BusinessActivityNotFound);
            }
            var updatedEntity = await _repo.GetByIdAsync(sysBusinessActivityId, cancellationToken);
            return new SysBusinessActivityDetailsDTO
            {
                Id = updatedEntity.SysBusinessActivityId,
                Name = updatedEntity.BusinessActivityName,
                Status = updatedEntity.IsActive
            };
        }

        public Task<PagedResultDto<SysBusinessActivity>> GetAllAsync(
        CommonFilterDto1 filter,
        CancellationToken cancellationToken)
        {
             return _repo.GetAllAsync(filter, cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var deleted = await _repo.DeleteAsync(id, cancellationToken);

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
