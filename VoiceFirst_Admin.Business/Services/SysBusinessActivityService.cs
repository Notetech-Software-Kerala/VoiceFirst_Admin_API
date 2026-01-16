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
        private readonly ISysBusinessActivityRepo _repository;

        public SysBusinessActivityService(ISysBusinessActivityRepo repository)
        {
            _repository = repository;
        }

        public async Task<int> CreateAsync
            (SysBusinessActivityCreateDTO dto,int userId,
            CancellationToken cancellationToken = default)
        {
            var entity = new SysBusinessActivity
            {
                BusinessActivityName = dto.Name,
                CreatedBy = userId,
            };

            var created = await _repository.
                CreateAsync(entity, cancellationToken).ConfigureAwait(false);
            return created.SysBusinessActivityId;
        }

        public async Task<IEnumerable<SysBusinessActivityUpdateDTO>> GetAllAsync(CommonFilterDto filter, CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(filter, cancellationToken).ConfigureAwait(false);

            var result = new List<SysBusinessActivityUpdateDTO>();
            foreach (var item in items)
            {
                result.Add(new SysBusinessActivityUpdateDTO
                {
                    Name = item.BusinessActivityName,
                    Status = item.IsActive
                });
            }

            return result;
        }

        public async Task<SysBusinessActivityUpdateDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (entity == null)
            {
                return null;
            }

            return new SysBusinessActivityUpdateDTO
            {
                Name = entity.BusinessActivityName,
                Status = entity.IsActive
            };
        }

        public async Task<bool> UpdateAsync(int id, SysBusinessActivityUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            var existing = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                existing.BusinessActivityName = dto.Name;
            }

            if (dto.Status.HasValue)
            {
                existing.IsActive = dto.Status.Value;
            }

            existing.UpdatedAt = System.DateTime.UtcNow;

            return await _repository.UpdateAsync(existing, cancellationToken).ConfigureAwait(false);
        }

        public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            return _repository.DeleteAsync(id, cancellationToken);
        }
    }
}
