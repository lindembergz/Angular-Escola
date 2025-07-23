using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SistemaGestaoEscolar.Shared.Infrastructure.Middleware;

namespace SistemaGestaoEscolar.Shared.Infrastructure.Configuration;

public static class SecurityMiddlewareExtensions
{
    /// <summary>
    /// Adiciona o middleware de logging de segurança ao pipeline da aplicação.
    /// Este middleware intercepta e registra tentativas de acesso não autorizado,
    /// atividades suspeitas e exceções relacionadas à segurança.
    /// </summary>
    /// <param name="app">O application builder</param>
    /// <returns>O application builder para encadeamento</returns>
    public static IApplicationBuilder UseSecurityLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityLoggingMiddleware>();
    }

    /// <summary>
    /// Adiciona o middleware de tratamento de erros de autorização ao pipeline da aplicação.
    /// Este middleware intercepta exceções de autorização e retorna respostas padronizadas
    /// sem expor informações sensíveis.
    /// </summary>
    /// <param name="app">O application builder</param>
    /// <returns>O application builder para encadeamento</returns>
    public static IApplicationBuilder UseAuthorizationErrorHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AuthorizationErrorMiddleware>();
    }

    /// <summary>
    /// Configura todos os middleware de segurança na ordem correta.
    /// Deve ser chamado após UseRouting() e antes de UseAuthentication().
    /// </summary>
    /// <param name="app">O application builder</param>
    /// <returns>O application builder para encadeamento</returns>
    public static IApplicationBuilder UseSecurityMiddleware(this IApplicationBuilder app)
    {
        // Ordem importante: 
        // 1. Tratamento de erros de autorização (deve vir primeiro para capturar todas as exceções)
        // 2. Logging de segurança (para registrar eventos)
        app.UseAuthorizationErrorHandling();
        app.UseSecurityLogging();
        
        return app;
    }
}