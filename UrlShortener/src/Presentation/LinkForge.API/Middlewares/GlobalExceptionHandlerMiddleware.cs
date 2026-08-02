namespace LinkForge.API.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
        catch (ValidationException ex)
        {
            await WriteValidationProblemAsync(context, ex);
        }
        catch (BadRequestException ex)
        {
            await WriteProblemAsync(context, ex, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (UnauthorizedException ex)
        {
            await WriteProblemAsync(context, ex, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteProblemAsync(context, ex, StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (NotFoundException ex)
        {
            await WriteProblemAsync(context, ex, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteProblemAsync(context, ex, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await WriteProblemAsync(context, ex, StatusCodes.Status500InternalServerError, "An unexpected error occurred while processing the request.");
        }
    }

    private static async Task WriteValidationProblemAsync(HttpContext context, ValidationException exception)
    {
        var response = LinkForge.API.Common.Http.ProblemDetailsFactory.Create(
            context,
            StatusCodes.Status400BadRequest,
            "Validation failed.",
            exception.Errors);

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(response);
    }

    private async Task WriteProblemAsync(HttpContext context, Exception exception, int statusCode, string detail)
    {
        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Server error occurred while processing the request.");
        }
        else
        {
            _logger.LogWarning(exception, "Request failed with status code {StatusCode}.", statusCode);
        }

        // Assuming IHostEnvironment is not immediately available, we pass null or we can inject it.
        // Let's pass null for environment for now since we don't have it in constructor
        var response = LinkForge.API.Common.Http.ProblemDetailsFactory.Create(
            context,
            statusCode,
            detail,
            errors: null,
            exception,
            null);

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}
