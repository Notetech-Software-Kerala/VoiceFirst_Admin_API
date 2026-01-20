using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class ProgramActionService : IProgramActionService
    {
        private readonly IMapper _mapper;
        private readonly IProgramActionRepo _repo;

        public ProgramActionService(IMapper mapper, IProgramActionRepo repo)
        {
            _mapper = mapper;
            _repo = repo;
        }

        public async Task<SysProgramActions> CreateAsync(ProgramActionCreateDto dto, int loginId, CancellationToken cancellationToken = default)
        {
            var entity = new SysProgramActions
            {
                ProgramActionName = dto.ProgramActionName,
                CreatedBy = loginId,
            };

            return await _repo.CreateAsync(entity, cancellationToken);
        }

        public async Task<ProgramActionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repo.GetByIdAsync(id, cancellationToken);
            if (entity == null) return null;
            var dto = _mapper.Map<ProgramActionDto>(entity);
            return dto;
        }

        public async Task<PagedResultDto<ProgramActionDto>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default)
        {
            var entities = await _repo.GetAllAsync(filter, cancellationToken);
            var list = _mapper.Map<IEnumerable<ProgramActionDto>>(entities.Items);
            return new PagedResultDto<ProgramActionDto>
            {
                Items = list,
                TotalCount = entities.TotalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.Limit
            };
        }
        public async Task<IEnumerable<ProgramActionLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _repo.GetLookupAsync( cancellationToken);
            if (entities == null) return null;
            var dto = _mapper.Map<IEnumerable<ProgramActionLookupDto>>(entities);
            return dto;

        }
        public async Task<bool> UpdateAsync(ProgramActionUpdateDto dto,int id,int loginId, CancellationToken cancellationToken = default)
        {
            var entity = new SysProgramActions
            {
                SysProgramActionId = id,
                ProgramActionName = dto.ActionName ?? string.Empty,
                IsActive = dto.Active,
                UpdatedBy = loginId
            };

            return await _repo.UpdateAsync(entity, cancellationToken);
        }
        public async Task<bool> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {   
            
            var entity = new SysProgramActions
            {
                SysProgramActionId = id,
                DeletedBy = loginId,
            };

            return await _repo.DeleteAsync(entity, cancellationToken);
        }
        public async Task<bool> RestoreAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {

            var entity = new SysProgramActions
            {
                SysProgramActionId = id,
                UpdatedBy = loginId,
            };

            return await _repo.RestoreAsync(entity, cancellationToken);
        }

        public Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
            => _repo.ExistsByNameAsync(name, excludeId, cancellationToken);

    }
}
