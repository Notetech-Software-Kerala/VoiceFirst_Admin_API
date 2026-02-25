using AutoMapper;
using Microsoft.AspNetCore.Http;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueCharacterType;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class SysIssueCharacterTypeService : ISysIssueCharacterTypeService
    {
        private readonly ISysIssueCharacterTypeRepo _repo;
        private readonly IMapper _mapper;
        public SysIssueCharacterTypeService(ISysIssueCharacterTypeRepo repository, IMapper mapper) { _repo = repository; _mapper = mapper; }

        public async Task<ApiResponse<SysIssueCharacterTypeDTO>> CreateAsync(SysIssueCharacterTypeCreateDTO dto, int loginId, CancellationToken cancellationToken)
        {
            var existing = await _repo.ExistsAsync
                (dto.IssueCharacterType, null, cancellationToken);

            if (existing != null)
            {
                if (!existing.Deleted) 
                    return ApiResponse<SysIssueCharacterTypeDTO>.Fail
                        (Messages.IssueCharacterTypeAlreadyExists, 
                        StatusCodes.Status409Conflict, 
                        ErrorCodes.IssueCharacterTypeAlreadyExists);

                return ApiResponse<SysIssueCharacterTypeDTO>.Fail
                    (
                    Messages.IssueCharacterTypeAlreadyExistsRecoverable, 
                    StatusCodes.Status422UnprocessableEntity,
                    ErrorCodes.IssueCharacterTypeAlreadyExistsRecoverable,
                    new SysIssueCharacterTypeDTO 
                    { IssueCharacterTypeId = existing.IssueCharacterTypeId });
            }
            var entity = _mapper.Map<SysIssueCharacterType>((dto, loginId)); 
      

            entity.SysIssueCharacterTypeId = await _repo.CreateAsync(entity, cancellationToken);
            if (entity.SysIssueCharacterTypeId <= 0) 
                return ApiResponse<SysIssueCharacterTypeDTO>.
                    Fail(Messages.SomethingWentWrong,
                    StatusCodes.Status500InternalServerError, 
                    ErrorCodes.InternalServerError);

            var createdDto =
                await _repo.GetByIdAsync(entity.SysIssueCharacterTypeId, cancellationToken);

            return ApiResponse<SysIssueCharacterTypeDTO>.Ok
                (createdDto,
                Messages.IssueCharacterTypeCreated, 
                StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<SysIssueCharacterTypeDTO>?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var dto = await _repo.GetByIdAsync(id, cancellationToken);
            if (dto == null) return ApiResponse<SysIssueCharacterTypeDTO>.Fail(Messages.IssueCharacterTypeNotFoundById, StatusCodes.Status404NotFound, ErrorCodes.IssueCharacterTypeNotFoundById);
            return ApiResponse<SysIssueCharacterTypeDTO>.Ok(dto, Messages.IssueCharacterTypeRetrieved, StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<PagedResultDto<SysIssueCharacterTypeDTO>>> GetAllAsync(IssueCharacterTypeFilterDTO filter, CancellationToken cancellationToken = default)
        {
            filter.PageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            filter.Limit = filter.Limit <= 0 ? 10 : filter.Limit;
            filter.Limit = Math.Min(filter.Limit, 30);
            var result = await _repo.GetAllAsync(filter, cancellationToken);
            return ApiResponse<PagedResultDto<SysIssueCharacterTypeDTO>>.Ok(result, result.TotalCount == 0 ? Messages.IssueCharacterTypesNotFound : Messages.IssueCharacterTypesRetrieved, statusCode: StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<List<SysIssueCharacterTypeActiveDTO>>> GetActiveAsync(CancellationToken cancellationToken)
        {
            var result = await _repo.GetActiveAsync(cancellationToken) ?? new List<SysIssueCharacterTypeActiveDTO>();
            return ApiResponse<List<SysIssueCharacterTypeActiveDTO>>.Ok(result, result.Count == 0 ? Messages.NoActiveIssueCharacterTypes : Messages.IssueCharacterTypesRetrieved, statusCode: StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<SysIssueCharacterTypeDTO>> UpdateAsync(SysIssueCharacterTypeUpdateDTO dto, int id, int loginId, CancellationToken cancellationToken = default)
        {
            var existDto = await _repo.IsIdExistAsync(id, cancellationToken);
            if (existDto == null) return ApiResponse<SysIssueCharacterTypeDTO>.Fail(Messages.IssueCharacterTypeNotFoundById, StatusCodes.Status404NotFound, ErrorCodes.IssueCharacterTypeNotFoundById);
            if (existDto.Deleted) return ApiResponse<SysIssueCharacterTypeDTO>.Fail(Messages.IssueCharacterTypeNotFound, StatusCodes.Status409Conflict, ErrorCodes.IssueCharacterTypeNotFound);
            if (!string.IsNullOrWhiteSpace(dto.IssueCharacterType))
            {
                var existing = await _repo.ExistsAsync(dto.IssueCharacterType, id, cancellationToken);
                if (existing is not null)
                {
                    if (!existing.Deleted) return ApiResponse<SysIssueCharacterTypeDTO>.Fail(Messages.IssueCharacterTypeAlreadyExists, StatusCodes.Status409Conflict, ErrorCodes.IssueCharacterTypeAlreadyExists);
                    return ApiResponse<SysIssueCharacterTypeDTO>.Fail(Messages.IssueCharacterTypeAlreadyExistsRecoverable, StatusCodes.Status422UnprocessableEntity, ErrorCodes.IssueCharacterTypeAlreadyExistsRecoverable, new SysIssueCharacterTypeDTO { IssueCharacterTypeId = existing.IssueCharacterTypeId });
                }
            }
            var entity = _mapper.Map<SysIssueCharacterType>((dto, id, loginId));
            var updated = await _repo.UpdateAsync(entity, cancellationToken);
            if (!updated) return ApiResponse<SysIssueCharacterTypeDTO>.Fail(Messages.IssueCharacterTypeUpdated, StatusCodes.Status204NoContent, ErrorCodes.NoRowAffected);
            var updatedEntity = await _repo.GetByIdAsync(id, cancellationToken);
            return ApiResponse<SysIssueCharacterTypeDTO>.Ok(updatedEntity, Messages.IssueCharacterTypeUpdated, statusCode: StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<SysIssueCharacterTypeDTO>> RecoverAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {
            var existDto = await _repo.IsIdExistAsync(id, cancellationToken);
            if (existDto == null) return ApiResponse<SysIssueCharacterTypeDTO>.Fail(Messages.IssueCharacterTypeNotFoundById, StatusCodes.Status404NotFound, ErrorCodes.IssueCharacterTypeNotFoundById);
            if (!existDto.Deleted) return ApiResponse<SysIssueCharacterTypeDTO>.Fail(Messages.IssueCharacterTypeAlreadyRecovered, StatusCodes.Status409Conflict, ErrorCodes.IssueCharacterTypeAlreadyRecovered);
            if (!await _repo.RecoverAsync(id, loginId, cancellationToken)) return ApiResponse<SysIssueCharacterTypeDTO>.Fail(Messages.IssueCharacterTypeAlreadyRecovered, StatusCodes.Status409Conflict, ErrorCodes.IssueCharacterTypeAlreadyRecovered);
            var dto = await _repo.GetByIdAsync(id, cancellationToken);
            return ApiResponse<SysIssueCharacterTypeDTO>.Ok(dto, Messages.IssueCharacterTypeRecovered, statusCode: StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<SysIssueCharacterTypeDTO>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {
            var existDto = await _repo.IsIdExistAsync(id, cancellationToken);
            if (existDto == null) return ApiResponse<SysIssueCharacterTypeDTO>.Fail(Messages.IssueCharacterTypeNotFoundById, StatusCodes.Status404NotFound, ErrorCodes.IssueCharacterTypeNotFoundById);
            if (existDto.Deleted) return ApiResponse<SysIssueCharacterTypeDTO>.Fail(Messages.IssueCharacterTypeAlreadyDeleted, StatusCodes.Status409Conflict, ErrorCodes.IssueCharacterTypeAlreadyDeleted);
            if (!await _repo.DeleteAsync(id, loginId, cancellationToken)) return ApiResponse<SysIssueCharacterTypeDTO>.Fail(Messages.IssueCharacterTypeAlreadyDeleted, StatusCodes.Status409Conflict, ErrorCodes.IssueCharacterTypeAlreadyDeleted);
            var dto = await _repo.GetByIdAsync(id, cancellationToken);
            return ApiResponse<SysIssueCharacterTypeDTO>.Ok(dto, Messages.IssueCharacterTypeDeleted, statusCode: StatusCodes.Status200OK);
        }
    }
}
