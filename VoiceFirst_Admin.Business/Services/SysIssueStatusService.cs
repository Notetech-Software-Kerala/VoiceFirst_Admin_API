using AutoMapper;
using Microsoft.AspNetCore.Http;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueStatus;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class SysIssueStatusService : ISysIssueStatusService
    {
        private readonly ISysIssueStatusRepo _repo;
        private readonly IMapper _mapper;

        public SysIssueStatusService(ISysIssueStatusRepo repository, IMapper mapper)
        {
            _repo = repository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<SysIssueStatusDTO>> CreateAsync(SysIssueStatusCreateDTO dto, int loginId, CancellationToken cancellationToken)
        {
            var existing = await _repo.IssueStatusExistsAsync(dto.IssueStatus, null, cancellationToken);
            if (existing != null)
            {
                if (!existing.Deleted)
                    return ApiResponse<SysIssueStatusDTO>.Fail(Messages.IssueStatusAlreadyExists, StatusCodes.Status409Conflict, ErrorCodes.IssueStatusAlreadyExists);
                return ApiResponse<SysIssueStatusDTO>.Fail(Messages.IssueStatusAlreadyExistsRecoverable, StatusCodes.Status422UnprocessableEntity, ErrorCodes.IssueStatusAlreadyExistsRecoverable, new SysIssueStatusDTO { IssueStatusId = existing.IssueStatusId });
            }
            var entity = _mapper.Map<SysIssueStatus>(dto);
            entity.CreatedBy = loginId;
            entity.SysIssueStatusId = await _repo.CreateAsync(entity, cancellationToken);
            if (entity.SysIssueStatusId <= 0)
                return ApiResponse<SysIssueStatusDTO>.Fail(Messages.SomethingWentWrong, StatusCodes.Status500InternalServerError, ErrorCodes.InternalServerError);
            var createdDto = await _repo.GetByIdAsync(entity.SysIssueStatusId, cancellationToken);
            return ApiResponse<SysIssueStatusDTO>.Ok(createdDto, Messages.IssueStatusCreated, StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<SysIssueStatusDTO>?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var dto = await _repo.GetByIdAsync(id, cancellationToken);
            if (dto == null) return ApiResponse<SysIssueStatusDTO>.Fail(Messages.IssueStatusNotFoundById, StatusCodes.Status404NotFound, ErrorCodes.IssueStatusNotFoundById);
            return ApiResponse<SysIssueStatusDTO>.Ok(dto, Messages.IssueStatusRetrieved, StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<PagedResultDto<SysIssueStatusDTO>>> GetAllAsync(IssueStatusFilterDTO filter, CancellationToken cancellationToken = default)
        {
            filter.PageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            filter.Limit = filter.Limit <= 0 ? 10 : filter.Limit;
            filter.Limit = Math.Min(filter.Limit, 60);
            var result = await _repo.GetAllAsync(filter, cancellationToken);
            return ApiResponse<PagedResultDto<SysIssueStatusDTO>>.Ok(result, result.TotalCount == 0 ? Messages.IssueStatusesNotFound : Messages.IssueStatusesRetrieved, statusCode: StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<List<SysIssueStatusActiveDTO>>> GetActiveAsync(CancellationToken cancellationToken)
        {
            var result = await _repo.GetActiveAsync(cancellationToken) ?? new List<SysIssueStatusActiveDTO>();
            return ApiResponse<List<SysIssueStatusActiveDTO>>.Ok(result, result.Count == 0 ? Messages.NoActiveIssueStatuses : Messages.IssueStatusesRetrieved, statusCode: StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<SysIssueStatusDTO>> UpdateAsync(SysIssueStatusUpdateDTO dto, int id, int loginId, CancellationToken cancellationToken = default)
        {
            var existDto = await _repo.IsIdExistAsync(id, cancellationToken);
            if (existDto == null) return ApiResponse<SysIssueStatusDTO>.Fail(Messages.IssueStatusNotFoundById, StatusCodes.Status404NotFound, ErrorCodes.IssueStatusNotFoundById);
            if (existDto.Deleted) return ApiResponse<SysIssueStatusDTO>.Fail(Messages.IssueStatusNotFound, StatusCodes.Status409Conflict, ErrorCodes.IssueStatusNotFound);
            if (!string.IsNullOrWhiteSpace(dto.IssueStatus))
            {
                var existing = await _repo.IssueStatusExistsAsync(dto.IssueStatus, id, cancellationToken);
                if (existing is not null)
                {
                    if (!existing.Deleted) return ApiResponse<SysIssueStatusDTO>.Fail(Messages.IssueStatusAlreadyExists, StatusCodes.Status409Conflict, ErrorCodes.IssueStatusAlreadyExists);
                    return ApiResponse<SysIssueStatusDTO>.Fail(Messages.IssueStatusAlreadyExistsRecoverable, StatusCodes.Status422UnprocessableEntity, ErrorCodes.IssueStatusAlreadyExistsRecoverable, new SysIssueStatusDTO { IssueStatusId = existing.IssueStatusId });
                }
            }
            var entity = _mapper.Map<SysIssueStatus>((dto, id, loginId));
            var updated = await _repo.UpdateAsync(entity, cancellationToken);
            if (!updated) return ApiResponse<SysIssueStatusDTO>.Fail(Messages.IssueStatusUpdated, StatusCodes.Status204NoContent, ErrorCodes.NoRowAffected);
            var updatedEntity = await _repo.GetByIdAsync(id, cancellationToken);
            return ApiResponse<SysIssueStatusDTO>.Ok(updatedEntity, Messages.IssueStatusUpdated, statusCode: StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<SysIssueStatusDTO>> RecoverIssueStatusAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {
            var existDto = await _repo.IsIdExistAsync(id, cancellationToken);
            if (existDto == null) return ApiResponse<SysIssueStatusDTO>.Fail(Messages.IssueStatusNotFoundById, StatusCodes.Status404NotFound, ErrorCodes.IssueStatusNotFoundById);
            if (!existDto.Deleted) return ApiResponse<SysIssueStatusDTO>.Fail(Messages.IssueStatusAlreadyRecovered, StatusCodes.Status409Conflict, ErrorCodes.IssueStatusAlreadyRecovered);
            var rowAffect = await _repo.RecoverIssueStatusAsync(id, loginId, cancellationToken);
            if (!rowAffect) return ApiResponse<SysIssueStatusDTO>.Fail(Messages.IssueStatusAlreadyRecovered, StatusCodes.Status409Conflict, ErrorCodes.IssueStatusAlreadyRecovered);
            var dto = await _repo.GetByIdAsync(id, cancellationToken);
            return ApiResponse<SysIssueStatusDTO>.Ok(dto, Messages.IssueStatusRecovered, statusCode: StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<SysIssueStatusDTO>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {
            var existDto = await _repo.IsIdExistAsync(id, cancellationToken);
            if (existDto == null) return ApiResponse<SysIssueStatusDTO>.Fail(Messages.IssueStatusNotFoundById, StatusCodes.Status404NotFound, ErrorCodes.IssueStatusNotFoundById);
            if (existDto.Deleted) return ApiResponse<SysIssueStatusDTO>.Fail(Messages.IssueStatusAlreadyDeleted, StatusCodes.Status409Conflict, ErrorCodes.IssueStatusAlreadyDeleted);
            var rowAffect = await _repo.DeleteAsync(id, loginId, cancellationToken);
            if (!rowAffect) return ApiResponse<SysIssueStatusDTO>.Fail(Messages.IssueStatusAlreadyDeleted, StatusCodes.Status409Conflict, ErrorCodes.IssueStatusAlreadyDeleted);
            var dto = await _repo.GetByIdAsync(id, cancellationToken);
            return ApiResponse<SysIssueStatusDTO>.Ok(dto, Messages.IssueStatusDeleted, statusCode: StatusCodes.Status200OK);
        }
    }
}
