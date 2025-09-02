using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OrderManagement.API.Controllers
{
    /// <summary>
    /// Health check endpoints
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        /// <summary>
        /// Get application health status
        /// </summary>
        /// <returns>Health status</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(503)]
        public async Task<ActionResult> GetHealth()
        {
            var report = await _healthCheckService.CheckHealthAsync();

            var response = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    exception = entry.Value.Exception?.Message,
                    duration = entry.Value.Duration.ToString()
                }).ToList(),
                totalDuration = report.TotalDuration.ToString()
            };

            return report.Status == HealthStatus.Healthy ? Ok(response) : StatusCode(503, response);
        }

        /// <summary>
        /// Simple health check for load balancers
        /// </summary>
        /// <returns>OK</returns>
        [HttpGet("ready")]
        [ProducesResponseType(200)]
        public ActionResult Ready()
        {
            return Ok(new { status = "Ready" });
        }

        /// <summary>
        /// Liveness probe for Kubernetes
        /// </summary>
        /// <returns>OK</returns>
        [HttpGet("live")]
        [ProducesResponseType(200)]
        public ActionResult Live()
        {
            return Ok(new { status = "Alive" });
        }
    }
}
