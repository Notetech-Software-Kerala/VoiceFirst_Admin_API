using AutoMapper;
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
        private readonly IMapper _mapper;
        public SysBusinessActivityService(
            ISysBusinessActivityRepo repository,
            IMapper mapper)
        {
            _repo = repository;
            _mapper = mapper;
        }

        public async Task<SysBusinessActivityDTO> CreateAsync(
           SysBusinessActivityCreateDTO dto,
           int loginId,
           CancellationToken cancellationToken)
        {
            if (await _repo.BusinessActivityExistsAsync(dto.Name, null, cancellationToken))
                throw new BusinessConflictException(
                    Messages.SysBusinessActivityAlreadyExists,
                    ErrorCodes.BusinessActivityAlreadyExists);

            var entity = _mapper.Map<SysBusinessActivity>(dto);
            entity.CreatedBy = loginId;

            entity.SysBusinessActivityId =
                await _repo.CreateAsync(entity, cancellationToken);

            return _mapper.Map<SysBusinessActivityDTO>(entity);
        }

        public async Task<int> RecoverBusinessActivityAsync(
            int id,
            int loginId,
            CancellationToken cancellationToken = default)
        {
            var recoveredId = await _repo.RecoverBusinessActivityAsync(id, loginId, cancellationToken);
            if (recoveredId == 0)
            {
                throw new BusinessNotFoundException(
                    Messages.NotFound,
                    ErrorCodes.BusinessActivityNotFound);
            }
            return recoveredId;
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
            if (await _repo.BusinessActivityExistsAsync(dto.Name ?? string.Empty, sysBusinessActivityId, cancellationToken))
            {
                throw new BusinessConflictException(
                    Messages.SysBusinessActivityAlreadyExists,
                    ErrorCodes.BusinessActivityAlreadyExists);
            }


            var entity = _mapper.Map<SysBusinessActivity>((dto, sysBusinessActivityId));
            entity.UpdatedBy = loginId;
            var updated = await _repo.UpdateAsync(entity, cancellationToken);

            if (!updated)
            {
                throw new BusinessNotFoundException(
                    Messages.NotFound,
                    ErrorCodes.BusinessActivityNotFound);
            }
            var updatedEntity = await _repo.GetByIdAsync(sysBusinessActivityId, cancellationToken);

            return _mapper.Map<SysBusinessActivityDTO>(updatedEntity);

        }


        public async Task<PagedResultDto<SysBusinessActivityDTO>> GetAllAsync(
     CommonFilterDto1 filter,
     CancellationToken cancellationToken)
        {
            var pagedEntities = await _repo.GetAllAsync(filter, cancellationToken);

            return new PagedResultDto<SysBusinessActivityDTO>
            {
                Items = _mapper.Map<IEnumerable<SysBusinessActivityDTO>>(pagedEntities.Items),
                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
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
