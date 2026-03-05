using AutoMapper;
using Microsoft.AspNetCore.Http;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueCharacterType;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Exceptions;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class SysIssueTypeService : ISysIssueTypeService
    {
        private readonly ISysIssueTypeRepo _repo;
        private readonly ISysIssueMediaFormatRepo _mediaFormatRepo;
        private readonly ISysIssueMediaTypeRepo _mediaTypeRepo;
        private readonly ISysIssueMediaRuleRepo _mediaRuleRepo;
        private readonly ISysIssueMediaRuleTypeRepo _mediaRuleTypeRepo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public SysIssueTypeService(
            ISysIssueTypeRepo repository,
            ISysIssueMediaFormatRepo mediaFormatRepo,
            ISysIssueMediaTypeRepo mediaTypeRepo,
            ISysIssueMediaRuleRepo mediaRuleRepo,
            ISysIssueMediaRuleTypeRepo mediaRuleTypeRepo,
            IMapper mapper,
            IUnitOfWork uow)
        {
            _repo = repository;
            _mediaFormatRepo = mediaFormatRepo;
            _mediaTypeRepo = mediaTypeRepo;
            _mediaRuleRepo = mediaRuleRepo;
            _mediaRuleTypeRepo = mediaRuleTypeRepo;
            _mapper = mapper;
            _uow = uow;
        }


        // ───────────────────────── CREATE ─────────────────────────

        public async Task<ApiResponse<SysIssueTypeDTO>> CreateAsync(
         SysIssueTypeCreateDTO dto,
         int loginId,
         CancellationToken cancellationToken)
        {
            dto.IssueType = dto.IssueType.Trim();

            var validationError = await ValidateExistingAsync(dto.IssueType, cancellationToken);
            if (validationError != null)
                return validationError;

            if (dto.MediaRules?.Count > 0)
            {
                var mediaError = await ValidateMediaRulesAsync(dto, cancellationToken);
                if (mediaError != null)
                    return mediaError;
            }

            try
            {
                return await CreateIssueTypeInternalAsync(dto, loginId, cancellationToken);
            }
            catch (DuplicateException)
            {
                return await HandleDuplicateAsync(dto.IssueType, cancellationToken);
            }
        }



        private async Task<ApiResponse<SysIssueTypeDTO>>
            HandleDuplicateAsync(
            string normalizedName,
            CancellationToken cancellationToken)
        {
            var existing = await _repo.GetIdAndDeletedByNameAsync(
                normalizedName,
                null,
                cancellationToken);

            if (existing == null)
                throw new InvalidOperationException(
                    "Duplicate detected but record not found.");

            var minimalDto = new SysIssueTypeDTO
            {
                IssueTypeId = existing.IssueTypeId
            };

            if (existing.Deleted)
            {
                return ApiResponse<SysIssueTypeDTO>.Fail(
                    Messages.IssueTypeAlreadyExistsRecoverable,
                    StatusCodes.Status422UnprocessableEntity,
                    ErrorCodes.IssueTypeAlreadyExistsRecoverable,
                    minimalDto);
            }

            return ApiResponse<SysIssueTypeDTO>.Fail(
                Messages.IssueTypeAlreadyExists,
                StatusCodes.Status409Conflict,
                ErrorCodes.IssueTypeAlreadyExists,
                minimalDto);
        }


        private async Task<ApiResponse<SysIssueTypeDTO>?> ValidateExistingAsync(
            string issueType,
            CancellationToken cancellationToken)
        {
            var existing = await _repo.IssueTypeExistsAsync(issueType, null, cancellationToken);

            if (existing == null)
                return null;

            if (!existing.Deleted)
            {
                return ApiResponse<SysIssueTypeDTO>.Fail(
                    Messages.IssueTypeAlreadyExists,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.IssueTypeAlreadyExists);
            }

            return ApiResponse<SysIssueTypeDTO>.Fail(
                Messages.IssueTypeAlreadyExistsRecoverable,
                StatusCodes.Status422UnprocessableEntity,
                ErrorCodes.IssueTypeAlreadyExistsRecoverable,
                new SysIssueTypeDTO
                {
                    IssueTypeId = existing.IssueTypeId
                });
        }


        private async Task<ApiResponse<SysIssueTypeDTO>?> ValidateMediaRulesAsync(
            SysIssueTypeCreateDTO dto,
            CancellationToken cancellationToken)
        {
            var formatIds = dto.MediaRules!
                .Select(r => r.IssueMediaFormatId)
                .Distinct()
                .ToList();

            var formatValidation = await _mediaFormatRepo
                .IsBulkIdsExistAsync(formatIds, cancellationToken);

            var formatError = HandleMediaFormatValidation(formatValidation);
            if (formatError != null)
                return formatError;

            var typeIds = dto.MediaRules!
                .Where(r => r.MediaTypes?.Count > 0)
                .SelectMany(r => r.MediaTypes!)
                .Select(mt => mt.IssueMediaTypeId)
                .Distinct()
                .ToList();

            if (typeIds.Count == 0)
                return null;

            var typeValidation = await _mediaTypeRepo
                .IsBulkIdsExistAsync(typeIds, cancellationToken);

            return HandleMediaTypeValidation(typeValidation);
        }


        private async Task<ApiResponse<SysIssueTypeDTO>> CreateIssueTypeInternalAsync(
            SysIssueTypeCreateDTO dto,
            int loginId,
            CancellationToken cancellationToken)
        {
           
            await _uow.BeginAsync();
            try
            {
                var entity = _mapper.Map<SysIssueType>(dto);
                entity.CreatedBy = loginId;


                entity.SysIssueTypeId = await _repo.CreateAsync(
                  entity, _uow.Connection, _uow.Transaction, cancellationToken);

                if (entity.SysIssueTypeId <= 0)

                    return InternalError();


                var hasMediaRules = dto.MediaRules?.Count > 0;
                if (hasMediaRules)
                {
                    await CreateMediaRulesAsync(entity.SysIssueTypeId, dto, loginId, cancellationToken);                                    
                }
                await _uow.CommitAsync();
                var createdDto = await _repo.GetByIdAsync(entity.SysIssueTypeId, cancellationToken);
                return ApiResponse<SysIssueTypeDTO>.Ok(
                    createdDto,
                    Messages.IssueTypeCreated,
                    StatusCodes.Status201Created);                                                        
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }




        private async Task CreateMediaRulesAsync(
            int issueTypeId,
            SysIssueTypeCreateDTO dto,
            int loginId,
            CancellationToken cancellationToken)
        {
            foreach (var ruleDto in dto.MediaRules!)
            {
                var rule = _mapper.Map<SysIssueMediaRule>((ruleDto, issueTypeId, loginId));

                var ruleId = await _mediaRuleRepo.CreateAsync(
                    rule, _uow.Connection, _uow.Transaction, cancellationToken);

                if (ruleId <= 0)
                    throw new InvalidOperationException("Media rule creation failed.");

                if (ruleDto.MediaTypes?.Count > 0)
                {
                    await _mediaRuleTypeRepo.BulkInsertAsync(
                        ruleId,
                        ruleDto.MediaTypes,
                        loginId,
                        _uow.Connection,
                        _uow.Transaction,
                        cancellationToken);
                }
            }
        }


        // ───────────────────────── READ ─────────────────────────

        public async Task<ApiResponse<SysIssueTypeDTO>?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var dto = await _repo.GetByIdAsync(id, cancellationToken);

            if (dto == null)
            {
                return ApiResponse<SysIssueTypeDTO>.Fail(
                    Messages.IssueTypeNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.IssueTypeNotFoundById);
            }

            return ApiResponse<SysIssueTypeDTO>.Ok(
                dto,
                Messages.IssueTypeRetrieved,
                StatusCodes.Status200OK);
        }


        public async Task<ApiResponse<PagedResultDto<SysIssueTypeDTO>>>
            GetAllAsync(IssueTypeFilterDTO filter,
            CancellationToken cancellationToken = default)
        {
            filter.PageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            filter.Limit = filter.Limit <= 0 ? 10 : filter.Limit;
            filter.Limit = Math.Min(filter.Limit, 30);
            var result = await _repo.GetAllAsync(filter, cancellationToken);

            return ApiResponse<PagedResultDto<SysIssueTypeDTO>>.Ok(
                result,
                result.TotalCount == 0
                    ? Messages.IssueTypesNotFound
                    : Messages.IssueTypesRetrieved,
                statusCode: StatusCodes.Status200OK);
        }


        public async Task<ApiResponse<List<SysIssueTypeActiveDTO>>>
            GetActiveAsync(CancellationToken cancellationToken)
        {
            var result = await _repo.GetActiveAsync(cancellationToken)
                         ?? new List<SysIssueTypeActiveDTO>();

            return ApiResponse<List<SysIssueTypeActiveDTO>>.Ok(
                result,
                result.Count == 0
                    ? Messages.NoActiveIssueTypes
                    : Messages.IssueTypesRetrieved,
                statusCode: StatusCodes.Status200OK);
        }


        // ───────────────────────── UPDATE ─────────────────────────

        public async Task<ApiResponse<SysIssueTypeDTO>> UpdateAsync(
            SysIssueTypeUpdateDTO dto,
            int sysIssueTypeId,
            int loginId,
            CancellationToken cancellationToken = default)
        {
            var existDto = await _repo.IsIdExistAsync(sysIssueTypeId, cancellationToken);

            if (existDto == null)
            {
                return ApiResponse<SysIssueTypeDTO>.Fail(
                    Messages.IssueTypeNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.IssueTypeNotFoundById);
            }
            else if (existDto.Deleted)
            {
                return ApiResponse<SysIssueTypeDTO>.Fail(
                    Messages.IssueTypeNotAvailable,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.IssueTypeNotAvailable);
            }

            if (!string.IsNullOrWhiteSpace(dto.IssueType))
            {
                var existingEntity = await _repo.IssueTypeExistsAsync(
                    dto.IssueType, sysIssueTypeId, cancellationToken);

                if (existingEntity is not null)
                {
                    if (!existingEntity.Deleted)
                    {
                        return ApiResponse<SysIssueTypeDTO>.Fail(
                            Messages.IssueTypeAlreadyExists,
                            StatusCodes.Status409Conflict,
                            ErrorCodes.IssueTypeAlreadyExists);
                    }
                    return ApiResponse<SysIssueTypeDTO>.Fail(
                        Messages.IssueTypeAlreadyExistsRecoverable,
                        StatusCodes.Status422UnprocessableEntity,
                        ErrorCodes.IssueTypeAlreadyExistsRecoverable,
                        new SysIssueTypeDTO { IssueTypeId = existingEntity.IssueTypeId });
                }
            }

            var hasUpdateRules = dto.UpdateMediaRules != null && dto.UpdateMediaRules.Count > 0;
            var hasInsertRules = dto.InsertMediaRules != null && dto.InsertMediaRules.Count > 0;

            if (!hasUpdateRules && !hasInsertRules)
            {
                if (dto.Active == null && dto.IssueType == null)
                {
                    return ApiResponse<SysIssueTypeDTO>.Fail(
                        Messages.IssueTypeUpdated,
                        StatusCodes.Status204NoContent,
                        ErrorCodes.NoRowAffected);
                }

                var entity = _mapper.Map<SysIssueType>((dto, sysIssueTypeId, loginId));
                var updated = await _repo.UpdateAsync(entity, cancellationToken);

                if (!updated)
                    throw new InvalidOperationException("No rows were affected during the update.");

                var updatedEntity = await _repo.GetByIdAsync(sysIssueTypeId, cancellationToken);
                return ApiResponse<SysIssueTypeDTO>.Ok(
                    updatedEntity,
                    Messages.IssueTypeUpdated,
                    statusCode: StatusCodes.Status200OK);
            }

            await _uow.BeginAsync();
            try
            {
                if (dto.Active != null && dto.IssueType != null)
                {
                    var entity = _mapper.Map<SysIssueType>((dto, sysIssueTypeId, loginId));
                    var updated = await _repo.UpdateAsync(
                        entity, _uow.Connection, _uow.Transaction, cancellationToken);

                    if (!updated)
                    {
                        await _uow.RollbackAsync();
                        return ApiResponse<SysIssueTypeDTO>.Fail(
                            Messages.IssueTypeUpdated,
                            StatusCodes.Status204NoContent,
                            ErrorCodes.NoRowAffected);
                    }
                }

                if (hasUpdateRules)
                {
                    var updateFormatIds = dto.UpdateMediaRules!
                        .Select(r => r.IssueMediaFormatId).Distinct().ToList();

                    var checkRules = await _mediaRuleRepo.IsBulkExistAsync(
                        sysIssueTypeId, updateFormatIds,
                        _uow.Connection, _uow.Transaction, cancellationToken);

                    if (checkRules.IdNotFound)
                    {
                        await _uow.RollbackAsync();
                        return ApiResponse<SysIssueTypeDTO>.Fail(
                            Messages.MediaFormatIsNotFound,
                            StatusCodes.Status404NotFound,
                            ErrorCodes.MediaFormatNotLinked);
                    }

                    var updateMediaRuleEntities = _mapper.Map<List<SysIssueMediaRule>>(
                        dto.UpdateMediaRules.Select(r => (r, sysIssueTypeId, loginId)));

                    await _mediaRuleRepo.BulkUpdateAsync(
                        updateMediaRuleEntities,
                        _uow.Connection, _uow.Transaction, cancellationToken);

                    var allTypeIdsToCheck = dto.UpdateMediaRules
                        .SelectMany(r => r.MediaTypes ?? new List<IssueMediaRuleTypeUpdateDTO>())
                        .Select(mt => mt.IssueMediaTypeId).Distinct().ToList();

                    if (allTypeIdsToCheck.Count > 0)
                    {
                        var checkTypes = await _mediaTypeRepo.IsBulkIdsExistAsync(
                            allTypeIdsToCheck,
                            _uow.Connection, _uow.Transaction, cancellationToken);

                        if (checkTypes.IdNotFound)
                        {
                            await _uow.RollbackAsync();
                            return ApiResponse<SysIssueTypeDTO>.Fail(
                                Messages.MediaTypeIsNotFound,
                                StatusCodes.Status404NotFound,
                                ErrorCodes.MediaTypeNotLinked);
                        }
                    }

                    var existingRules = (await _mediaRuleRepo.GetByIssueTypeAndFormatsAsync(
                        sysIssueTypeId, updateFormatIds,
                        _uow.Connection, _uow.Transaction, cancellationToken)).ToList();

                    var ruleMap = existingRules.ToDictionary(
                        r => r.IssueMediaFormatId, r => r.SysIssueMediaRuleId);

                    var updateRuleTypeDtos = new List<SysIssueMediaRuleType>();
                    foreach (var ruleDto in dto.UpdateMediaRules)
                    {
                        if (ruleDto.MediaTypes == null) continue;
                        var ruleId = ruleMap[ruleDto.IssueMediaFormatId];
                        foreach (var mt in ruleDto.MediaTypes)
                        {
                            updateRuleTypeDtos.Add(new SysIssueMediaRuleType
                            {
                                IssueMediaRuleId = ruleId,
                                IssueMediaTypeId = mt.IssueMediaTypeId,
                                IsMandatory = mt.IsMandatory,
                                IsActive = mt.Active ?? true,
                                UpdatedBy = loginId
                            });
                        }
                    }

                    if (updateRuleTypeDtos.Count > 0)
                    {
                        await _mediaRuleTypeRepo.BulkUpdateAsync(
                            updateRuleTypeDtos,
                            _uow.Connection, _uow.Transaction, cancellationToken);
                    }
                }

                if (hasInsertRules)
                {
                    var insertFormatIds = dto.InsertMediaRules!
                        .Select(r => r.IssueMediaFormatId).Distinct().ToList();

                    var checkExisting = await _mediaRuleRepo.IsBulkExistAsync(
                        sysIssueTypeId, insertFormatIds,
                        _uow.Connection, _uow.Transaction, cancellationToken);

                    if (!checkExisting.IdNotFound && !checkExisting.IsInactive)
                    {
                        var existing = await _mediaRuleRepo.GetByIssueTypeAndFormatsAsync(
                            sysIssueTypeId, insertFormatIds,
                            _uow.Connection, _uow.Transaction, cancellationToken);

                        if (existing.Any())
                        {
                            await _uow.RollbackAsync();
                            return ApiResponse<SysIssueTypeDTO>.Fail(
                                Messages.ActionsAreAlreadyLinked,
                                StatusCodes.Status409Conflict,
                                ErrorCodes.ActionsAreAlreadyLinked);
                        }
                    }

                    await _mediaRuleRepo.BulkInsertAsync(
                        sysIssueTypeId, dto.InsertMediaRules!, loginId,
                        _uow.Connection, _uow.Transaction, cancellationToken);

                    var createdRules = (await _mediaRuleRepo.GetByIssueTypeAndFormatsAsync(
                        sysIssueTypeId, insertFormatIds,
                        _uow.Connection, _uow.Transaction, cancellationToken)).ToList();

                    var createdRuleMap = createdRules.ToDictionary(
                        r => r.IssueMediaFormatId, r => r.SysIssueMediaRuleId);

                    var insertTypeDtos = new List<SysIssueMediaRuleType>();
                    var allInsertTypeIds = new List<int>();
                    foreach (var ruleDto in dto.InsertMediaRules)
                    {
                        if (ruleDto.MediaTypes == null) continue;
                        var ruleId = createdRuleMap[ruleDto.IssueMediaFormatId];
                        foreach (var mt in ruleDto.MediaTypes)
                        {
                            insertTypeDtos.Add(new SysIssueMediaRuleType
                            {
                                IssueMediaRuleId = ruleId,
                                IssueMediaTypeId = mt.IssueMediaTypeId,
                                IsMandatory = mt.IsMandatory,
                                CreatedBy = loginId
                            });
                            allInsertTypeIds.Add(mt.IssueMediaTypeId);
                        }
                    }

                    if (allInsertTypeIds.Count > 0)
                    {
                        var checkTypes = await _mediaTypeRepo.IsBulkIdsExistAsync(
                            allInsertTypeIds.Distinct(),
                            _uow.Connection, _uow.Transaction, cancellationToken);

                        if (checkTypes.IdNotFound)
                        {
                            await _uow.RollbackAsync();
                            return ApiResponse<SysIssueTypeDTO>.Fail(
                                Messages.NotFound,
                                StatusCodes.Status404NotFound,
                                ErrorCodes.NotFound);
                        }
                        if (checkTypes.IsInactive)
                        {
                            await _uow.RollbackAsync();
                            return ApiResponse<SysIssueTypeDTO>.Fail(
                                Messages.ProgramActionNotFound,
                                StatusCodes.Status409Conflict,
                                ErrorCodes.ProgramActionNotFound);
                        }
                    }

                    if (insertTypeDtos.Count > 0)
                    {
                        await _mediaRuleTypeRepo.BulkInsertAsync(
                            insertTypeDtos,
                            _uow.Connection, _uow.Transaction, cancellationToken);
                    }
                }

                await _uow.CommitAsync();

                var updatedEntity = await _repo.GetByIdAsync(sysIssueTypeId, cancellationToken);
                return ApiResponse<SysIssueTypeDTO>.Ok(
                    updatedEntity,
                    Messages.IssueTypeUpdated,
                    statusCode: StatusCodes.Status200OK);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }


        // ───────────────────────── RECOVER / DELETE ─────────────────────────

        public async Task<ApiResponse<SysIssueTypeDTO>> RecoverIssueTypeAsync(
            int id,
            int loginId,
            CancellationToken cancellationToken = default)
        {
            var existDto = await _repo.IsIdExistAsync(id, cancellationToken);

            if (existDto == null)
            {
                return ApiResponse<SysIssueTypeDTO>.Fail(
                    Messages.IssueTypeNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.IssueTypeNotFoundById);
            }

            if (!existDto.Deleted)
            {
                return ApiResponse<SysIssueTypeDTO>.Fail(
                    Messages.IssueTypeAlreadyRecovered,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.IssueTypeAlreadyRecovered);
            }

            var rowAffect = await _repo.RecoverIssueTypeAsync(id, loginId, cancellationToken);
            if (!rowAffect)
            {
                return ApiResponse<SysIssueTypeDTO>.Fail(
                    Messages.IssueTypeAlreadyRecovered,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.IssueTypeAlreadyRecovered);
            }

            var dto = await _repo.GetByIdAsync(id, cancellationToken);
            return ApiResponse<SysIssueTypeDTO>.Ok(
                dto, Messages.IssueTypeRecovered, statusCode: StatusCodes.Status200OK);
        }


        public async Task<ApiResponse<SysIssueTypeDTO>> DeleteAsync(
            int id,
            int loginId,
            CancellationToken cancellationToken = default)
        {
            var existDto = await _repo.IsIdExistAsync(id, cancellationToken);

            if (existDto == null)
            {
                return ApiResponse<SysIssueTypeDTO>.Fail(
                    Messages.IssueTypeNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.IssueTypeNotFoundById);
            }

            if (existDto.Deleted)
            {
                return ApiResponse<SysIssueTypeDTO>.Fail(
                    Messages.IssueTypeAlreadyDeleted,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.IssueTypeAlreadyDeleted);
            }

            var rowAffect = await _repo.DeleteAsync(id, loginId, cancellationToken);
            if (!rowAffect)
            {
                return ApiResponse<SysIssueTypeDTO>.Fail(
                    Messages.IssueTypeAlreadyDeleted,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.IssueTypeAlreadyDeleted);
            }

            var dto = await _repo.GetByIdAsync(id, cancellationToken);
            return ApiResponse<SysIssueTypeDTO>.Ok(
                dto, Messages.IssueTypeDeleted, statusCode: StatusCodes.Status200OK);
        }


        // ───────────────────────── HELPERS ─────────────────────────

        private static ApiResponse<SysIssueTypeDTO>? HandleMediaFormatValidation(
            BulkValidationResult result)
        {
            if (result.IdNotFound)
                return ApiResponse<SysIssueTypeDTO>.Fail(
                    Messages.IssueMediaFormatNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.IssueMediaFormatNotFoundById);
            if (result.IsDeleted)
                return ApiResponse<SysIssueTypeDTO>.Fail(
                    Messages.IssueMediaFormatNotFound,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.IssueMediaFormatNotFound);
            if (result.IsInactive)
                return ApiResponse<SysIssueTypeDTO>.Fail(
                    Messages.IssueMediaFormatNotActive,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.IssueMediaFormatNotActive);
            return null;
        }

        private static ApiResponse<SysIssueTypeDTO>? HandleMediaTypeValidation(
            BulkValidationResult result)
        {
            if (result.IdNotFound)
                return ApiResponse<SysIssueTypeDTO>.Fail(
                    Messages.IssueMediaTypeNotFoundById,
                    StatusCodes.Status404NotFound,
                    ErrorCodes.IssueMediaTypeNotFoundById);
            if (result.IsDeleted)
                return ApiResponse<SysIssueTypeDTO>.Fail(
                    Messages.IssueMediaTypeNotFound,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.IssueMediaTypeNotFound);
            if (result.IsInactive)
                return ApiResponse<SysIssueTypeDTO>.Fail(
                    Messages.IssueMediaTypeNotActive,
                    StatusCodes.Status409Conflict,
                    ErrorCodes.IssueMediaTypeNotActive);
            return null;
        }

        private static ApiResponse<SysIssueTypeDTO> InternalError()
            => ApiResponse<SysIssueTypeDTO>.Fail(
                Messages.SomethingWentWrong,
                StatusCodes.Status500InternalServerError,
                ErrorCodes.InternalServerError);
    }
}
