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
        Task<BulkUpsertError?> CreateAsync(SysUserCustomField entity, List<SysUserCustomFieldValidations> validations, List<SysUserCustomFieldOptions> options, int createdBy, CancellationToken cancellationToken = default);
        Task<SysUserCustomField> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<SysUserCustomFieldValidations>> GetValidationsByFieldIdAsync(int fieldId, CancellationToken cancellationToken = default);
        Task<IEnumerable<SysUserCustomFieldOptions>> GetOptionsByFieldIdAsync(int fieldId, CancellationToken cancellationToken = default);
        Task<PagedResultDto<SysUserCustomField>> GetAllAsync(SysUserCustomFieldFilterDto filter, CancellationToken cancellationToken = default);
        Task<BulkUpsertError?> UpdateAsync(SysUserCustomField entity, List<SysUserCustomFieldValidations> addValidations, List<SysUserCustomFieldOptions> addOptions, IEnumerable<SysUserCustomFieldValidations> validations, IEnumerable<SysUserCustomFieldOptions> options, int updatedBy, CancellationToken cancellationToken = default);
        Task<bool> SoftDeleteAsync(int id, int deletedBy, CancellationToken cancellationToken = default);
    }
}
