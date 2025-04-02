using System.Data.Common;
using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.ExceptionHandlers;

public class DbExceptionHandler(IProblemDetailsService _problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not DbException ex)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        var problemDatails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title ="Database error",
            Detail = ex.Message,
            Instance = httpContext.Request.Path,
            Type = ex.GetType().Name
        };
        
        var problemDatailsContext = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDatails,
            Exception = ex
        };

        await _problemDetailsService.WriteAsync(problemDatailsContext);

        return true;
    }
}