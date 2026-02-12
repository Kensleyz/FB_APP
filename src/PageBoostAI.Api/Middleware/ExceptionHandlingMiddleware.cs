using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PageBoostAI.Domain.Exceptions;

namespace PageBoostAI.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            UsageLimitExceededException => ((int)HttpStatusCode.TooManyRequests, "Usage Limit Exceeded"),
            InvalidEmailException => ((int)HttpStatusCode.BadRequest, "Invalid Email"),
            DomainException => ((int)HttpStatusCode.BadRequest, "Domain Error"),
            UnauthorizedAccessException => ((int)HttpStatusCode.Unauthorized, "Unauthorized"),
            _ => ((int)HttpStatusCode.InternalServerError, "Internal Server Error")
        };

        if (statusCode == (int)HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning(exception, "Handled exception: {Message}", exception.Message);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = statusCode == (int)HttpStatusCode.InternalServerError
                ? "An unexpected error occurred. Please try again later."
                : exception.Message,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
