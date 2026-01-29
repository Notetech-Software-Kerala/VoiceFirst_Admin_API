using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.Place;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface IPlaceRepo
    {
        Task<PlaceDTO> PlaceExistsAsync
            (string name,
            int? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<int> CreateAsync
            (Place entity,
            CancellationToken cancellationToken = default);

        Task<PlaceDTO?> GetByIdAsync(
            int PlaceId, 
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<PlaceDTO>>
        GetAllAsync(PlaceFilterDTO filter,
            CancellationToken cancellationToken = default);


        Task<bool> DeleteAsync
            (int id, int deletedBy,
            CancellationToken cancellationToken = default);

        Task<List<PlaceLookUpDTO?>> 
            GetActiveAsync(CancellationToken cancellationToken = default);

        Task<bool> RecoverAsync
            (int id, int loginId, 
            CancellationToken cancellationToken = default);
        Task<PlaceDTO> IsIdExistAsync(
          int placeId,
          CancellationToken cancellationToken = default);

        Task<bool> 
            UpdateAsync(Place entity, 
            CancellationToken cancellationToken = default);


    }
}
