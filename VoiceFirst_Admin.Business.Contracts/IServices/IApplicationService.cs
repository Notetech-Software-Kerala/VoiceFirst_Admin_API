using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Features.Application;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface IApplicationService
    {
        Task<ApiResponse<IEnumerable<ApplicationActiveDTO>>>
            GetActiveAsync(CancellationToken cancellationToken);
    }
}
