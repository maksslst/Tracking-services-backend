using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.ExceptionHandlers;

public class ApplicationExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BaseApplicationException ex)
        {
            return false;
        }

        httpContext.Response.StatusCode = (int)ex.StatusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = (int)ex.StatusCode,
            Title = ex.Title,
            Detail = ex.Message,
            Instance = httpContext.Request.Path,
            Type = ex.GetType().Name
        };
        
        var problemDetailsContext = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = ex
        };

        await problemDetailsService.WriteAsync(problemDetailsContext);

        return true;
    }
}