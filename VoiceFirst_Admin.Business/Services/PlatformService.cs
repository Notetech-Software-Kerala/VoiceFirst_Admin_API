using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Application;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Services
{
    public class PlatformService : IPlatformService
    {
        private readonly IApplicationRepository _applicationRepository;

        public PlatformService(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository
                ?? throw new ArgumentNullException(nameof(applicationRepository));
        }

        public async Task<ApiResponse<IEnumerable<PlatformLookupDto>>>
            GetActivePlatformsAsync(CancellationToken cancellationToken)
        {
            var platforms = await _applicationRepository
                .GetActiveApplicationsAsync(cancellationToken);

            if (platforms == null || !platforms.Any())
            {
                return ApiResponse<IEnumerable<PlatformLookupDto>>.Ok(
                    Enumerable.Empty<PlatformLookupDto>(),
                    Messages.NoPlatformsFound);
            }

            return ApiResponse<IEnumerable<PlatformLookupDto>>.Ok(
                platforms,
                Messages.PlatformsRetrieved);
        }
    }
}
