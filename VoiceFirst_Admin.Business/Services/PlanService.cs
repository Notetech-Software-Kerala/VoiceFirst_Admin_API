using System;
using System.Collections.Generic;
using System.Text;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.Plan;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Business.Services
{
    public class PlanService:IPlanService
    {
        private readonly IPlanRepo _planRepository;

        public PlanService(IPlanRepo planRepository)
        {
            _planRepository = planRepository;
        }

        public async Task<ApiResponse<IEnumerable<PlanActiveDto>>> 
            GetActiveAsync(CancellationToken cancellationToken = default)
        {
            var items = await _planRepository.GetActiveAsync(cancellationToken);
            return ApiResponse<IEnumerable<PlanActiveDto>>.
                Ok(items, Messages.PlanRetrieved);
        }

    }
}
