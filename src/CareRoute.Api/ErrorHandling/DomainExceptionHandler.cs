// src/CareRoute.Api/ErrorHandling/DomainExceptionHandler.cs
using CareRoute.Domain.SharedKernel;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CareRoute.Api.ErrorHandling;

public sealed class DomainExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<DomainExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)                     // ①
            return false;

        logger.LogInformation("Request rejected by domain rule {ErrorCode}", domainException.Code);   // ②

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "A business rule was violated.",
            Detail = domainException.Message,                                      // ③
            Type = "https://careroute.example/problems/domain-rule"
        };
        problem.Extensions["code"] = domainException.Code;                         // ④

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem
        });
    }
}