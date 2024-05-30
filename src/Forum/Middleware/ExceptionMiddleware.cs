using System.Net;
using Common.DTOs;
using Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Services.Contracts.Contracts;

namespace Forum.Middleware;

public static class ExceptionMiddleware
{
    /// <summary>
    /// Configures the exception handler middleware to handle exceptions and return a JSON response.
    /// </summary>
    /// <param name="app">The <see cref="logger"/> instance this method extends.</param>
    /// <param name="logger">The <see cref="WebApplication"/> instance to log the exceptions.</param>
    public static void ConfigureExceptionHandler(this WebApplication app, ILoggerService logger)
    {
        app.UseExceptionHandler(error =>
            error.Run(async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = contextFeature?.Error ?? new Exception("An unknown error occurred.");

                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    await HandleAsApi(context, exception, logger);
                }
                else{
                    await HandleAsWeb(context, exception, logger);
                }
            }));
    }

    private static async Task HandleAsApi(HttpContext context, Exception exception, ILoggerService logger)
    {
        switch (exception)
        {
            case BadRequest:
                logger.LogInfo(
                    $"[Expected Error] Bad Request: thrown by {exception.TargetSite}: {exception.Message}");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync(new ErrorDetails
                (
                    context.Response.StatusCode,
                    $"{Resources.Localization.BadRequest}. {exception.Message}"
                ).ToString());
                break;
            case Conflict:
                logger.LogInfo(
                    $"[Expected Error] Conflict: thrown by {exception.TargetSite}: {exception.Message}");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                await context.Response.WriteAsync(new ErrorDetails
                (
                    context.Response.StatusCode,
                    $"{Resources.Localization.Conflict}. {exception.Message}"
                ).ToString());
                break;
            case NotFound:
                logger.LogInfo(
                    $"[Expected Error] Not Found: thrown by {exception.TargetSite}: {exception.Message}");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.WriteAsync(new ErrorDetails
                (
                    context.Response.StatusCode,
                    $"{Resources.Localization.NotFound}. {exception.Message}"
                ).ToString());
                break;
            case Unauthorized:
                logger.LogInfo(
                    $"[Expected Error] Unauthorized: thrown by {exception.TargetSite}: {exception.Message}");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync(new ErrorDetails
                (
                    context.Response.StatusCode,
                    $"{Resources.Localization.Unauthorized}. {exception.Message}"
                ).ToString());
                break;
            case Forbidden:
                logger.LogInfo(
                    $"[Expected Error] Forbidden: thrown by {exception.TargetSite}: {exception.Message}");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync(new ErrorDetails
                (
                    context.Response.StatusCode,
                    $"{Resources.Localization.Forbidden}. {exception.Message}"
                ).ToString());
                break;
            case FluentValidation.ValidationException:
                logger.LogInfo(
                    $"[Expected Error] Validation Error: thrown by {exception.TargetSite}: {exception.Message}");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync(new ErrorDetails
                (
                    context.Response.StatusCode,
                    $"{Resources.Localization.BadRequest}. {exception.Message}"
                ).ToString());
                break;
            default:
                logger.LogError($"Something went wrong: {exception}");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(new ErrorDetails
                (
                    context.Response.StatusCode,
                    Resources.Localization.InternalError
                ).ToString());
                break;
        }
    }

    private static Task HandleAsWeb(HttpContext context, Exception exception, ILoggerService logger)
    {
        logger.LogInfo($"Redirecting error from WEB. {exception.Message}");
        
        switch (exception)
        {
            case NotFound:
                context.Response.Redirect($"/error/404/{exception.Message}");
                break;
            case BadRequest:
                context.Response.Redirect($"/error/400/{exception.Message}");
                break;
            case Conflict:
                context.Response.Redirect($"/error/409/{exception.Message}");
                break;
            case Unauthorized:
                context.Response.Redirect($"/Login");
                break;
            case Forbidden:
                context.Response.Redirect($"/error/403/{exception.Message}");
                break;
            default:
                context.Response.Redirect($"/error/500/{exception.Message}");
                break;
        }

        return Task.CompletedTask;
    }
}