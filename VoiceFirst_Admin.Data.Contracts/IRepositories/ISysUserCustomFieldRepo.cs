using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface ISysUserCustomFieldRepo
    {
        Task<int> CreateAsync(SysUserCustomField entity, List<CustomFieldDataTypeDto>? customFieldDataTypes,  int createdBy, CancellationToken cancellationToken = default);
        Task<SysUserCustomField> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<SysUserCustomFieldDataTypeLink> GetByLinkIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResultDto<SysUserCustomField>> GetLookUpAsync(BasicFilterDto filter, CancellationToken cancellationToken = default);
        Task<IEnumerable<SysUserCustomFieldDataTypeLink>> GetFieldDataTypeByFieldIdAsync(int fieldId, CancellationToken cancellationToken = default);
        Task<IEnumerable<SysUserCustomFieldValidations>> GetValidationsByFieldLinkIdAsync(int fieldLinkId, CancellationToken cancellationToken = default);
        Task<IEnumerable<SysUserCustomFieldOptions>> GetOptionsByFieldLinkIdAsync(int fieldLinkId, CancellationToken cancellationToken = default);
        Task<PagedResultDto<SysUserCustomField>> GetAllAsync(CustomFieldFilterDto filter, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(SysUserCustomField entity, List<UpdateCustomFieldDataTypeDto>? UpdateCustomFieldDataTypes, List<CustomFieldDataTypeDto>? addCustomFieldDataTypes, int updatedBy, CancellationToken cancellationToken = default);
        Task<bool> SoftDeleteAsync(int id, int deletedBy, CancellationToken cancellationToken = default);
        Task<bool> RestoreAsync(int id, int deletedBy, CancellationToken cancellationToken = default);
        Task<SysUserCustomField> ExistsByFieldKeyAsync(string fieldKey, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<SysUserCustomFieldDataType> ExistsByFieldDataTypeByIdAsync(int id,  CancellationToken cancellationToken = default);
        Task<IEnumerable<SysUserCustomFieldDataType>> FieldDataTypeLookupAsync(CancellationToken cancellationToken = default);
        Task<SysUserCustomFieldValidationsRule> ExistsByValidationRuleByIdAsync(int id,  CancellationToken cancellationToken = default);
        Task<List<string>> GetAllowedValueDataTypesByFieldDataTypeIdAsync(int fieldDataTypeId, CancellationToken cancellationToken);
        Task<PagedResultDto<SysUserCustomFieldValidationsRule>> ValidationRuleLookupAsync(ValidationRuleFilterDto filter, CancellationToken cancellationToken = default);
        Task<SysUserCustomFieldDataTypeLink> ExistsByFieldIdAndDataTypeIdIdAsync(int customerFieldId, int fieldDataTypeId, CancellationToken cancellationToken = default);
        Task<SysUserCustomField> ExistsByFieldNameAsync(string fieldName, CancellationToken cancellationToken = default);
    }
}
