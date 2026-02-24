using AutoMapper;
using Microsoft.AspNetCore.Http;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueMediaType;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class SysIssueMediaTypeService : ISysIssueMediaTypeService
    {
        private readonly ISysIssueMediaTypeRepo _repo; private readonly IMapper _mapper;
        public SysIssueMediaTypeService(ISysIssueMediaTypeRepo repo, IMapper mapper) { _repo = repo; _mapper = mapper; }

        public async Task<ApiResponse<SysIssueMediaTypeDTO>> CreateAsync(SysIssueMediaTypeCreateDTO dto, int loginId, CancellationToken ct)
        { var ex = await _repo.ExistsAsync(dto.IssueMediaType, null, ct); if (ex != null) { if (!ex.Deleted) return ApiResponse<SysIssueMediaTypeDTO>.Fail(Messages.IssueMediaTypeAlreadyExists, StatusCodes.Status409Conflict, ErrorCodes.IssueMediaTypeAlreadyExists); return ApiResponse<SysIssueMediaTypeDTO>.Fail(Messages.IssueMediaTypeAlreadyExistsRecoverable, StatusCodes.Status422UnprocessableEntity, ErrorCodes.IssueMediaTypeAlreadyExistsRecoverable, new SysIssueMediaTypeDTO { IssueMediaTypeId = ex.IssueMediaTypeId }); } var e = _mapper.Map<SysIssueMediaType>(dto); e.CreatedBy = loginId; e.SysIssueMediaTypeId = await _repo.CreateAsync(e, ct); if (e.SysIssueMediaTypeId <= 0) return ApiResponse<SysIssueMediaTypeDTO>.Fail(Messages.SomethingWentWrong, StatusCodes.Status500InternalServerError, ErrorCodes.InternalServerError); return ApiResponse<SysIssueMediaTypeDTO>.Ok(await _repo.GetByIdAsync(e.SysIssueMediaTypeId, ct), Messages.IssueMediaTypeCreated, StatusCodes.Status201Created); }

        public async Task<ApiResponse<SysIssueMediaTypeDTO>?> GetByIdAsync(int id, CancellationToken ct = default)
        { var d = await _repo.GetByIdAsync(id, ct); if (d == null) return ApiResponse<SysIssueMediaTypeDTO>.Fail(Messages.IssueMediaTypeNotFoundById, StatusCodes.Status404NotFound, ErrorCodes.IssueMediaTypeNotFoundById); return ApiResponse<SysIssueMediaTypeDTO>.Ok(d, Messages.IssueMediaTypeRetrieved, StatusCodes.Status200OK); }

        public async Task<ApiResponse<PagedResultDto<SysIssueMediaTypeDTO>>> GetAllAsync(IssueMediaTypeFilterDTO filter, CancellationToken ct = default)
        {
            filter.PageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            filter.Limit = filter.Limit <= 0 ? 10 : filter.Limit;
            filter.Limit = Math.Min(filter.Limit, 60);
            var r = await _repo.GetAllAsync(filter, ct); 
            return ApiResponse<PagedResultDto<SysIssueMediaTypeDTO>>.Ok
                (r, r.TotalCount == 0 ? Messages.IssueMediaTypesNotFound :
                Messages.IssueMediaTypesRetrieved, statusCode: StatusCodes.Status200OK); 
        }

        public async Task<ApiResponse<List<SysIssueMediaTypeActiveDTO>>> GetActiveAsync(CancellationToken ct)
        { var r = await _repo.GetActiveAsync(ct) ?? new List<SysIssueMediaTypeActiveDTO>(); return ApiResponse<List<SysIssueMediaTypeActiveDTO>>.Ok(r, r.Count == 0 ? Messages.NoActiveIssueMediaTypes : Messages.IssueMediaTypesRetrieved, statusCode: StatusCodes.Status200OK); }

        public async Task<ApiResponse<SysIssueMediaTypeDTO>> UpdateAsync(SysIssueMediaTypeUpdateDTO dto, int id, int loginId, CancellationToken ct = default)
        { var ed = await _repo.IsIdExistAsync(id, ct); if (ed == null) return ApiResponse<SysIssueMediaTypeDTO>.Fail(Messages.IssueMediaTypeNotFoundById, StatusCodes.Status404NotFound, ErrorCodes.IssueMediaTypeNotFoundById); if (ed.Deleted) return ApiResponse<SysIssueMediaTypeDTO>.Fail(Messages.IssueMediaTypeNotFound, StatusCodes.Status409Conflict, ErrorCodes.IssueMediaTypeNotFound); if (!string.IsNullOrWhiteSpace(dto.IssueMediaType)) { var ex = await _repo.ExistsAsync(dto.IssueMediaType, id, ct); if (ex is not null) { if (!ex.Deleted) return ApiResponse<SysIssueMediaTypeDTO>.Fail(Messages.IssueMediaTypeAlreadyExists, StatusCodes.Status409Conflict, ErrorCodes.IssueMediaTypeAlreadyExists); return ApiResponse<SysIssueMediaTypeDTO>.Fail(Messages.IssueMediaTypeAlreadyExistsRecoverable, StatusCodes.Status422UnprocessableEntity, ErrorCodes.IssueMediaTypeAlreadyExistsRecoverable, new SysIssueMediaTypeDTO { IssueMediaTypeId = ex.IssueMediaTypeId }); } } var e = _mapper.Map<SysIssueMediaType>((dto, id, loginId)); if (!await _repo.UpdateAsync(e, ct)) return ApiResponse<SysIssueMediaTypeDTO>.Fail(Messages.IssueMediaTypeUpdated, StatusCodes.Status204NoContent, ErrorCodes.NoRowAffected); return ApiResponse<SysIssueMediaTypeDTO>.Ok(await _repo.GetByIdAsync(id, ct), Messages.IssueMediaTypeUpdated, statusCode: StatusCodes.Status200OK); }

        public async Task<ApiResponse<SysIssueMediaTypeDTO>> RecoverAsync(int id, int loginId, CancellationToken ct = default)
        { var ed = await _repo.IsIdExistAsync(id, ct); if (ed == null) return ApiResponse<SysIssueMediaTypeDTO>.Fail(Messages.IssueMediaTypeNotFoundById, StatusCodes.Status404NotFound, ErrorCodes.IssueMediaTypeNotFoundById); if (!ed.Deleted) return ApiResponse<SysIssueMediaTypeDTO>.Fail(Messages.IssueMediaTypeAlreadyRecovered, StatusCodes.Status409Conflict, ErrorCodes.IssueMediaTypeAlreadyRecovered); if (!await _repo.RecoverAsync(id, loginId, ct)) return ApiResponse<SysIssueMediaTypeDTO>.Fail(Messages.IssueMediaTypeAlreadyRecovered, StatusCodes.Status409Conflict, ErrorCodes.IssueMediaTypeAlreadyRecovered); return ApiResponse<SysIssueMediaTypeDTO>.Ok(await _repo.GetByIdAsync(id, ct), Messages.IssueMediaTypeRecovered, statusCode: StatusCodes.Status200OK); }

        public async Task<ApiResponse<SysIssueMediaTypeDTO>> DeleteAsync(int id, int loginId, CancellationToken ct = default)
        { var ed = await _repo.IsIdExistAsync(id, ct); if (ed == null) return ApiResponse<SysIssueMediaTypeDTO>.Fail(Messages.IssueMediaTypeNotFoundById, StatusCodes.Status404NotFound, ErrorCodes.IssueMediaTypeNotFoundById); if (ed.Deleted) return ApiResponse<SysIssueMediaTypeDTO>.Fail(Messages.IssueMediaTypeAlreadyDeleted, StatusCodes.Status409Conflict, ErrorCodes.IssueMediaTypeAlreadyDeleted); if (!await _repo.DeleteAsync(id, loginId, ct)) return ApiResponse<SysIssueMediaTypeDTO>.Fail(Messages.IssueMediaTypeAlreadyDeleted, StatusCodes.Status409Conflict, ErrorCodes.IssueMediaTypeAlreadyDeleted); return ApiResponse<SysIssueMediaTypeDTO>.Ok(await _repo.GetByIdAsync(id, ct), Messages.IssueMediaTypeDeleted, statusCode: StatusCodes.Status200OK); }
    }
}
