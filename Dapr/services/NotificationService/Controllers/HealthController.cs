using Microsoft.AspNetCore.Mvc;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new 
            { 
                status = "healthy",
                timestamp = DateTime.UtcNow,
                application = "NotificationService"
            });
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            return Ok(new { status = "ok" });
        }
    }
}
