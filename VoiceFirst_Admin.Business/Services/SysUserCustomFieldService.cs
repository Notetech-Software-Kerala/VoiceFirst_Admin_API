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

            if (dto.AddCustomFieldDataType != null && dto.AddCustomFieldDataType.Count() > 0)
            {
                foreach (var dt in dto.AddCustomFieldDataType)
                {
                    var exsitFieldDataType = await _repo.ExistsByFieldDataTypeByIdAsync(dt.FieldDataTypeId, cancellationToken);
                    if (exsitFieldDataType == null)
                    {
                        return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldDataTypeNotExists, StatusCodes.Status404NotFound);
                    }
                    if (dt.AddOptions != null && dt.AddOptions.Count() > 0)
                    {
                        foreach (var o in dt.AddOptions)
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
                            if (!IsValidValueByDataType(dt.ValueDataType, o.value))
                            {
                                return new ApiResponse<CustomFieldDetailDto>
                                {
                                    StatusCode = StatusCodes.Status400BadRequest,
                                    Message = GetValueDataTypeErrorMessage(dt.ValueDataType)
                                };
                            }
                        }
                    }
                    if (dt.AddValidations != null && dt.AddValidations.Count() > 0)
                    {
                        foreach (var v in dt.AddValidations)
                        {
                            if (v.RuleId > 0 &&
                                v.RuleValue == null &&
                                string.IsNullOrWhiteSpace(v.Message))
                            {
                                return new ApiResponse<CustomFieldDetailDto>
                                {
                                    StatusCode = StatusCodes.Status400BadRequest,
                                    Message = Messages.CustomFieldValidationRequired
                                };
                            }
                            var exsitValidationRule = await _repo.ExistsByValidationRuleByIdAsync(v.RuleId, cancellationToken);
                            if (exsitValidationRule == null)
                            {
                                return ApiResponse<CustomFieldDetailDto>.Fail(Messages.ValidationRuleNotExists, StatusCodes.Status404NotFound);
                            }

                            if (v.RuleValue == null)
                            {
                                return new ApiResponse<CustomFieldDetailDto>
                                {
                                    StatusCode = StatusCodes.Status400BadRequest,
                                    Message = Messages.CustomFieldValidationRuleValueRequired
                                };
                            }

                            if (string.IsNullOrWhiteSpace(v.Message))
                            {
                                return new ApiResponse<CustomFieldDetailDto>
                                {
                                    StatusCode = StatusCodes.Status400BadRequest,
                                    Message = Messages.CustomFieldValidationMessageRequired
                                };
                            }
                           
                        }
                    }
                }

            }
            var entity = _mapper.Map<SysUserCustomField>(dto);
            

            //List<SysUserCustomFieldValidations> userCustomFieldValidations = _mapper.Map<List<SysUserCustomFieldValidations>>(dto.AddValidations);
                
            //List<SysUserCustomFieldOptions> userCustomFieldOptions = _mapper.Map<List<SysUserCustomFieldOptions>>(dto.AddOptions);
                
                
            var id = await _repo.CreateAsync(entity, dto.AddCustomFieldDataType, loginId, cancellationToken);
            if (id <= 0)
                return ApiResponse<CustomFieldDetailDto>.Fail(Messages.SomethingWentWrong);
            var userCustomField = await GetByIdAsync(id, cancellationToken);

            return ApiResponse<CustomFieldDetailDto>.Ok(userCustomField, Messages.CustomFieldCreated);
            
        }

        public async Task<CustomFieldDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var field = await _repo.GetByIdAsync(id, cancellationToken);
            if (field == null) return null;
            var dto = _mapper.Map<CustomFieldDetailDto>(field);
            var Fieldlink = await _repo.GetFieldDataTypeByFieldIdAsync(id, cancellationToken);
            if (Fieldlink != null)
            {
                List<CustomFieldDataTypeDetailsDto> customFieldDataTypes = new List<CustomFieldDataTypeDetailsDto>();  
                foreach (var f in Fieldlink)
                {
                    var customFieldDataTypeObj = _mapper.Map<CustomFieldDataTypeDetailsDto>(f);

                    var validations = await _repo.GetValidationsByFieldLinkIdAsync(f.SysUserCustomFieldDataTypeLinkId, cancellationToken);

                    var options = await _repo.GetOptionsByFieldLinkIdAsync(f.SysUserCustomFieldDataTypeLinkId, cancellationToken);

                    customFieldDataTypeObj.Validations = _mapper.Map<List<CustomFieldValidationsDto>>(validations);
                    customFieldDataTypeObj.Options = _mapper.Map<List<CustomFieldOptionsDto>>(options);
                    customFieldDataTypes.Add(customFieldDataTypeObj);
                }
                dto.FieldDataTypes = customFieldDataTypes;
            }
            

            return dto;
        }
        
        public async Task<ApiResponse<CustomFieldDetailDto>> UpdateAsync(CustomFieldUpdateDto dto, int id, int loginId, CancellationToken cancellationToken = default)
        {
            if(dto.FieldKey!=null )
            {
                var exsitFieldKey = await _repo.ExistsByFieldKeyAsync(dto.FieldKey, null, cancellationToken);
                if (exsitFieldKey != null)
                {
                    if (exsitFieldKey.IsDeleted == true)
                        return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldAlreadyExistsRecoverable, StatusCodes.Status409Conflict);
                    else
                        return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldKeyAlreadyExists, StatusCodes.Status409Conflict);
                }
            }
            if(dto.FieldName!=null)
            {
                var exsitName = await _repo.ExistsByFieldNameAsync(dto.FieldName,  cancellationToken);
                if (exsitName != null)
                {
                    if (exsitName.IsDeleted == true)
                        return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomNameAlreadyExistsRecoverable, StatusCodes.Status409Conflict);
                    else
                        return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldNameAlreadyExists, StatusCodes.Status409Conflict);
                }
            }
            if(dto.addCustomFieldDataTypes!=null && dto.addCustomFieldDataTypes.Count() > 0)
            {
                foreach (var dt in dto.addCustomFieldDataTypes)
                {
                    var exsitFieldDataType = await _repo.ExistsByFieldDataTypeByIdAsync(dt.FieldDataTypeId, cancellationToken);
                    if (exsitFieldDataType == null)
                    {
                        return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldDataTypeNotExists, StatusCodes.Status404NotFound);
                    }
                    var exsitFieldNameWithDataType = await _repo.ExistsByFieldIdAndDataTypeIdIdAsync(id, dt.FieldDataTypeId, cancellationToken);
                    if (exsitFieldNameWithDataType != null)
                    {
                        return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomNameAndDataTypeAlreadyExistsRecoverable, StatusCodes.Status409Conflict);
                    }

                    if (dt.AddOptions != null && dt.AddOptions.Count() > 0)
                    {
                        foreach (var o in dt.AddOptions)
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
                    if (dt.AddValidations != null && dt.AddValidations.Count() > 0)
                    {
                        foreach (var v in dt.AddValidations)
                        {
                            if (v.RuleId > 0 &&
                                    v.RuleValue == null &&
                                    string.IsNullOrWhiteSpace(v.Message))
                            {
                                return new ApiResponse<CustomFieldDetailDto>
                                {
                                    StatusCode = StatusCodes.Status400BadRequest,
                                    Message = Messages.CustomFieldValidationRequired
                                };
                            }
                            var exsitValidationRule = await _repo.ExistsByValidationRuleByIdAsync(v.RuleId, cancellationToken);
                            if (exsitValidationRule == null)
                            {
                                return ApiResponse<CustomFieldDetailDto>.Fail(Messages.ValidationRuleNotExists, StatusCodes.Status404NotFound);
                            }


                            if (v.RuleValue == null)
                            {
                                return new ApiResponse<CustomFieldDetailDto>
                                {
                                    StatusCode = StatusCodes.Status400BadRequest,
                                    Message = Messages.CustomFieldValidationRuleValueRequired
                                };
                            }

                            if (string.IsNullOrWhiteSpace(v.Message))
                            {
                                return new ApiResponse<CustomFieldDetailDto>
                                {
                                    StatusCode = StatusCodes.Status400BadRequest,
                                    Message = Messages.CustomFieldValidationMessageRequired
                                };
                            }
                        }
                    }

                }
            }
            if (dto.UpdateCustomFieldDataTypes != null && dto.UpdateCustomFieldDataTypes.Count() > 0)
            {
                foreach (var dt in dto.UpdateCustomFieldDataTypes)
                {
                    if (dt.AddOptions != null && dt.AddOptions.Count() > 0)
                    {
                        foreach (var o in dt.AddOptions)
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

                            if (!IsValidValueByDataType(dt.ValueDataType, o.value))
                            {
                                return new ApiResponse<CustomFieldDetailDto>
                                {
                                    StatusCode = StatusCodes.Status400BadRequest,
                                    Message = GetValueDataTypeErrorMessage(dt.ValueDataType)
                                };
                            }
                        }
                    }
                    if (dt.AddValidations != null && dt.AddValidations.Count() > 0)
                    {
                        foreach (var v in dt.AddValidations)
                        {
                            if (v.RuleId > 0 &&
                                    v.RuleValue == null &&
                                    string.IsNullOrWhiteSpace(v.Message))
                            {
                                return new ApiResponse<CustomFieldDetailDto>
                                {
                                    StatusCode = StatusCodes.Status400BadRequest,
                                    Message = Messages.CustomFieldValidationRequired
                                };
                            }
                            var exsitValidationRule = await _repo.ExistsByValidationRuleByIdAsync(v.RuleId, cancellationToken);
                            if (exsitValidationRule == null)
                            {
                                return ApiResponse<CustomFieldDetailDto>.Fail(Messages.ValidationRuleNotExists, StatusCodes.Status404NotFound);
                            }

                            if (v.RuleValue == null)
                            {
                                return new ApiResponse<CustomFieldDetailDto>
                                {
                                    StatusCode = StatusCodes.Status400BadRequest,
                                    Message = Messages.CustomFieldValidationRuleValueRequired
                                };
                            }

                            if (string.IsNullOrWhiteSpace(v.Message))
                            {
                                return new ApiResponse<CustomFieldDetailDto>
                                {
                                    StatusCode = StatusCodes.Status400BadRequest,
                                    Message = Messages.CustomFieldValidationMessageRequired
                                };
                            }
                        }
                    }
                    if (dt.UpdateValidations != null && dt.UpdateValidations.Count() > 0)
                    {
                        foreach (var item in dt.UpdateValidations)
                        {
                            if (item.CustomFieldValidationId <= 0)
                            {
                                return new ApiResponse<CustomFieldDetailDto>
                                {
                                    StatusCode = StatusCodes.Status400BadRequest,
                                    Message = Messages.CustomFieldValidationIdRequired
                                };
                            }
                            if ( string.IsNullOrWhiteSpace(item.RuleValue) && string.IsNullOrWhiteSpace(item.message) && !item.Active.HasValue)
                            {
                                return new ApiResponse<CustomFieldDetailDto>
                                {
                                    StatusCode = StatusCodes.Status400BadRequest,
                                    Message = Messages.CustomFieldValidationAtLeastRequired
                                };
                            }
                        }
                    }
                    if (dt.UpdateOptions != null && dt.UpdateOptions.Count() > 0)
                    {
                        foreach (var item in dt.UpdateOptions)
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
                }
            }

                    
            var entity = _mapper.Map<SysUserCustomField>(dto);
            entity.SysUserCustomFieldId = id;
            entity.UpdatedBy = loginId;
                
            //List<SysUserCustomFieldValidations> userCustomFieldValidations = _mapper.Map<List<SysUserCustomFieldValidations>>(dto.AddValidations);

            //List<SysUserCustomFieldOptions> userCustomFieldOptions = _mapper.Map<List<SysUserCustomFieldOptions>>(dto.AddOptions);
            //List<SysUserCustomFieldValidations> updateCustomFieldValidations = _mapper.Map<List<SysUserCustomFieldValidations>>(dto.UpdateValidations);
            //List<SysUserCustomFieldOptions> updateCustomFieldOptions = _mapper.Map<List<SysUserCustomFieldOptions>>(dto.UpdateOptions);
           
               
            var status = await _repo.UpdateAsync(entity, dto.UpdateCustomFieldDataTypes, dto.addCustomFieldDataTypes, loginId, cancellationToken);
            if (status==false)
                return ApiResponse<CustomFieldDetailDto>.Fail(Messages.SomethingWentWrong);

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
        public async Task<ApiResponse<CustomFieldDetailDto>> RestoreAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {
            var field = await _repo.GetByIdAsync(id, cancellationToken);
            if (field == null) 
                return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldNotFound, StatusCodes.Status404NotFound);

            if(field.IsDeleted == false)
                return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldAlreadyRecovered, StatusCodes.Status409Conflict);

            var ok = await _repo.RestoreAsync(id, loginId, cancellationToken);
            if (!ok)
                    return ApiResponse<CustomFieldDetailDto>.Fail(Messages.CustomFieldRestoreFailed);
            var userCustomField = await GetByIdAsync(id, cancellationToken);
            return ApiResponse<CustomFieldDetailDto>.Ok(userCustomField, Messages.CustomFieldRestored);
            
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
        public async Task<PagedResultDto<RuleLookUpDto>?> GetRuleLookUpAsync(ValidationRuleFilterDto filter, CancellationToken cancellationToken = default)
        {
            
            var entities = await _repo.ValidationRuleLookupAsync(filter, cancellationToken);
            var list = _mapper.Map<IEnumerable<RuleLookUpDto>>(entities.Items);
            // load actions for each? skip for performance
            return new PagedResultDto<RuleLookUpDto>
            {
                Items = list,
                TotalCount = entities.TotalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.Limit
            };
        }
        public async Task<List<DataTypeLookUpDto>> GetDataTypeLookUpAsync( CancellationToken cancellationToken = default)
        {
            var entities = await _repo.FieldDataTypeLookupAsync( cancellationToken);
            var list = _mapper.Map<IEnumerable<DataTypeLookUpDto>>(entities);
            // load actions for each? skip for performance
            return list.ToList();
        }

        // private helper methods

        private static bool IsValidValueByDataType(ValueDataType? dataType, object? value)
        {
            if (!dataType.HasValue || value == null)
                return false;

            var stringValue = value.ToString()?.Trim();

            if (string.IsNullOrWhiteSpace(stringValue))
                return false;

            switch (dataType)
            {
                case ValueDataType.Varchar:
                case ValueDataType.NVarchar:
                    return true;

                case ValueDataType.Int:
                    return int.TryParse(stringValue, out _);

                case ValueDataType.Float:
                    return float.TryParse(stringValue, out _);

                case ValueDataType.Decimal:
                    return decimal.TryParse(stringValue, out _);

                case ValueDataType.Bit:
                    return bool.TryParse(stringValue, out _) ||
                           stringValue == "0" ||
                           stringValue == "1";

                case ValueDataType.DateTime:
                    return DateTime.TryParse(stringValue, out _);

                case ValueDataType.Date:
                    return DateOnly.TryParse(stringValue, out _);
                // If your project does not support DateOnly, use:
                // return DateTime.TryParse(stringValue, out _);

                default:
                    return false;
            }
        }
        private static string GetValueDataTypeErrorMessage(ValueDataType? dataType)
        {
            if (!dataType.HasValue)
                return Messages.CustomFieldValueDataTypeRequired;

            return dataType.Value switch
            {
                ValueDataType.Varchar => Messages.CustomFieldOptionInvalidVarchar,
                ValueDataType.NVarchar => Messages.CustomFieldOptionInvalidNVarchar,
                ValueDataType.Int => Messages.CustomFieldOptionInvalidInt,
                ValueDataType.Float => Messages.CustomFieldOptionInvalidFloat,
                ValueDataType.Decimal => Messages.CustomFieldOptionInvalidDecimal,
                ValueDataType.Bit => Messages.CustomFieldOptionInvalidBit,
                ValueDataType.DateTime => Messages.CustomFieldOptionInvalidDateTime,
                ValueDataType.Date => Messages.CustomFieldOptionInvalidDate,
                _ => Messages.CustomFieldOptionInvalid
            };
        }
    }
}
