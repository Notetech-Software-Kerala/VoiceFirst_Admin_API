using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Utilities.Models.Entities;

namespace VoiceFirst_Admin.Data.Contracts.IRepositories
{
    public interface IApplicationRepo
    {
        Task<Application> GetActiveByIdAsync
             (int ApplicationId, CancellationToken cancellationToken = default);
    }
}
