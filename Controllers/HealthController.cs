using Microsoft.AspNetCore.Mvc;
using System;
namespace TraineeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase{  
    [HttpGet(Name = "GetHealth")]
    public Object Get()
    {
        DateTime timestamp = DateTime.Now;
        return new {
            status = "running",
            application = "Trainee Management API",
            timestamp = timestamp
        };
    }  
} 