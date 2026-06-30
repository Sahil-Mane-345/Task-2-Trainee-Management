using Microsoft.AspNetCore.Mvc;
namespace TraineeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase{  
    [HttpGet(Name = "GetHealth")]
    public object Get()
    {
        DateTime timestamp = DateTime.Now;
        return new {
            status = "running",
            application = "Trainee Management API",
            timestamp = timestamp
        };
    }  
} 