using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace VoiceFirst_Admin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private static readonly Stopwatch _uptime = Stopwatch.StartNew();

        // GET: api/health
        // Returns basic service health and a simple outbound connectivity check.
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var status = "Healthy";
            var checks = new List<object>();

            // Simple outbound connectivity check to a reliable public endpoint.
            bool outboundOk;
            string outboundDetails;
            try
            {
                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
                var resp = await http.GetAsync("https://www.google.com", cancellationToken);
                outboundOk = resp.IsSuccessStatusCode;
                outboundDetails = $"StatusCode:{(int)resp.StatusCode}";
            }
            catch (Exception ex)
            {
                outboundOk = false;
                outboundDetails = ex.Message;
            }

            checks.Add(new { name = "outbound", ok = outboundOk, details = outboundDetails });
            if (!outboundOk)
            {
                status = "Degraded";
            }

            var result = new
            {
                status,
                uptime = _uptime.Elapsed.ToString(),
                checks
            };

            return Ok(result);
        }
    }
}
