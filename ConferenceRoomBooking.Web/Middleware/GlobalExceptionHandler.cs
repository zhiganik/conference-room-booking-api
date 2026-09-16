using System.Net;
using ConferenceRoomBooking.Bll.Common.Bookings.Exceptions;
using ConferenceRoomBooking.Bll.Common.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.Web.Middleware;

public class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title) = MapException(exception);
        
        if (exception is AppException appException)
        {
            logger.LogWarning(exception, appException.LogTemplate, appException.LogArgs);
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
            Detail = exception is AppException clientSafeException
                ? clientSafeException.Message
                : "An unexpected error occurred. Please contact support if this persists.",
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier
            }
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
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