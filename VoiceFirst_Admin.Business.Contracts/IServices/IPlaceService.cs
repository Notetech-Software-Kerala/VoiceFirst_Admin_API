using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.Place;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface IPlaceService
    {
        Task<ApiResponse<PlaceDetailDTO>> CreateAsync(
            PlaceCreateDTO dto, int loginId,
            CancellationToken cancellationToken = default);

        Task<ApiResponse<PlaceDetailDTO>?> GetByIdAsync(
            int id, CancellationToken cancellationToken = default);


        Task<ApiResponse<PagedResultDto<PlaceDTO>>> 
            GetAllAsync(PlaceFilterDTO filter, 
            CancellationToken cancellationToken = default);


        Task<ApiResponse<PlaceDTO>>
            UpdateAsync(
            PlaceUpdateDTO dto,
            int sysBusinessActivityId, int loginId,
            CancellationToken cancellationToken = default);


        Task<ApiResponse<PlaceDetailDTO>> DeleteAsync(int id, int loginId, 
            CancellationToken cancellationToken = default);


        Task<ApiResponse<List<PlaceLookUpDTO>>> 
            GetLookUpAsync(CancellationToken cancellationToken);


        Task<ApiResponse<PlaceDetailDTO>> RecoverAsync(
            int id,
            int loginId,
            CancellationToken cancellationToken = default);
    }
}
