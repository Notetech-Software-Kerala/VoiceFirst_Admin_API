using AutoMapper;
using Microsoft.AspNetCore.Http;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueMediaFormat;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class SysIssueMediaFormatService : ISysIssueMediaFormatService
    {
        private readonly ISysIssueMediaFormatRepo _repo; private readonly IMapper _mapper;
        public SysIssueMediaFormatService(ISysIssueMediaFormatRepo repo, IMapper mapper) { _repo = repo; _mapper = mapper; }

        public async Task<ApiResponse<SysIssueMediaFormatDTO>> CreateAsync(SysIssueMediaFormatCreateDTO dto, int loginId, CancellationToken ct)
        { var ex = await _repo.ExistsAsync(dto.IssueMediaFormat, null, ct); if (ex != null) { if (!ex.Deleted) return ApiResponse<SysIssueMediaFormatDTO>.Fail(Messages.IssueMediaFormatAlreadyExists, StatusCodes.Status409Conflict, ErrorCodes.IssueMediaFormatAlreadyExists); return ApiResponse<SysIssueMediaFormatDTO>.Fail(Messages.IssueMediaFormatAlreadyExistsRecoverable, StatusCodes.Status422UnprocessableEntity, ErrorCodes.IssueMediaFormatAlreadyExistsRecoverable, new SysIssueMediaFormatDTO { IssueMediaFormatId = ex.IssueMediaFormatId }); } var e = _mapper.Map<SysIssueMediaFormat>(dto); e.CreatedBy = loginId; e.SysIssueMediaFormatId = await _repo.CreateAsync(e, ct); if (e.SysIssueMediaFormatId <= 0) return ApiResponse<SysIssueMediaFormatDTO>.Fail(Messages.SomethingWentWrong, StatusCodes.Status500InternalServerError, ErrorCodes.InternalServerError); return ApiResponse<SysIssueMediaFormatDTO>.Ok(await _repo.GetByIdAsync(e.SysIssueMediaFormatId, ct), Messages.IssueMediaFormatCreated, StatusCodes.Status201Created); }

        public async Task<ApiResponse<SysIssueMediaFormatDTO>?> GetByIdAsync(int id, CancellationToken ct = default)
        { var d = await _repo.GetByIdAsync(id, ct); if (d == null) return ApiResponse<SysIssueMediaFormatDTO>.Fail(Messages.IssueMediaFormatNotFoundById, StatusCodes.Status404NotFound, ErrorCodes.IssueMediaFormatNotFoundById); return ApiResponse<SysIssueMediaFormatDTO>.Ok(d, Messages.IssueMediaFormatRetrieved, StatusCodes.Status200OK); }

        public async Task<ApiResponse<PagedResultDto<SysIssueMediaFormatDTO>>> GetAllAsync(IssueMediaFormatFilterDTO filter, CancellationToken ct = default)
        { var r = await _repo.GetAllAsync(filter, ct); return ApiResponse<PagedResultDto<SysIssueMediaFormatDTO>>.Ok(r, r.TotalCount == 0 ? Messages.IssueMediaFormatsNotFound : Messages.IssueMediaFormatsRetrieved, statusCode: StatusCodes.Status200OK); }

        public async Task<ApiResponse<List<SysIssueMediaFormatActiveDTO>>> GetActiveAsync(CancellationToken ct)
        { var r = await _repo.GetActiveAsync(ct) ?? new List<SysIssueMediaFormatActiveDTO>(); return ApiResponse<List<SysIssueMediaFormatActiveDTO>>.Ok(r, r.Count == 0 ? Messages.NoActiveIssueMediaFormats : Messages.IssueMediaFormatsRetrieved, statusCode: StatusCodes.Status200OK); }

        public async Task<ApiResponse<SysIssueMediaFormatDTO>> UpdateAsync(SysIssueMediaFormatUpdateDTO dto, int id, int loginId, CancellationToken ct = default)
        { var ed = await _repo.IsIdExistAsync(id, ct); if (ed == null) return ApiResponse<SysIssueMediaFormatDTO>.Fail(Messages.IssueMediaFormatNotFoundById, StatusCodes.Status404NotFound, ErrorCodes.IssueMediaFormatNotFoundById); if (ed.Deleted) return ApiResponse<SysIssueMediaFormatDTO>.Fail(Messages.IssueMediaFormatNotFound, StatusCodes.Status409Conflict, ErrorCodes.IssueMediaFormatNotFound); if (!string.IsNullOrWhiteSpace(dto.IssueMediaFormat)) { var ex = await _repo.ExistsAsync(dto.IssueMediaFormat, id, ct); if (ex is not null) { if (!ex.Deleted) return ApiResponse<SysIssueMediaFormatDTO>.Fail(Messages.IssueMediaFormatAlreadyExists, StatusCodes.Status409Conflict, ErrorCodes.IssueMediaFormatAlreadyExists); return ApiResponse<SysIssueMediaFormatDTO>.Fail(Messages.IssueMediaFormatAlreadyExistsRecoverable, StatusCodes.Status422UnprocessableEntity, ErrorCodes.IssueMediaFormatAlreadyExistsRecoverable, new SysIssueMediaFormatDTO { IssueMediaFormatId = ex.IssueMediaFormatId }); } } var e = _mapper.Map<SysIssueMediaFormat>((dto, id, loginId)); if (!await _repo.UpdateAsync(e, ct)) return ApiResponse<SysIssueMediaFormatDTO>.Fail(Messages.IssueMediaFormatUpdated, StatusCodes.Status204NoContent, ErrorCodes.NoRowAffected); return ApiResponse<SysIssueMediaFormatDTO>.Ok(await _repo.GetByIdAsync(id, ct), Messages.IssueMediaFormatUpdated, statusCode: StatusCodes.Status200OK); }

        public async Task<ApiResponse<SysIssueMediaFormatDTO>> RecoverAsync(int id, int loginId, CancellationToken ct = default)
        { var ed = await _repo.IsIdExistAsync(id, ct); if (ed == null) return ApiResponse<SysIssueMediaFormatDTO>.Fail(Messages.IssueMediaFormatNotFoundById, StatusCodes.Status404NotFound, ErrorCodes.IssueMediaFormatNotFoundById); if (!ed.Deleted) return ApiResponse<SysIssueMediaFormatDTO>.Fail(Messages.IssueMediaFormatAlreadyRecovered, StatusCodes.Status409Conflict, ErrorCodes.IssueMediaFormatAlreadyRecovered); if (!await _repo.RecoverAsync(id, loginId, ct)) return ApiResponse<SysIssueMediaFormatDTO>.Fail(Messages.IssueMediaFormatAlreadyRecovered, StatusCodes.Status409Conflict, ErrorCodes.IssueMediaFormatAlreadyRecovered); return ApiResponse<SysIssueMediaFormatDTO>.Ok(await _repo.GetByIdAsync(id, ct), Messages.IssueMediaFormatRecovered, statusCode: StatusCodes.Status200OK); }

        public async Task<ApiResponse<SysIssueMediaFormatDTO>> DeleteAsync(int id, int loginId, CancellationToken ct = default)
        { var ed = await _repo.IsIdExistAsync(id, ct); if (ed == null) return ApiResponse<SysIssueMediaFormatDTO>.Fail(Messages.IssueMediaFormatNotFoundById, StatusCodes.Status404NotFound, ErrorCodes.IssueMediaFormatNotFoundById); if (ed.Deleted) return ApiResponse<SysIssueMediaFormatDTO>.Fail(Messages.IssueMediaFormatAlreadyDeleted, StatusCodes.Status409Conflict, ErrorCodes.IssueMediaFormatAlreadyDeleted); if (!await _repo.DeleteAsync(id, loginId, ct)) return ApiResponse<SysIssueMediaFormatDTO>.Fail(Messages.IssueMediaFormatAlreadyDeleted, StatusCodes.Status409Conflict, ErrorCodes.IssueMediaFormatAlreadyDeleted); return ApiResponse<SysIssueMediaFormatDTO>.Ok(await _repo.GetByIdAsync(id, ct), Messages.IssueMediaFormatDeleted, statusCode: StatusCodes.Status200OK); }
    }
}
