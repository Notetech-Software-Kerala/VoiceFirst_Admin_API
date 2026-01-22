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
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepo _repo;
        public ApplicationService(IApplicationRepo repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<IEnumerable<ApplicationActiveDTO>>> 
            GetActiveAsync(CancellationToken cancellationToken)
        {          
            var items = await _repo.GetActiveAsync(cancellationToken);
            return ApiResponse<IEnumerable<ApplicationActiveDTO>>.
                Ok(items, Messages.ApplicationRetrieved);
        }
    }
}
