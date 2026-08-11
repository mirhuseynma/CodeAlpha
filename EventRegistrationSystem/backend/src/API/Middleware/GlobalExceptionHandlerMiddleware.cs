namespace EventRegistrationSystem.API.Middleware;

public class GlobalExceptionHandlerMiddleware : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server error",
            Detail = "An unexpected error occurred."
        };

        if (exception is FluentValidation.ValidationException validationException)
        {
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = "Validation failed";
            problemDetails.Detail = "One or more validation errors occurred.";
            problemDetails.Extensions["errors"] = validationException.Errors
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(g => g.Key, g => g.ToArray());
        }
        else if (exception is EventRegistrationSystem.Application.Exceptions.UnauthorizedException unauthorizedException)
        {
            problemDetails.Status = StatusCodes.Status401Unauthorized;
            problemDetails.Title = "Unauthorized";
            problemDetails.Detail = unauthorizedException.Message;
        }
        else if (exception is EventRegistrationSystem.Application.Exceptions.ForbiddenException forbiddenException)
        {
            problemDetails.Status = StatusCodes.Status403Forbidden;
            problemDetails.Title = "Forbidden";
            problemDetails.Detail = forbiddenException.Message;
        }
        else if (exception is EventRegistrationSystem.Application.Exceptions.BadRequestException badRequestException)
        {
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = "Bad Request";
            problemDetails.Detail = badRequestException.Message;
        }
        else if (exception is EventRegistrationSystem.Application.Exceptions.NotFoundException notFoundException)
        {
            problemDetails.Status = StatusCodes.Status404NotFound;
            problemDetails.Title = "Not Found";
            problemDetails.Detail = notFoundException.Message;
        }
        else if (exception is EventRegistrationSystem.Application.Exceptions.ConflictException conflictException)
        {
            problemDetails.Status = StatusCodes.Status409Conflict;
            problemDetails.Title = "Conflict";
            problemDetails.Detail = conflictException.Message;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}


