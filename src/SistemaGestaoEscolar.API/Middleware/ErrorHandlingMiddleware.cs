using System.Net;
using System.Text.Json;
using System.Security;
using SistemaGestaoEscolar.Shared.Infrastructure.Authorization;

namespace SistemaGestaoEscolar.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            // Skip authorization-related exceptions as they are handled by AuthorizationErrorMiddleware
            if (IsAuthorizationException(ex))
            {
                throw; // Re-throw to let AuthorizationErrorMiddleware handle it
            }

            _logger.LogError(ex, "Erro não tratado na aplicação");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static bool IsAuthorizationException(Exception exception)
    {
        return exception is UnauthorizedAccessException ||
               exception is SecurityException ||
               exception.GetType().Name.Contains("Authorization", StringComparison.OrdinalIgnoreCase) ||
               exception.GetType().Name.Contains("Token", StringComparison.OrdinalIgnoreCase) ||
               exception.Message.Contains("authorization", StringComparison.OrdinalIgnoreCase) ||
               exception.Message.Contains("token", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Avoid writing response if it has already started
        if (context.Response.HasStarted)
            return;

        context.Response.ContentType = "application/json";
        
        var response = new ErrorResponse();

        switch (exception)
        {
            case ArgumentException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = SanitizeMessage(exception.Message);
                break;
            case KeyNotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = "Recurso não encontrado";
                break;
            case InvalidOperationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Operação inválida";
                break;
            case NotSupportedException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Operação não suportada";
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "Erro interno do servidor";
                // Only include details in development
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    response.Details = SanitizeMessage(exception.ToString());
                }
                break;
        }

        context.Response.StatusCode = response.StatusCode;

        // Add security headers
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

        // Add trace ID for debugging
        response.TraceId = context.TraceIdentifier;
        response.Timestamp = DateTime.UtcNow;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private static string SanitizeMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return "Erro não especificado";

        // Use the same sanitization logic from SecurityErrorResponses
        return SecurityErrorResponses.SanitizeErrorMessage(message);
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}