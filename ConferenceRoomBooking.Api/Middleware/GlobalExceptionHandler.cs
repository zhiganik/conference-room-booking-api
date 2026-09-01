using System.Net;
using ConferenceRoomBooking.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.Api.Middleware;

public class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title) = MapException(exception);
        
        if (exception is AppException)
        {
            logger.LogWarning(exception, "Handled domain exception: {Message}", exception.Message);
        }
        else
        {
            logger.LogError(exception, "Unhandled exception processing {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
        }
        
        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/problem+json";
    
        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Type = exception.GetType().Name,
            Detail = exception is AppException appException
                ? appException.Message
                : "An unexpected error occurred. Please contact support if this persists.",
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier
            }
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
        
        // I have some problems with swagger when return this. Swagger does not give info below while Postman does
        // httpContext.Response.StatusCode = (int)statusCode;
        //
        // var problemDetails = new ProblemDetails
        // {
        //     Status = (int)statusCode,
        //     Title = title,
        //     Type = exception.GetType().Name,
        //     Detail = exception is AppException appException
        //         ? appException.Message
        //         : "An unexpected error occurred. Please contact support if this persists."
        // };
        //
        // return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        // {
        //     HttpContext = httpContext,
        //     Exception = exception,
        //     ProblemDetails = problemDetails
        // });
    }
    
    private static (HttpStatusCode StatusCode, string Title) MapException(Exception exception) => exception switch
    {
        NotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
        ConflictException => (HttpStatusCode.Conflict, "Conflict"),
        RoomUnavailableException => (HttpStatusCode.Conflict, "Room unavailable"),
        UnauthorizedException => (HttpStatusCode.Unauthorized, "Authentication failed"),
        _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
    };
}