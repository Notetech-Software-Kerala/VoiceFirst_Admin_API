using AutoMapper;
using Microsoft.AspNetCore.Http;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysIssueType;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class SysIssueTypeService : ISysIssueTypeService
    {
        private readonly ISysIssueTypeRepo _repo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        public SysIssueTypeService(
            ISysIssueTypeRepo repository,
            IMapper mapper,
            IUnitOfWork uow)
        {
            _repo = repository;
            _mapper = mapper;
            _uow = uow;
        }


        public async Task<ApiResponse<SysIssueTypeDTO>> CreateAsync(
          SysIssueTypeCreateDTO dto,
          int loginId,
          CancellationToken cancellationToken)
        {
            var existingEntity = await _repo.IssueTypeExistsAsync(
                dto.IssueType,
                null,
                cancellationToken);

            if (existingEntity != null)
            {
                if (!existingEntity.Deleted)
                {
                    return ApiResponse<SysIssueTypeDTO>.Fail
                       (Messages.IssueTypeAlreadyExists,
                       StatusCodes.Status409Conflict,
                       ErrorCodes.IssueTypeAlreadyExists
                       );
                }
                return ApiResponse<SysIssueTypeDTO>.Fail(
                     Messages.IssueTypeAlreadyExistsRecoverable,
                     StatusCodes.Status422UnprocessableEntity,
                      ErrorCodes.IssueTypeAlreadyExistsRecoverable,
                      new SysIssueTypeDTO
                      {
                          IssueTypeId = existingEntity.IssueTypeId
                      }
                 );
            }

            var hasMediaRules = dto.MediaRules != null && dto.MediaRules.Count > 0;

            if (hasMediaRules)
            {
                await _uow.BeginAsync();
                try
                {
                    var entity = _mapper.Map<SysIssueType>(dto);
                    entity.CreatedBy = loginId;

                    entity.SysIssueTypeId = await _repo.CreateAsync(
                        entity, _uow.Connection, _uow.Transaction, cancellationToken);

                    if (entity.SysIssueTypeId <= 0)
                    {
                        await _uow.RollbackAsync();
                        return ApiResponse<SysIssueTypeDTO>.Fail(
                            Messages.SomethingWentWrong,
                            StatusCodes.Status500InternalServerError,
                            ErrorCodes.InternalServerError);
                    }

                    foreach (var ruleDto in dto.MediaRules!)
                    {
                        var rule = new SysIssueMediaRule
                        {
                            IssueTypeId = entity.SysIssueTypeId,
                            IssueMediaFormatId = ruleDto.IssueMediaFormatId,
                            Min = ruleDto.Min,
                            Max = ruleDto.Max,
                            MaxSizeMB = ruleDto.MaxSizeMB,
                            CreatedBy = loginId
                        };

                        var ruleId = await _repo.CreateMediaRuleAsync(
                            rule, _uow.Connection, _uow.Transaction, cancellationToken);

                        if (ruleDto.MediaTypes != null && ruleDto.MediaTypes.Count > 0)
                        {
                            await _repo.BulkInsertMediaRuleTypesAsync(
                                ruleId, ruleDto.MediaTypes, loginId,
                                _uow.Connection, _uow.Transaction, cancellationToken);
                        }
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
            else
            {
                var entity = _mapper.Map<SysIssueType>(dto);
                entity.CreatedBy = loginId;

                entity.SysIssueTypeId =
                    await _repo.CreateAsync(entity, cancellationToken);

                if (entity.SysIssueTypeId <= 0)
                {
                    return ApiResponse<SysIssueTypeDTO>.Fail(
                        Messages.SomethingWentWrong,
                        StatusCodes.Status500InternalServerError,
                        ErrorCodes.InternalServerError);
                }

                var createdDto = await _repo.GetByIdAsync(entity.SysIssueTypeId, cancellationToken);
                return ApiResponse<SysIssueTypeDTO>.Ok(
                    createdDto,
                    Messages.IssueTypeCreated,
                    StatusCodes.Status201Created);
            }
        }


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
            filter.Limit = Math.Min(filter.Limit, 60);
            var result = await _repo.GetAllAsync(filter, cancellationToken);

            return ApiResponse<PagedResultDto<SysIssueTypeDTO>>.Ok(
                result,
                result.TotalCount == 0
                    ? Messages.IssueTypesNotFound
                    : Messages.IssueTypesRetrieved,
                 statusCode: StatusCodes.Status200OK
            );
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
                statusCode: StatusCodes.Status200OK
            );
        }


        public async Task<ApiResponse<SysIssueTypeDTO>> UpdateAsync(
          SysIssueTypeUpdateDTO dto,
          int sysIssueTypeId,
          int loginId,
          CancellationToken cancellationToken = default)
        {
            var existDto =
                await _repo.IsIdExistAsync(sysIssueTypeId,
                cancellationToken);

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
               dto.IssueType,
               sysIssueTypeId,
               cancellationToken);

                if (existingEntity is not null)
                {
                    if (!existingEntity.Deleted)
                    {
                        return ApiResponse<SysIssueTypeDTO>.Fail
                           (Messages.IssueTypeAlreadyExists,
                           StatusCodes.Status409Conflict,
                           ErrorCodes.IssueTypeAlreadyExists
                           );
                    }
                    return ApiResponse<SysIssueTypeDTO>.Fail(
                         Messages.IssueTypeAlreadyExistsRecoverable,
                         StatusCodes.Status422UnprocessableEntity,
                          ErrorCodes.IssueTypeAlreadyExistsRecoverable,
                          new SysIssueTypeDTO
                          {
                              IssueTypeId = existingEntity.IssueTypeId
                          }
                     );
                }
            }

            // If no media rule updates/inserts provided, perform a simple update
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
                    return ApiResponse<SysIssueTypeDTO>.Fail(
                        Messages.IssueTypeUpdated,
                        StatusCodes.Status204NoContent,
                        ErrorCodes.NoRowAffected);

                var updatedEntity = await _repo.GetByIdAsync(sysIssueTypeId, cancellationToken);

                return ApiResponse<SysIssueTypeDTO>.Ok(
                    updatedEntity,
                    Messages.IssueTypeUpdated,
                    statusCode: StatusCodes.Status200OK);
            }

            // Perform transactional update/insert for media rules
            await _uow.BeginAsync();
            try
            {

                if (dto.Active != null && dto.IssueType != null)
                {

                    var entity = _mapper.Map<SysIssueType>((dto, sysIssueTypeId, loginId));
                    var updated = await _repo.UpdateAsync(entity, _uow.Connection, _uow.Transaction, cancellationToken);

                    if (!updated)
                    {
                        await _uow.RollbackAsync();
                        return ApiResponse<SysIssueTypeDTO>.Fail(
                            Messages.IssueTypeUpdated,
                            StatusCodes.Status204NoContent,
                            ErrorCodes.NoRowAffected);
                    }
                }
                // Bulk update existing rules and their types
                if (hasUpdateRules)
                {
                    var updateFormatIds = 
                        dto.UpdateMediaRules!.Select
                        (r => r.IssueMediaFormatId).Distinct().ToList();

                    // check rules existence and active state
                    var checkRules =
                        await _repo.IsBulkMediaRulesExistAsync
                        (sysIssueTypeId, 
                        updateFormatIds,
                        _uow.Connection,
                        _uow.Transaction,
                        cancellationToken);

                    if (checkRules["idNotFound"])
                    { 
                        await _uow.RollbackAsync(); 
                        return ApiResponse<SysIssueTypeDTO>.Fail
                            (
                            Messages.MediaFormatIsNotFound, 
                            StatusCodes.Status404NotFound,
                            ErrorCodes.MediaFormatNotLinked
                            );
                    }

                   var  updateMediaRuleEntities = 
                        _mapper.Map<List<SysIssueMediaRule>>(
                        dto.UpdateMediaRules.Select(r => (r, sysIssueTypeId, loginId))
                    );

                    // bulk update rules
                    await _repo.BulkUpdateMediaRulesAsync
                        (updateMediaRuleEntities,
                        _uow.Connection, 
                        _uow.Transaction, 
                        cancellationToken);


                    // prepare bulk update for media types
                    var allTypeIdsToCheck = dto.UpdateMediaRules.SelectMany(
                        r => r.MediaTypes ?? new List<IssueMediaRuleTypeUpdateDTO>()).
                        Select(mt => mt.IssueMediaTypeId).Distinct().ToList();

                    if (allTypeIdsToCheck.Any())
                    {
                        var checkTypes = 
                            await _repo.IsBulkMediaTypesExistAsync
                            (allTypeIdsToCheck,
                            _uow.Connection,
                            _uow.Transaction,
                            cancellationToken);

                        if (checkTypes["idNotFound"]) 
                        { 
                            await _uow.RollbackAsync(); 
                            return ApiResponse<SysIssueTypeDTO>.Fail
                                (Messages.MediaTypeIsNotFound, 
                                StatusCodes.Status404NotFound,
                                ErrorCodes.MediaTypeNotLinked); 
                        }

                    }

                    // resolve rule ids by format
                    var existingRules = (
                        await _repo.GetRulesByIssueTypeAndFormatsAsync
                        (sysIssueTypeId,
                        updateFormatIds,
                        _uow.Connection, 
                        _uow.Transaction, 
                        cancellationToken)).ToList();

                    var ruleMap = 
                        existingRules.ToDictionary
                        (r => r.IssueMediaFormatId,
                        r => r.SysIssueMediaRuleId);

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

                    if (updateRuleTypeDtos.Any())
                    {
                        await _repo.BulkUpdateMediaRuleTypesAsync(updateRuleTypeDtos, _uow.Connection, _uow.Transaction, cancellationToken);
                    }
                }

                // Bulk insert new rules and their types
                if (hasInsertRules)
                {
                    var insertFormatIds = dto.InsertMediaRules!.Select(r => r.IssueMediaFormatId).Distinct().ToList();

                    // ensure none of these formats already linked
                    var checkExisting = await _repo.IsBulkMediaRulesExistAsync(sysIssueTypeId, insertFormatIds, _uow.Connection, _uow.Transaction, cancellationToken);
                    if (checkExisting["idNotFound"] == false && checkExisting["inActive"] == false && checkExisting.Count > 0)
                    {
                        // if any existing found, consider as conflict
                        var existing = await _repo.GetRulesByIssueTypeAndFormatsAsync(sysIssueTypeId, insertFormatIds, _uow.Connection, _uow.Transaction, cancellationToken);
                        if (existing.Any()) { await _uow.RollbackAsync(); return ApiResponse<SysIssueTypeDTO>.Fail(Messages.ActionsAreAlreadyLinked, StatusCodes.Status409Conflict, ErrorCodes.ActionsAreAlreadyLinked); }
                    }

                    // bulk insert rules
                    await _repo.BulkInsertMediaRulesAsync(sysIssueTypeId, dto.InsertMediaRules!, loginId, _uow.Connection, _uow.Transaction, cancellationToken);

                    // resolve newly created rules to get their ids
                    var createdRules = (await _repo.GetRulesByIssueTypeAndFormatsAsync(sysIssueTypeId, insertFormatIds, _uow.Connection, _uow.Transaction, cancellationToken)).ToList();
                    var createdRuleMap = createdRules.ToDictionary(r => r.IssueMediaFormatId, r => r.SysIssueMediaRuleId);

                    // prepare media-type inserts
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

                    if (allInsertTypeIds.Any())
                    {
                        var checkTypes = await _repo.IsBulkMediaTypesExistAsync(allInsertTypeIds.Distinct(), _uow.Connection, _uow.Transaction, cancellationToken);
                        if (checkTypes["idNotFound"]) { await _uow.RollbackAsync(); return ApiResponse<SysIssueTypeDTO>.Fail(Messages.NotFound, StatusCodes.Status404NotFound, ErrorCodes.NotFound); }
                        if (checkTypes["inActive"]) { await _uow.RollbackAsync(); return ApiResponse<SysIssueTypeDTO>.Fail(Messages.ProgramActionNotFound, StatusCodes.Status409Conflict, ErrorCodes.ProgramActionNotFound); }
                    }

                    if (insertTypeDtos.Any())
                    {
                        await _repo.BulkInsertMediaRuleTypesAsync(insertTypeDtos, _uow.Connection, _uow.Transaction, cancellationToken);
                    }
                }

                await _uow.CommitAsync();

                var updatedEntity = await _repo.GetByIdAsync(sysIssueTypeId, cancellationToken);
                return ApiResponse<SysIssueTypeDTO>.Ok(updatedEntity, Messages.IssueTypeUpdated, statusCode: StatusCodes.Status200OK);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }



        public async Task<ApiResponse<SysIssueTypeDTO>>
            RecoverIssueTypeAsync(
            int id,
            int loginId,
            CancellationToken cancellationToken = default)
        {
            var existDto =
              await _repo.IsIdExistAsync(id,
              cancellationToken);

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

            var rowAffect = await _repo.RecoverIssueTypeAsync
                (id, loginId, cancellationToken);
            if (!rowAffect)
            {
                return ApiResponse<SysIssueTypeDTO>.Fail(
                     Messages.IssueTypeAlreadyRecovered,
                     StatusCodes.Status409Conflict,
                     ErrorCodes.IssueTypeAlreadyRecovered);
            }

            var dto =
               await _repo.GetByIdAsync
               (id,
               cancellationToken);
            return ApiResponse<SysIssueTypeDTO>.
               Ok(dto, Messages.IssueTypeRecovered, statusCode: StatusCodes.Status200OK);
        }


        public async Task<ApiResponse<SysIssueTypeDTO>>
            DeleteAsync(
             int id,
             int loginId,
             CancellationToken cancellationToken = default)
        {
            var existDto =
            await _repo.IsIdExistAsync(id,
            cancellationToken);

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

            var rowAffect = await _repo.DeleteAsync
                (id, loginId, cancellationToken);
            if (!rowAffect)
            {
                return ApiResponse<SysIssueTypeDTO>.Fail(
                     Messages.IssueTypeAlreadyDeleted,
                     StatusCodes.Status409Conflict,
                     ErrorCodes.IssueTypeAlreadyDeleted);
            }

            var dto =
               await _repo.GetByIdAsync
               (id,
               cancellationToken);
            return ApiResponse<SysIssueTypeDTO>.
               Ok(dto, Messages.IssueTypeDeleted, statusCode: StatusCodes.Status200OK);
        }
    }
}
