using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Auth.Application.Interfaces;
using System.Security.Claims;

namespace SistemaGestaoEscolar.Auth.Infrastructure.Middleware;

/// <summary>
/// Middleware para validação automática de tokens JWT.
/// Processa tokens em requisições e configura o contexto de usuário.
/// </summary>
public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtMiddleware> _logger;

    public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
    {
        try
        {
            var token = ExtractTokenFromRequest(context.Request);
            
            if (!string.IsNullOrWhiteSpace(token))
            {
                await ValidateAndSetUserContext(context, tokenService, token);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro durante validação de token JWT");
            // Não bloquear a requisição, apenas não definir o usuário
        }

        await _next(context);
    }

    private static string? ExtractTokenFromRequest(HttpRequest request)
    {
        // Verificar header Authorization
        var authHeader = request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }

        // Verificar query parameter (para casos específicos como download de arquivos)
        var tokenFromQuery = request.Query["access_token"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(tokenFromQuery))
        {
            return tokenFromQuery;
        }

        // Verificar cookie (se configurado)
        var tokenFromCookie = request.Cookies["access_token"];
        if (!string.IsNullOrWhiteSpace(tokenFromCookie))
        {
            return tokenFromCookie;
        }

        return null;
    }

    private async Task ValidateAndSetUserContext(HttpContext context, ITokenService tokenService, string token)
    {
        try
        {
            var principal = await tokenService.ValidateTokenAsync(token);
            
            if (principal != null)
            {
                context.User = principal;
                
                // Log para auditoria (apenas em desenvolvimento)
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value;
                
                _logger.LogDebug("Token JWT validado para usuário: {UserId} ({Email})", userId, userEmail);
                
                // Adicionar informações adicionais ao contexto se necessário
                context.Items["UserId"] = userId;
                context.Items["UserEmail"] = userEmail;
                context.Items["UserRole"] = principal.FindFirst(ClaimTypes.Role)?.Value;
            }
            else
            {
                _logger.LogDebug("Token JWT inválido ou expirado");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao validar token JWT");
        }
    }
}

/// <summary>
/// Extensões para configurar o middleware JWT
/// </summary>
public static class JwtMiddlewareExtensions
{
    /// <summary>
    /// Adiciona o middleware JWT ao pipeline
    /// </summary>
    public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtMiddleware>();
    }
}