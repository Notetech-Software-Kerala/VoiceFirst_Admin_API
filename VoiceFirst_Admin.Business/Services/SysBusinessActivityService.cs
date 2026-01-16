using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    // Service layer for SysBusinessActivity orchestration
    public class SysBusinessActivityService : ISysBusinessActivityService
    {
        private readonly ISysBusinessActivityRepo _repo;

        public SysBusinessActivityService(ISysBusinessActivityRepo repository)
        {
            _repo = repository;
        }

        public async Task<SysBusinessActivity> CreateAsync(SysBusinessActivityCreateDTO dto, int loginId, CancellationToken cancellationToken = default)
        {
            var entity = new SysBusinessActivity
            {
                BusinessActivityName = dto.Name,
                CreatedBy = loginId,
            };

            return await _repo.CreateAsync(entity, cancellationToken);
        }

        public Task<SysBusinessActivity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => _repo.GetByIdAsync(id, cancellationToken);

        public Task<IEnumerable<SysBusinessActivity>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default)
            => _repo.GetAllAsync(filter, cancellationToken);

        public async Task<bool> UpdateAsync(SysBusinessActivityUpdateDTO dto,int sysBusinessActivityId, int loginId, CancellationToken cancellationToken = default)
        {
            var entity = new SysBusinessActivity
            {
                SysBusinessActivityId = sysBusinessActivityId,
                BusinessActivityName = dto.Name ?? string.Empty,
                IsActive = dto.Status,
                UpdatedBy = loginId
            };

            return await _repo.UpdateAsync(entity, cancellationToken);
        }

        public Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
            => _repo.ExistsByNameAsync(name, excludeId, cancellationToken);
        public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            return _repo.DeleteAsync(id, cancellationToken);
        }
    }
}
