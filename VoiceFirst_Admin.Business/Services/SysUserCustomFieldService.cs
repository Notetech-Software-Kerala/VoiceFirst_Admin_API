using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Services
{
    public class SysUserCustomFieldService : ISysUserCustomFieldService
    {
    private readonly ISysUserCustomFieldRepo _repo;

    public SysUserCustomFieldService(ISysUserCustomFieldRepo repo)
    {
        _repo = repo;
    }

        public async Task<ApiResponse<object>> CreateAsync(UserCustomFieldCreateDto dto, int loginId, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = new SysUserCustomField
                {
                    FieldName = dto.FieldName,
                    FieldKey = dto.FieldKey,
                    FieldDataType = dto.FieldDataType,
                };
                List<SysUserCustomFieldValidations> userCustomFieldValidations = new List<SysUserCustomFieldValidations>();
                foreach (var item in dto.AddValidations)
                {
                    var ValidationEntity = new SysUserCustomFieldValidations
                    {
                        RuleName = item.RuleName,
                        RuleValue = item.RuleValue,
                        message = item.message
                    };
                    userCustomFieldValidations.Add(ValidationEntity);
                }
                List<SysUserCustomFieldOptions> userCustomFieldOptions = new List<SysUserCustomFieldOptions>();
                foreach (var item in dto.AddOptions)
                {
                    var OptionEntity = new SysUserCustomFieldOptions
                    {
                        label = item.label,
                        value = item.value
                    };

                    userCustomFieldOptions.Add(OptionEntity);
                }
                
                var upsertError = await _repo.CreateAsync(entity, userCustomFieldValidations, userCustomFieldOptions, loginId, cancellationToken);
                if (upsertError != null)
                    return ApiResponse<object>.Fail(upsertError.Message,upsertError.StatuaCode);

                return ApiResponse<object>.Ok(null, "Custom field created.");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail("Something went wrong.", StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public async Task<SysUserCustomFieldDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var field = await _repo.GetByIdAsync(id, cancellationToken);
            if (field == null) return null;

            var dto = new SysUserCustomFieldDetailDto
            {
                CustomFieldId = field.SysUserCustomFieldId,
                FieldName = field.FieldName,
                FieldKey = field.FieldKey,
                FieldDataType = field.FieldDataType,
                Active = field.IsActive ?? false,
                Deleted = field.IsDeleted ?? false
            };

            return dto;
        }

        public async Task<ApiResponse<object>> UpdateAsync(SysUserCustomFieldUpdateDto dto, int id, int loginId, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = new SysUserCustomField
                {
                    SysUserCustomFieldId = id,
                    FieldName = dto.FieldName,
                    FieldKey = dto.FieldKey,
                    FieldDataType = dto.FieldDataType,
                    IsActive = dto.Active
                };
                List<SysUserCustomFieldValidations> userCustomFieldValidations = new List<SysUserCustomFieldValidations>();
                foreach (var item in dto.AddValidations)
                {
                    var ValidationEntity = new SysUserCustomFieldValidations
                    {
                        RuleName = item.RuleName,
                        RuleValue = item.RuleValue,
                        message = item.message
                    };
                    userCustomFieldValidations.Add(ValidationEntity);
                }
                List<SysUserCustomFieldOptions> userCustomFieldOptions = new List<SysUserCustomFieldOptions>();
                foreach (var item in dto.AddOptions)
                {
                    var OptionEntity = new SysUserCustomFieldOptions
                    {
                        label = item.label,
                        value = item.value
                    };

                    userCustomFieldOptions.Add(OptionEntity);
                }
                List<SysUserCustomFieldValidations> updateCustomFieldValidations = new List<SysUserCustomFieldValidations>();
                foreach (var item in dto.UpdateValidations)
                {
                    var ValidationEntity = new SysUserCustomFieldValidations
                    {
                        SysUserCustomFieldValidationId = item.CustomFieldValidationId,
                        RuleName = item.RuleName,
                        RuleValue = item.RuleValue,
                        message = item.message,
                        IsActive = item.Active
                    };
                    userCustomFieldValidations.Add(ValidationEntity);
                }
                List<SysUserCustomFieldOptions> updateCustomFieldOptions = new List<SysUserCustomFieldOptions>();
                foreach (var item in dto.UpdateOptions)
                {
                    var OptionEntity = new SysUserCustomFieldOptions
                    {
                        SysUserCustomFieldOptionsId = item.CustomFieldOptionsId,
                        label = item.label,
                        value = item.value,
                        IsActive = item.Active
                    };

                    userCustomFieldOptions.Add(OptionEntity);
                }
                var upsertError = await _repo.UpdateAsync(entity, userCustomFieldValidations, userCustomFieldOptions, updateCustomFieldValidations, updateCustomFieldOptions, loginId, cancellationToken);
                if (upsertError != null)
                    return ApiResponse<object>.Fail(upsertError.Message, upsertError.StatuaCode);

                return ApiResponse<object>.Ok(null, "Custom field updated.");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail("Something went wrong.", StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public async Task<ApiResponse<object>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default)
        {
            try
            {
                var ok = await _repo.SoftDeleteAsync(id, loginId, cancellationToken);
                if (!ok)
                    return ApiResponse<object>.Fail("Failed to delete custom field.");

                return ApiResponse<object>.Ok(null, "Custom field deleted.");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail("Something went wrong.", StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
