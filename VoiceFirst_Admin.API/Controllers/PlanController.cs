using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VoiceFirst_Admin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanController : ControllerBase
    {
        private readonly VoiceFirst_Admin.Business.Contracts.IServices.IPlanService _planService;
        public PlanController(VoiceFirst_Admin.Business.Contracts.IServices.IPlanService planService)
        {
            _planService = planService;
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> GetActivePlans(CancellationToken cancellationToken)
        {
            var result = await _planService.GetActiveAsync(cancellationToken);
            return StatusCode(result.StatusCode,result);
        }
    }

}
