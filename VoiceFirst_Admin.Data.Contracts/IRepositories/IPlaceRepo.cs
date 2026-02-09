using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.Place;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface IPlaceRepo
    {
        Task<bool>
            CheckPlaceZipCodeLinksExistAsync(
                        int placeId,
                IEnumerable<int> zipCodeLinkId,
                bool update,
                IDbConnection connection,
                IDbTransaction transaction,
                CancellationToken cancellationToken = default);

         //Task<bool>
         //   CheckPlacePostOfficeLinksExistAsync(
         //               int placeId,
         //       IEnumerable<int> postOfficeIds,
         //       IDbConnection connection,
         //       IDbTransaction transaction,
         //       CancellationToken cancellationToken = default);


        Task<bool> BulkUpdatePlaceZipCodeLinksAsync(
         int placeId,
         IEnumerable<PlaceZipCodeLinkUpdateDTO> dtos,
         int updatedBy,
         IDbConnection connection,
         IDbTransaction tx,
         CancellationToken cancellationToken);

        Task<PlaceDTO> PlaceExistsAsync
            (string name,
            int? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<bool> BulkInsertPlaceZipCodeLinksAsync(
         int programId,
         IEnumerable<int> zipCodeLinkIds,
         int createdBy,
         IDbConnection connection,
         IDbTransaction transaction,
         CancellationToken cancellationToken);

        Task<int> CreateAsync
            (Place entity,
            IDbConnection connection,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default);

        Task<PlaceDetailDTO?> GetByIdAsync(
            int PlaceId, 
            IDbConnection connection,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<PlaceZipCodeLinkDetailDTO>>
            GetPlaceZipCodeLinksByPlaceIdAsync(int placeId, IDbConnection connection,
            IDbTransaction transaction, CancellationToken cancellationToken = default);

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
            UpdateAsync(Place entity, IDbConnection connection,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default);


    }
}
