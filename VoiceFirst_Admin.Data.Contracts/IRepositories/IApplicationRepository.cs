using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.Application;
using VoiceFirst_Admin.Utilities.DTOs.Features.ApplicationVersion;
using VoiceFirst_Admin.Utilities.Enums;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface IApplicationRepository
    {

        Task<IEnumerable<PlatformLookupDto>> GetActiveApplicationsAsync(
            CancellationToken cancellationToken = default);
        Task<Application> IsIdExistAsync
         (int ApplicationId, CancellationToken cancellationToken = default);

        Task<PlatformVersionDto?> VersionExistsAsync(
            int applicationId, 
            string version,
            ClientType type, 
            CancellationToken cancellationToken = default
            );

        Task<int> CreateVersionAsync
            (ApplicationVersion entity, 
            CancellationToken cancellationToken = default);

        Task<PlatformVersionDto?> GetVersionByIdAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}
