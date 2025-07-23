using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Shared.Infrastructure.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Authentication;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace SistemaGestaoEscolar.Shared.Infrastructure.Middleware;

/// <summary>
/// Middleware responsável por interceptar e tratar exceções de autorização,
/// retornando respostas padronizadas e seguras sem expor informações sensíveis.
/// </summary>
public class AuthorizationErrorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthorizationErrorMiddleware> _logger;

    public AuthorizationErrorMiddleware(RequestDelegate next, ILogger<AuthorizationErrorMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
            
            // Handle authorization failures that don't throw exceptions
            await HandleAuthorizationResponse(context);
        }
        catch (Exception ex)
        {
            await HandleAuthorizationException(context, ex);
        }
    }

    /// <summary>
    /// Trata respostas de autorização que não geram exceções (401, 403)
    /// </summary>
    private async Task HandleAuthorizationResponse(HttpContext context)
    {
        // Only handle if response hasn't been written yet
        if (context.Response.HasStarted)
            return;

        var statusCode = context.Response.StatusCode;
        
        if (statusCode == StatusCodes.Status401Unauthorized)
        {
            await WriteSecurityErrorResponse(context, SecurityErrorResponses.Unauthorized);
            LogSecurityEvent(context, "Acesso não autorizado detectado", null);
        }
        else if (statusCode == StatusCodes.Status403Forbidden)
        {
            await WriteSecurityErrorResponse(context, SecurityErrorResponses.Forbidden);
            LogSecurityEvent(context, "Acesso negado detectado", null);
        }
    }

    /// <summary>
    /// Trata exceções relacionadas à autorização
    /// </summary>
    private async Task HandleAuthorizationException(HttpContext context, Exception exception)
    {
        var errorResponse = DetermineErrorResponse(exception);
        var sanitizedMessage = SecurityErrorResponses.SanitizeErrorMessage(exception.Message);
        
        await WriteSecurityErrorResponse(context, errorResponse);
        LogSecurityEvent(context, sanitizedMessage, exception);
    }

    /// <summary>
    /// Determina a resposta de erro apropriada baseada no tipo de exceção
    /// </summary>
    private static Microsoft.AspNetCore.Mvc.ProblemDetails DetermineErrorResponse(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException => SecurityErrorResponses.Unauthorized,
            SecurityException => SecurityErrorResponses.Forbidden,
            AuthenticationException => SecurityErrorResponses.InvalidToken,
            SecurityTokenExpiredException => SecurityErrorResponses.TokenExpired,
            SecurityTokenValidationException => SecurityErrorResponses.InvalidToken,
            AuthorizationFailureException => SecurityErrorResponses.InsufficientRole,
            _ when IsJwtRelated(exception) => SecurityErrorResponses.InvalidToken,
            _ when IsAuthorizationRelated(exception) => SecurityErrorResponses.Forbidden,
            _ => SecurityErrorResponses.Unauthorized
        };
    }

    /// <summary>
    /// Verifica se a exceção está relacionada a JWT
    /// </summary>
    private static bool IsJwtRelated(Exception exception)
    {
        var message = exception.Message.ToLowerInvariant();
        return message.Contains("jwt") || 
               message.Contains("token") || 
               message.Contains("bearer") ||
               exception.GetType().Name.Contains("Token", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Verifica se a exceção está relacionada à autorização
    /// </summary>
    private static bool IsAuthorizationRelated(Exception exception)
    {
        var message = exception.Message.ToLowerInvariant();
        return message.Contains("authorization") || 
               message.Contains("authorize") || 
               message.Contains("permission") ||
               message.Contains("role") ||
               message.Contains("policy") ||
               exception.GetType().Name.Contains("Authorization", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Escreve a resposta de erro padronizada
    /// </summary>
    private static async Task WriteSecurityErrorResponse(HttpContext context, Microsoft.AspNetCore.Mvc.ProblemDetails problemDetails)
    {
        // Avoid writing response if it has already started
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();
        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        // Add security headers
        context.Response.Headers.Remove("Server");
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

        // Add trace ID for debugging (safe to expose)
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var jsonResponse = JsonSerializer.Serialize(problemDetails, jsonOptions);
        await context.Response.WriteAsync(jsonResponse);
    }

    /// <summary>
    /// Registra eventos de segurança no log
    /// </summary>
    private void LogSecurityEvent(HttpContext context, string message, Exception? exception)
    {
        var securityEvent = new
        {
            Timestamp = DateTime.UtcNow,
            TraceId = context.TraceIdentifier,
            IpAddress = GetClientIpAddress(context),
            UserAgent = context.Request.Headers.UserAgent.ToString(),
            RequestPath = context.Request.Path.Value,
            HttpMethod = context.Request.Method,
            StatusCode = context.Response.StatusCode,
            UserId = context.User?.Identity?.Name,
            UserRoles = context.User?.Claims
                .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList() ?? new List<string>(),
            Message = message,
            ExceptionType = exception?.GetType().Name
        };

        if (exception != null)
        {
            _logger.LogWarning(exception, 
                "Erro de autorização interceptado: {Message}. Detalhes: {@SecurityEvent}", 
                message, securityEvent);
        }
        else
        {
            _logger.LogWarning(
                "Evento de autorização detectado: {Message}. Detalhes: {@SecurityEvent}", 
                message, securityEvent);
        }
    }

    /// <summary>
    /// Obtém o endereço IP do cliente considerando proxies
    /// </summary>
    private static string GetClientIpAddress(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}

/// <summary>
/// Exceção personalizada para falhas de autorização
/// </summary>
public class AuthorizationFailureException : Exception
{
    public AuthorizationFailureException(string message) : base(message) { }
    public AuthorizationFailureException(string message, Exception innerException) : base(message, innerException) { }
}