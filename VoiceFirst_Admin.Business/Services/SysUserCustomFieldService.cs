using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Role;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class SysUserCustomFieldService : ISysUserCustomFieldService
    {
        private readonly ISysUserCustomFieldRepo _repo;
        private readonly IMapper _mapper;

        public SysUserCustomFieldService(ISysUserCustomFieldRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ApiResponse<CustomFieldDetailDto>> CreateAsync(CustomFieldCreateDto dto, int loginId, CancellationToken cancellationToken = default)
        {

            var exsitFieldKey = await _repo.ExistsByFieldKeyAsync(dto.FieldKey,null, cancellationToken);
            if (exsitFieldKey != null )
            {
                if(exsitFieldKey.IsDeleted==true)
                    return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldAlreadyExistsRecoverable,StatusCodes.Status409Conflict);
                else
                    return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldKeyAlreadyExists, StatusCodes.Status409Conflict);
            }
            var exsitFieldNameWithDataType = await _repo.ExistsByFieldNameAndDataTypeAsync(dto.FieldName,dto.FieldDataType, null, cancellationToken);
            if (exsitFieldNameWithDataType != null )
            {
                if(exsitFieldKey.IsDeleted==true)
                    return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldAlreadyExistsRecoverable,StatusCodes.Status409Conflict);
                else
                    return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldAlreadyExists, StatusCodes.Status409Conflict);
            }
            if (dto.AddOptions != null && dto.AddOptions.Count() > 0)
            {
                foreach (var o in dto.AddOptions)
                {
                    if (string.IsNullOrWhiteSpace(o.label) && o.value == null)
                    {
                        return new ApiResponse<CustomFieldDetailDto>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = Messages.CustomFieldOptionRequired
                        };
                    }
                    if (string.IsNullOrWhiteSpace(o.label))
                    {
                        return new ApiResponse<CustomFieldDetailDto>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = Messages.CustomFieldOptionLabelRequired
                        };
                    }

                    if (o.value == null)
                    {
                        return new ApiResponse<CustomFieldDetailDto>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = Messages.CustomFieldOptionValueRequired
                        };
                    }
                }
            }
            if (dto.AddValidations != null && dto.AddValidations.Count() > 0)
            {
                foreach (var v in dto.AddValidations)
                {
                    if (string.IsNullOrWhiteSpace(v.RuleName) &&
                        v.RuleValue == null &&
                        string.IsNullOrWhiteSpace(v.message))
                    {
                        return new ApiResponse<CustomFieldDetailDto>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = Messages.CustomFieldValidationRequired
                        };
                    }
                    if (string.IsNullOrWhiteSpace(v.RuleName))
                    {
                        return new ApiResponse<CustomFieldDetailDto>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = Messages.CustomFieldValidationRuleNameRequired
                        };
                    }

                    if (v.RuleValue == null)
                    {
                        return new ApiResponse<CustomFieldDetailDto>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = Messages.CustomFieldValidationRuleValueRequired
                        };
                    }

                    if (string.IsNullOrWhiteSpace(v.message))
                    {
                        return new ApiResponse<CustomFieldDetailDto>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = Messages.CustomFieldValidationMessageRequired
                        };
                    }
                }
            }
            
            var entity = _mapper.Map<SysUserCustomField>(dto);

            List<SysUserCustomFieldValidations> userCustomFieldValidations = _mapper.Map<List<SysUserCustomFieldValidations>>(dto.AddValidations);
                
            List<SysUserCustomFieldOptions> userCustomFieldOptions = _mapper.Map<List<SysUserCustomFieldOptions>>(dto.AddOptions);
                
                
            var upsertError = await _repo.CreateAsync(entity, userCustomFieldValidations, userCustomFieldOptions, loginId, cancellationToken);
            if (upsertError.Success==false )
                return ApiResponse<CustomFieldDetailDto>.Fail(upsertError.Message,upsertError.StatusCode);
            var userCustomField = await GetByIdAsync(upsertError.Id??0, cancellationToken);

            return ApiResponse<CustomFieldDetailDto>.Ok(userCustomField, Messages.CustomFieldCreated);
            
        }

        public async Task<CustomFieldDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var field = await _repo.GetByIdAsync(id, cancellationToken);
            if (field == null) return null;

            var validations = await _repo.GetValidationsByFieldIdAsync(id, cancellationToken);

            var options = await _repo.GetOptionsByFieldIdAsync(id, cancellationToken);
            var dto = _mapper.Map<CustomFieldDetailDto>(field);
            dto.Validations = _mapper.Map<List<CustomFieldValidationsDto>>(validations);
            dto.Options = _mapper.Map<List<CustomFieldOptionsDto>>(options);

            return dto;
        }

        public async Task<ApiResponse<CustomFieldDetailDto>> UpdateAsync(CustomFieldUpdateDto dto, int id, int loginId, CancellationToken cancellationToken = default)
        {
            var exsitFieldKey = await _repo.ExistsByFieldKeyAsync(dto.FieldKey, null, cancellationToken);
            if (exsitFieldKey != null)
            {
                if (exsitFieldKey.IsDeleted == true)
                    return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldAlreadyExistsRecoverable, StatusCodes.Status409Conflict);
                else
                    return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldKeyAlreadyExists, StatusCodes.Status409Conflict);
            }
            var exsitFieldNameWithDataType = await _repo.ExistsByFieldNameAndDataTypeAsync(dto.FieldName, dto.FieldDataType, null, cancellationToken);
            if (exsitFieldNameWithDataType != null)
            {
                if (exsitFieldKey.IsDeleted == true)
                    return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldAlreadyExistsRecoverable, StatusCodes.Status409Conflict);
                else
                    return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldAlreadyExists, StatusCodes.Status409Conflict);
            }
            if (dto.AddOptions != null && dto.AddOptions.Count() > 0)
            {
                foreach (var o in dto.AddOptions)
                {
                    if (string.IsNullOrWhiteSpace(o.label) && o.value == null)
                    {
                        return new ApiResponse<CustomFieldDetailDto>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = Messages.CustomFieldOptionRequired
                        };
                    }
                    if (string.IsNullOrWhiteSpace(o.label))
                    {
                        return new ApiResponse<CustomFieldDetailDto>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = Messages.CustomFieldOptionLabelRequired
                        };
                    }

                    if (o.value == null)
                    {
                        return new ApiResponse<CustomFieldDetailDto>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = Messages.CustomFieldOptionValueRequired
                        };
                    }
                }
            }
            if (dto.AddValidations != null && dto.AddValidations.Count() > 0)
            {
                foreach (var v in dto.AddValidations)
                {
                    if (string.IsNullOrWhiteSpace(v.RuleName) &&
                        v.RuleValue == null &&
                        string.IsNullOrWhiteSpace(v.message))
                    {
                        return new ApiResponse<CustomFieldDetailDto>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = Messages.CustomFieldValidationRequired
                        };
                    }
                    if (string.IsNullOrWhiteSpace(v.RuleName))
                    {
                        return new ApiResponse<CustomFieldDetailDto>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = Messages.CustomFieldValidationRuleNameRequired
                        };
                    }

                    if (v.RuleValue == null)
                    {
                        return new ApiResponse<CustomFieldDetailDto>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = Messages.CustomFieldValidationRuleValueRequired
                        };
                    }

                    if (string.IsNullOrWhiteSpace(v.message))
                    {
                        return new ApiResponse<CustomFieldDetailDto>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = Messages.CustomFieldValidationMessageRequired
                        };
                    }
                }
            }
            if (dto.UpdateValidations != null && dto.UpdateValidations.Count() > 0)
            {
                foreach (var item in dto.UpdateValidations)
                {
                    if (item.CustomFieldValidationId <= 0)
                    {
                        return new ApiResponse<CustomFieldDetailDto>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = Messages.CustomFieldValidationIdRequired
                        };
                    }
                    if (string.IsNullOrWhiteSpace(item.RuleName) && string.IsNullOrWhiteSpace(item.RuleValue) && string.IsNullOrWhiteSpace(item.message) && !item.Active.HasValue)
                    {
                        return new ApiResponse<CustomFieldDetailDto>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = Messages.CustomFieldValidationAtLeastRequired
                        };
                    }
                }
            }
            if (dto.UpdateOptions != null && dto.UpdateOptions.Count() > 0)
            {
                foreach (var item in dto.UpdateOptions)
                {
                    if (item.CustomFieldOptionsId <= 0)
                    {
                        return new ApiResponse<CustomFieldDetailDto>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = Messages.CustomFieldOptionIdRequired
                        };
                    }
                    if (string.IsNullOrWhiteSpace(item.value) && string.IsNullOrWhiteSpace(item.label) && !item.Active.HasValue)
                    {
                        return new ApiResponse<CustomFieldDetailDto>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = Messages.CustomFieldOptionAtLeastRequired
                        };
                    }
                }
            }
            var entity = _mapper.Map<SysUserCustomField>(dto);
            entity.SysUserCustomFieldId = id;
                
            List<SysUserCustomFieldValidations> userCustomFieldValidations = _mapper.Map<List<SysUserCustomFieldValidations>>(dto.AddValidations);

            List<SysUserCustomFieldOptions> userCustomFieldOptions = _mapper.Map<List<SysUserCustomFieldOptions>>(dto.AddOptions);
            List<SysUserCustomFieldValidations> updateCustomFieldValidations = _mapper.Map<List<SysUserCustomFieldValidations>>(dto.UpdateValidations);
            List<SysUserCustomFieldOptions> updateCustomFieldOptions = _mapper.Map<List<SysUserCustomFieldOptions>>(dto.UpdateOptions);
           
               
            var upsertError = await _repo.UpdateAsync(entity, userCustomFieldValidations, userCustomFieldOptions, updateCustomFieldValidations, updateCustomFieldOptions, loginId, cancellationToken);
            if (upsertError != null)
                return ApiResponse<CustomFieldDetailDto>.Fail(upsertError.Message, upsertError.StatusCode);

            var userCustomField = await GetByIdAsync(entity.SysUserCustomFieldId, cancellationToken);
            
            return ApiResponse<CustomFieldDetailDto>.Ok(userCustomField, Messages.CustomFieldUpdated);
           
              
            
        }

        public async Task<ApiResponse<CustomFieldDetailDto>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {
            var field = await _repo.GetByIdAsync(id, cancellationToken);
            if (field == null) 
                return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldNotFound, StatusCodes.Status404NotFound);

            if(field.IsDeleted == true)
                return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldAlreadyDeleted, StatusCodes.Status409Conflict);

            var ok = await _repo.SoftDeleteAsync(id, loginId, cancellationToken);
            if (!ok)
                    return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldDeletionFailed);
            var userCustomField = await GetByIdAsync(id, cancellationToken);
            return ApiResponse<CustomFieldDetailDto>.Ok(userCustomField, Messages.CustomFieldDeleted);
            
        }

        public async Task<PagedResultDto<CustomFieldDto>> GetAllAsync(CustomFieldFilterDto filter, CancellationToken cancellationToken = default)
        {
            var entities = await _repo.GetAllAsync(filter, cancellationToken);
            var list = _mapper.Map<IEnumerable<CustomFieldDto>>(entities.Items);
            // load actions for each? skip for performance
            return new PagedResultDto<CustomFieldDto>
            {
                Items = list,
                TotalCount = entities.TotalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.Limit
            };
        }
        public async Task<PagedResultDto<CustomFieldLookUpDto>?> GetLookUpAsync(BasicFilterDto filter, CancellationToken cancellationToken = default)
        {
            var entities = await _repo.GetLookUpAsync(filter, cancellationToken);
            var list = _mapper.Map<IEnumerable<CustomFieldLookUpDto>>(entities.Items);
            // load actions for each? skip for performance
            return new PagedResultDto<CustomFieldLookUpDto>
            {
                Items = list,
                TotalCount = entities.TotalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.Limit
            };
        }
    }
}
