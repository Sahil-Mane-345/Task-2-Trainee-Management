using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TraineeApi.Utility.Exception;
namespace TraineeApi.Utility;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, System.Exception exception, CancellationToken cancellationToken)
    {

        ProblemDetails problemDetails = new()
        {
            Detail = exception.Message,
            Instance = httpContext.Request.Path,
        };
        
        var StatusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            InvalidValidationException => StatusCodes.Status400BadRequest,
            InvalidIdentifierException => StatusCodes.Status400BadRequest,
            InvalidFileValidationException => StatusCodes.Status400BadRequest,

            _ => StatusCodes.Status500InternalServerError
        };

        if(StatusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "An Unexpected Error Occurred");
        }

        problemDetails.Status = StatusCode;

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;
        problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
        
        httpContext.Response.StatusCode = StatusCode;
        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken
        );

        return true;
    }
}