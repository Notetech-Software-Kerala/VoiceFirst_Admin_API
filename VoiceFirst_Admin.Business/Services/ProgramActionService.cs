using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Basic;
using VoiceFirst_Admin.Utilities.Models.Entities;
using VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction;

namespace VoiceFirst_Admin.Business.Services
{
    public class ProgramActionService : IProgramActionService
    {
        private readonly IProgramActionRepo _repo;

        public ProgramActionService(IProgramActionRepo repo)
        {
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

        public Task<SysProgramActions?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => _repo.GetByIdAsync(id, cancellationToken);

        public Task<IEnumerable<SysProgramActions>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default)
            => _repo.GetAllAsync(filter, cancellationToken);

        public async Task<bool> UpdateAsync(ProgramActionUpdateDto dto,int loginId, CancellationToken cancellationToken = default)
        {
            var entity = new SysProgramActions
            {
                SysProgramActionId = dto.SysProgramActionId,
                ProgramActionName = dto.ProgramActionName ?? string.Empty,
                IsActive = dto.IsActive,
                UpdatedBy = loginId
            };

            return await _repo.UpdateAsync(entity, cancellationToken);
        }

        public Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
            => _repo.ExistsByNameAsync(name, excludeId, cancellationToken);

    }
}
