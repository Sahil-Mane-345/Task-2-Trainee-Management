using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TraineeApi.Utility;

public static class HealthCheckResponseWriter
{
    public static async Task WriteResponse(HttpContext httpContext, HealthReport healthReport)
    {
        httpContext.Response.ContentType = "application/json";

        var response = new
        {
            status = healthReport.Status.ToString(),
            timestamp = DateTime.UtcNow,
            totalDuration = healthReport.TotalDuration.TotalMilliseconds,
            checks = healthReport.Entries.Select( x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                duration = x.Value.Duration.Milliseconds,
                exception = x.Value.Exception?.Message,
                tags = x.Value.Tags,
                data = x.Value.Data
            })
        };

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(response,
            new JsonSerializerOptions
            {
                WriteIndented = true
            })
        );
    }
}