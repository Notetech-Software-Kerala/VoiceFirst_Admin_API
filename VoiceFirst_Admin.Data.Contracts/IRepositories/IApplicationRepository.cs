using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.Application;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface IApplicationRepository
    {
        
        Task<IEnumerable<PlatformLookupDto>> GetActiveApplicationsAsync(
            CancellationToken cancellationToken = default);
        Task<Application> IsIdExistAsync
         (int ApplicationId, CancellationToken cancellationToken = default);
    }
}
