using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysUserCustomField;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface ISysUserCustomFieldService
    {
        Task<ApiResponse<CustomFieldDetailDto>> CreateAsync(CustomFieldCreateDto dto, int loginId, CancellationToken cancellationToken = default);
        Task<CustomFieldDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResultDto<CustomFieldLookUpDto>?> GetLookUpAsync(BasicFilterDto filter, CancellationToken cancellationToken = default);
        Task<PagedResultDto<RuleLookUpDto>?> GetRuleLookUpAsync(BasicFilterDto filter, CancellationToken cancellationToken = default);
        Task<List<DataTypeLookUpDto>> GetDataTypeLookUpAsync( CancellationToken cancellationToken = default);
        Task<PagedResultDto<CustomFieldDto>> GetAllAsync(CustomFieldFilterDto filter, CancellationToken cancellationToken = default);
        Task<ApiResponse<CustomFieldDetailDto>> UpdateAsync(CustomFieldUpdateDto dto, int id, int loginId, CancellationToken cancellationToken = default);
        Task<ApiResponse<CustomFieldDetailDto>> DeleteAsync(int id, int loginId, CancellationToken cancellationToken = default);
    }
}
