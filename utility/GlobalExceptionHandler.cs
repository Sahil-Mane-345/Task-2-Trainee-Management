using Microsoft.AspNetCore.Diagnostics;

namespace TraineeApi.Utility;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        
        httpContext.Response.StatusCode = 500;
        await httpContext.Response.WriteAsJsonAsync(
            new
            {
                message = exception.Message,
                status = 500
            }
        );

        return true;
    }
}