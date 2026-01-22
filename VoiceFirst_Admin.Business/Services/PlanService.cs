using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Plan;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Services
{
    public class PlanService:IPlanService
    {
        private readonly IPlanRepo _planRepository;

        public PlanService(IPlanRepo planRepository)
        {
            _planRepository = planRepository;
        }

        public async Task<ApiResponse<IEnumerable<PlanActiveDto>>> 
            GetActiveAsync(CancellationToken cancellationToken = default)
        {
            var items = await _planRepository.GetActiveAsync(cancellationToken);
            return ApiResponse<IEnumerable<PlanActiveDto>>.
                Ok(items, Messages.PlanRetrieved);
        }

        public async Task<ApiResponse<IEnumerable<VoiceFirst_Admin.Utilities.DTOs.Features.PlanProgramActoinLink.ProgramPlanDetailDto>>> GetProgramDetailsByPlanIdAsync(int planId, CancellationToken cancellationToken = default)
        {
            var list = await _planRepository.GetProgramDetailsByPlanIdAsync(planId, cancellationToken);
            return ApiResponse<IEnumerable<VoiceFirst_Admin.Utilities.DTOs.Features.PlanProgramActoinLink.ProgramPlanDetailDto>>.Ok(list, Messages.PlanRetrieved);
        }

        public async Task<ApiResponse<PlanDto>> CreateAsync(VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanCreateDto dto, int loginId, CancellationToken cancellationToken = default)
        {
            var existing = await _planRepository.GetByNameAsync(dto.PlanName, cancellationToken);
            if (existing != null && existing.IsDeleted == false)
            {
                return ApiResponse<PlanDto>.Fail(Messages.AlreadyExist, StatusCodes.Status409Conflict, ErrorCodes.Conflict);
            }
            if (existing != null && existing.IsDeleted == true)
            {
                return ApiResponse<PlanDto>.Fail(Messages.AlreadyExist, StatusCodes.Status422UnprocessableEntity, ErrorCodes.Conflict);
            }

            var entity = new VoiceFirst_Admin.Utilities.Models.Entities.Plan
            {
                PlanName = dto.PlanName,
                CreatedBy = loginId
            };
            var planId = await _planRepository.CreatePlanAsync(entity, cancellationToken);

            if (dto.ProgramActionLinkIds != null && dto.ProgramActionLinkIds.Count > 0)
            {
                await _planRepository.LinkProgramActionLinksAsync(planId, dto.ProgramActionLinkIds, loginId, cancellationToken);
            }

            var outDto = new PlanDto { PlanId = planId, PlanName = dto.PlanName };
            return ApiResponse<PlanDto>.Ok(outDto, Messages.Created, StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<bool>> UpdateAsync(int planId, VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanUpdateDto dto, int loginId, CancellationToken cancellationToken = default)
        {
            // Uniqueness check when PlanName provided
            if (!string.IsNullOrWhiteSpace(dto.PlanName))
            {
                var existing = await _planRepository.GetByNameAsync(dto.PlanName, cancellationToken);
                if (existing != null && existing.PlanId != planId)
                {
                    if (existing.IsDeleted == false)
                        return ApiResponse<bool>.Fail(Messages.AlreadyExist, StatusCodes.Status409Conflict, ErrorCodes.Conflict);
                    return ApiResponse<bool>.Fail(Messages.AlreadyExist, StatusCodes.Status422UnprocessableEntity, ErrorCodes.Conflict);
                }
            }

            var updated = await _planRepository.UpdatePlanAsync(planId, dto.PlanName, dto.Active, loginId, cancellationToken);
            if (!updated && (dto.ActionLinks == null || dto.ActionLinks.Count == 0))
            {
                return ApiResponse<bool>.Fail(Messages.UpdateFailed, StatusCodes.Status400BadRequest, ErrorCodes.ValidationFailed);
            }

            if (dto.ActionLinks != null && dto.ActionLinks.Count > 0)
            {
                await _planRepository.UpsertPlanProgramActionLinksAsync(planId, dto.ActionLinks, loginId, cancellationToken);
            }

            return ApiResponse<bool>.Ok(true, Messages.Updated);
        }

        public async Task<VoiceFirst_Admin.Utilities.DTOs.Shared.PagedResultDto<VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanDetailDto>> GetAllAsync(VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanFilterDto filter, CancellationToken cancellationToken = default)
        {
            var paged = await _planRepository.GetAllAsync(filter, cancellationToken);
            //var items = paged.Items.Select(p => new VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanDetailDto
            //{
            //    PlanId = p.PlanId,
            //    PlanName = p.PlanName,
            //    Active = p.IsActive ,
            //    Deleted = p.IsDeleted ?? false,
            //    CreatedUser = p.CreatedUserName ?? string.Empty,
            //    CreatedDate = p.CreatedAt ?? System.DateTime.MinValue,
            //    ModifiedUser = p.UpdatedUserName ?? string.Empty,
            //    ModifiedDate = p.UpdatedAt,
            //    DeletedUser = p.DeletedUserName ?? string.Empty,
            //    DeletedDate = p.DeletedAt
            //}).ToList();

            return new VoiceFirst_Admin.Utilities.DTOs.Shared.PagedResultDto<VoiceFirst_Admin.Utilities.DTOs.Features.Plan.PlanDetailDto>
            {
                Items = paged.Items,
                TotalCount = paged.TotalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.Limit
            };
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {
            var deleted = await _planRepository.DeleteAsync(id, loginId, cancellationToken);
            if (!deleted)
            {
                return ApiResponse<bool>.Fail(Messages.NotFound, StatusCodes.Status404NotFound, ErrorCodes.NotFound);
            }
            return ApiResponse<bool>.Ok(true, Messages.Success);
        }

        public async Task<ApiResponse<int>> RecoverPlanAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {
            var recovered = await _planRepository.RecoverPlanAsync(id, loginId, cancellationToken);
            if (recovered <= 0)
            {
                return ApiResponse<int>.Fail(Messages.NotFound, StatusCodes.Status404NotFound, ErrorCodes.NotFound);
            }
            return ApiResponse<int>.Ok(recovered, Messages.Success);
        }

    }
}
