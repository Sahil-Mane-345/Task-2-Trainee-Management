using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TraineeApi.Utility;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {

        ProblemDetails problemDetails = new()
        {
            Detail = exception.Message,
            Status = StatusCodes.Status500InternalServerError,
            Instance = httpContext.Request.Path,

        };
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;
        
        httpContext.Response.StatusCode = 500;
        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken
        );

        return true;
    }
}