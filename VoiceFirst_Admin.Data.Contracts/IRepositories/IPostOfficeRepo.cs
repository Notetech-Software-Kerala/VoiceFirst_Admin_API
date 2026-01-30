using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.PostOffice;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories;

public interface IPostOfficeRepo
{
    Task<Dictionary<string, bool>> IsBulkIdsExistAsync(
    List<int> postOfficeIds,
    CancellationToken cancellationToken = default);

    

    Task<PostOffice> CreateAsync(PostOffice entity, CancellationToken cancellationToken = default);
    Task<PostOffice?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PostOfficeZipCode?> GetZipCodeByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResultDto<PostOffice>> GetAllAsync(PostOfficeFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<PostOffice>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<PostOffice?> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(PostOffice entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(PostOffice entity, CancellationToken cancellationToken = default);
    Task<bool> RestoreAsync(PostOffice entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<PostOfficeZipCode>> GetZipCodesByPostOfficeIdAsync(int postOfficeId, CancellationToken cancellationToken = default);
    Task<BulkUpsertError?> BulkUpdateZipCodesAsync(int postOfficeId, IEnumerable<PostOfficeZipCode> zipCodes, CancellationToken cancellationToken = default);
    Task<BulkUpsertError?> BulkInsertZipCodesAsync(int postOfficeId, List<string> zipCodes,int loginId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PostOfficeZipCode>> GetAllZipCodesAsync(string SearchText, CancellationToken cancellationToken = default);
}
