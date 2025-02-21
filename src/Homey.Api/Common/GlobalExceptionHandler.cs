using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Homey.Api.Common;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        
        var statusCode = StatusCodes.Status500InternalServerError;
        var errorMessage = "Server error";
        
        // Handle bad HTTP requests like model binding errors
        if (exception is BadHttpRequestException)
        {
             statusCode = StatusCodes.Status400BadRequest;
             errorMessage = "Bad Request";
             logger.LogWarning(exception, "Bad Request: {Message}", exception.Message);
        }
        else
        {
            logger.LogError(exception, "Exception Thrown: {Message}", exception.Message);
        }
        
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = errorMessage
        };
        
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";
        
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}