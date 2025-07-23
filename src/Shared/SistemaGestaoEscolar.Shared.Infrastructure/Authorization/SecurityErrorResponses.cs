using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SistemaGestaoEscolar.Shared.Infrastructure.Authorization;

/// <summary>
/// Classe responsável por fornecer respostas padronizadas para erros de autorização.
/// Garante que informações sensíveis não sejam expostas e mantém consistência nas mensagens de erro.
/// </summary>
public static class SecurityErrorResponses
{
    /// <summary>
    /// Resposta padronizada para erro 401 (Unauthorized)
    /// </summary>
    public static ProblemDetails Unauthorized => new()
    {
        Title = "Não Autorizado",
        Detail = "Token de acesso inválido, expirado ou não fornecido. Faça login novamente.",
        Status = StatusCodes.Status401Unauthorized,
        Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
    };

    /// <summary>
    /// Resposta padronizada para erro 403 (Forbidden)
    /// </summary>
    public static ProblemDetails Forbidden => new()
    {
        Title = "Acesso Negado",
        Detail = "Você não possui permissões suficientes para acessar este recurso.",
        Status = StatusCodes.Status403Forbidden,
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
    };

    /// <summary>
    /// Resposta padronizada para token expirado
    /// </summary>
    public static ProblemDetails TokenExpired => new()
    {
        Title = "Token Expirado",
        Detail = "Seu token de acesso expirou. Faça login novamente para continuar.",
        Status = StatusCodes.Status401Unauthorized,
        Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
    };

    /// <summary>
    /// Resposta padronizada para token inválido
    /// </summary>
    public static ProblemDetails InvalidToken => new()
    {
        Title = "Token Inválido",
        Detail = "O token fornecido é inválido ou malformado.",
        Status = StatusCodes.Status401Unauthorized,
        Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
    };

    /// <summary>
    /// Resposta padronizada para role insuficiente
    /// </summary>
    public static ProblemDetails InsufficientRole => new()
    {
        Title = "Permissão Insuficiente",
        Detail = "Seu nível de acesso não permite executar esta operação.",
        Status = StatusCodes.Status403Forbidden,
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
    };

    /// <summary>
    /// Resposta padronizada para acesso a recurso de outra escola
    /// </summary>
    public static ProblemDetails CrossSchoolAccess => new()
    {
        Title = "Acesso Restrito",
        Detail = "Você só pode acessar recursos da sua escola.",
        Status = StatusCodes.Status403Forbidden,
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
    };

    /// <summary>
    /// Cria uma resposta de erro personalizada mantendo o padrão de segurança
    /// </summary>
    /// <param name="title">Título do erro</param>
    /// <param name="detail">Detalhes do erro (sem informações sensíveis)</param>
    /// <param name="statusCode">Código de status HTTP</param>
    /// <returns>ProblemDetails padronizado</returns>
    public static ProblemDetails CreateSecureError(string title, string detail, int statusCode)
    {
        return new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = statusCode,
            Type = statusCode switch
            {
                401 => "https://tools.ietf.org/html/rfc7235#section-3.1",
                403 => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            }
        };
    }

    /// <summary>
    /// Sanitiza mensagens de erro para remover informações sensíveis
    /// </summary>
    /// <param name="originalMessage">Mensagem original do erro</param>
    /// <returns>Mensagem sanitizada</returns>
    public static string SanitizeErrorMessage(string originalMessage)
    {
        if (string.IsNullOrWhiteSpace(originalMessage))
            return "Erro de autorização";

        // Remove informações potencialmente sensíveis
        var sensitivePatterns = new[]
        {
            @"user\s+id\s*:\s*\d+",
            @"email\s*:\s*[\w\.-]+@[\w\.-]+",
            @"token\s*:\s*[A-Za-z0-9\-_\.]+",
            @"password",
            @"secret",
            @"key\s*:\s*[A-Za-z0-9]+",
            @"connection\s+string",
            @"database",
            @"server\s*=",
            @"uid\s*=",
            @"pwd\s*="
        };

        var sanitized = originalMessage;
        foreach (var pattern in sensitivePatterns)
        {
            sanitized = System.Text.RegularExpressions.Regex.Replace(
                sanitized, pattern, "[INFORMAÇÃO REMOVIDA]", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        return sanitized;
    }
}