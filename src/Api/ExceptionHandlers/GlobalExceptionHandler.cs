using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.ExceptionHandlers;

public class GlobalExceptionHandler(IProblemDetailsService _problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        var problemDatails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "An error occured while processing your request",
            Detail = exception.Message,
            Instance = httpContext.Request.Path,
            Type = exception.GetType().Name
        };

        var problemDatailsContext = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDatails,
            Exception = exception
        };
            
        await _problemDetailsService.WriteAsync(problemDatailsContext);
        return true;
    }
}