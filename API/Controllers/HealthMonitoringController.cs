using Microsoft.AspNetCore.Mvc;

namespace aquilosaurios_backend_core.API.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthMonitoringController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHealth()
        {
            return Ok("Healthy");
        }
    }
}
