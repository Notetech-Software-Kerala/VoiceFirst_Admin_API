using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceFirst_Admin.Utilities.DTOs.Features.SysProgram;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Contracts.IServices
{
    public interface ISysProgramService
    {
        Task<ApiResponse<SysProgramDto>> CreateAsync(SysProgramCreateDTO dto, int loginId, CancellationToken cancellationToken = default);
    }
}
