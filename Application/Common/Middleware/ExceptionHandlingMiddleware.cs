using System.Net;
using System.Text.Json;
using CushyPay.Application.Common.Responses;
using FluentValidation;

namespace CushyPay.Application.Common.Middleware;

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

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;

        switch (exception)
        {
            case ValidationException validationException:
                code = HttpStatusCode.BadRequest;
                var errors = validationException.Errors.Select(e => e.ErrorMessage).ToList();
                result = JsonSerializer.Serialize(Result.Failure(errors));
                break;

            case ArgumentException argumentException:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(Result.Failure(argumentException.Message));
                break;

            case InvalidOperationException invalidOperationException:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(Result.Failure(invalidOperationException.Message));
                break;

            default:
                result = JsonSerializer.Serialize(Result.Failure("An error occurred while processing your request."));
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}

