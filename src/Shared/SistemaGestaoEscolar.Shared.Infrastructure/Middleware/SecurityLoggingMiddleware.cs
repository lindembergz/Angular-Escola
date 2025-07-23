using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security;
using System.Security.Claims;
using System.Text.Json;

namespace SistemaGestaoEscolar.Shared.Infrastructure.Middleware;

public class SecurityLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityLoggingMiddleware> _logger;

    public SecurityLoggingMiddleware(RequestDelegate next, ILogger<SecurityLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalStatusCode = context.Response.StatusCode;
        
        try
        {
            await _next(context);
            
            // Log security events based on response status
            LogSecurityEvent(context, originalStatusCode);
        }
        catch (Exception ex)
        {
            // Log security-related exceptions
            LogSecurityException(context, ex);
            throw; // Re-throw to maintain exception flow
        }
    }

    private void LogSecurityEvent(HttpContext context, int originalStatusCode)
    {
        var statusCode = context.Response.StatusCode;
        
        // Log unauthorized access attempts (401)
        if (statusCode == StatusCodes.Status401Unauthorized)
        {
            var securityEvent = CreateSecurityEvent(context, SecurityEventType.UnauthorizedAccess);
            
            _logger.LogWarning(SecurityEvents.UnauthorizedAccess,
                "Tentativa de acesso não autorizado detectada. {@SecurityEvent}",
                securityEvent);
        }
        // Log forbidden access attempts (403)
        else if (statusCode == StatusCodes.Status403Forbidden)
        {
            var securityEvent = CreateSecurityEvent(context, SecurityEventType.ForbiddenAccess);
            
            _logger.LogWarning(SecurityEvents.ForbiddenAccess,
                "Tentativa de acesso negado detectada. {@SecurityEvent}",
                securityEvent);
        }
        // Log suspicious activity patterns
        else if (IsSuspiciousActivity(context))
        {
            var securityEvent = CreateSecurityEvent(context, SecurityEventType.SuspiciousActivity);
            
            _logger.LogError(SecurityEvents.SuspiciousActivity,
                "Atividade suspeita detectada. {@SecurityEvent}",
                securityEvent);
        }
    }

    private void LogSecurityException(HttpContext context, Exception exception)
    {
        // Log security-related exceptions
        if (IsSecurityRelated(exception))
        {
            var securityEvent = CreateSecurityEvent(context, SecurityEventType.SecurityException);
            securityEvent.ExceptionType = exception.GetType().Name;
            securityEvent.ExceptionMessage = exception.Message;
            
            _logger.LogError(SecurityEvents.SecurityException, exception,
                "Exceção relacionada à segurança detectada. {@SecurityEvent}",
                securityEvent);
        }
    }

    private SecurityEvent CreateSecurityEvent(HttpContext context, SecurityEventType eventType)
    {
        var request = context.Request;
        var user = context.User;
        
        return new SecurityEvent
        {
            EventType = eventType,
            Timestamp = DateTime.UtcNow,
            IpAddress = GetClientIpAddress(context),
            UserAgent = request.Headers.UserAgent.ToString(),
            RequestPath = request.Path.Value ?? string.Empty,
            HttpMethod = request.Method,
            QueryString = request.QueryString.Value ?? string.Empty,
            UserId = GetUserId(user),
            UserName = GetUserName(user),
            UserRoles = GetUserRoles(user),
            StatusCode = context.Response.StatusCode,
            RequestHeaders = GetSecurityRelevantHeaders(request),
            SessionId = context.Session?.Id,
            TraceId = context.TraceIdentifier
        };
    }

    private string GetClientIpAddress(HttpContext context)
    {
        // Check for forwarded IP first (for load balancers/proxies)
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

    private string? GetUserId(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
               user.FindFirst("sub")?.Value ??
               user.FindFirst("user_id")?.Value;
    }

    private string? GetUserName(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Name)?.Value ??
               user.FindFirst("name")?.Value ??
               user.FindFirst("username")?.Value;
    }

    private List<string> GetUserRoles(ClaimsPrincipal user)
    {
        return user.FindAll(ClaimTypes.Role)
                  .Select(c => c.Value)
                  .ToList();
    }

    private Dictionary<string, string> GetSecurityRelevantHeaders(HttpRequest request)
    {
        var relevantHeaders = new Dictionary<string, string>();
        
        // Headers that are relevant for security analysis
        var securityHeaders = new[]
        {
            "Authorization",
            "X-Forwarded-For",
            "X-Real-IP",
            "X-Forwarded-Proto",
            "Origin",
            "Referer",
            "Accept",
            "Accept-Language",
            "Accept-Encoding"
        };

        foreach (var headerName in securityHeaders)
        {
            if (request.Headers.ContainsKey(headerName))
            {
                var value = request.Headers[headerName].ToString();
                // Mask sensitive information in Authorization header
                if (headerName == "Authorization" && !string.IsNullOrEmpty(value))
                {
                    value = value.Length > 20 ? $"{value[..20]}..." : "[MASKED]";
                }
                relevantHeaders[headerName] = value;
            }
        }

        return relevantHeaders;
    }

    private bool IsSuspiciousActivity(HttpContext context)
    {
        var request = context.Request;
        var path = request.Path.Value?.ToLower() ?? string.Empty;
        
        // Detect common attack patterns
        var suspiciousPatterns = new[]
        {
            "admin", "wp-admin", "phpmyadmin", "sql", "script",
            "../", "..\\", "<script", "javascript:", "eval(",
            "union select", "drop table", "insert into", "delete from"
        };

        // Check path and query string for suspicious patterns
        var queryString = request.QueryString.Value?.ToLower() ?? string.Empty;
        var fullUrl = $"{path}{queryString}";

        return suspiciousPatterns.Any(pattern => fullUrl.Contains(pattern));
    }

    private bool IsSecurityRelated(Exception exception)
    {
        var securityExceptionTypes = new[]
        {
            typeof(UnauthorizedAccessException),
            typeof(SecurityException),
            typeof(ArgumentException) // Can be security-related in auth contexts
        };

        return securityExceptionTypes.Any(type => type.IsAssignableFrom(exception.GetType())) ||
               exception.Message.Contains("authorization", StringComparison.OrdinalIgnoreCase) ||
               exception.Message.Contains("authentication", StringComparison.OrdinalIgnoreCase) ||
               exception.Message.Contains("token", StringComparison.OrdinalIgnoreCase);
    }
}

public class SecurityEvent
{
    public SecurityEventType EventType { get; set; }
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string RequestPath { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public string QueryString { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public List<string> UserRoles { get; set; } = new();
    public int StatusCode { get; set; }
    public Dictionary<string, string> RequestHeaders { get; set; } = new();
    public string? SessionId { get; set; }
    public string TraceId { get; set; } = string.Empty;
    public string? ExceptionType { get; set; }
    public string? ExceptionMessage { get; set; }
}

public enum SecurityEventType
{
    UnauthorizedAccess,
    ForbiddenAccess,
    SuspiciousActivity,
    SecurityException
}

public static class SecurityEvents
{
    public static readonly EventId UnauthorizedAccess = new(4001, "UnauthorizedAccess");
    public static readonly EventId ForbiddenAccess = new(4003, "ForbiddenAccess");
    public static readonly EventId SuspiciousActivity = new(4999, "SuspiciousActivity");
    public static readonly EventId SecurityException = new(5001, "SecurityException");
}