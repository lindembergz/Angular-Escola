using SistemaGestaoEscolar.Shared.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace SistemaGestaoEscolar.Auth.Domain.ValueObjects;

/// <summary>
/// Value Object que representa um endereço de email válido.
/// Implementa validações rigorosas seguindo RFC 5322.
/// </summary>
public class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; private set; } = string.Empty;

    private Email() { } // Para EF Core

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email não pode ser vazio", nameof(value));

        var normalizedEmail = value.Trim().ToLowerInvariant();

        if (normalizedEmail.Length > 254) // RFC 5321 limit
            throw new ArgumentException("Email não pode ter mais de 254 caracteres", nameof(value));

        if (!EmailRegex.IsMatch(normalizedEmail))
            throw new ArgumentException("Formato de email inválido", nameof(value));

        // Validações adicionais de segurança
        if (normalizedEmail.Contains("..") || normalizedEmail.StartsWith('.') || normalizedEmail.EndsWith('.'))
            throw new ArgumentException("Email contém pontos consecutivos ou inválidos", nameof(value));

        Value = normalizedEmail;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email?.Value ?? string.Empty;
    public static implicit operator Email(string value) => new(value);

    /// <summary>
    /// Obtém o domínio do email (parte após @)
    /// </summary>
    public string GetDomain()
    {
        var atIndex = Value.IndexOf('@');
        return atIndex >= 0 ? Value.Substring(atIndex + 1) : string.Empty;
    }

    /// <summary>
    /// Obtém a parte local do email (parte antes do @)
    /// </summary>
    public string GetLocalPart()
    {
        var atIndex = Value.IndexOf('@');
        return atIndex >= 0 ? Value.Substring(0, atIndex) : Value;
    }

    /// <summary>
    /// Verifica se o email pertence a um domínio específico
    /// </summary>
    public bool PertenceAoDominio(string dominio)
    {
        if (string.IsNullOrWhiteSpace(dominio))
            return false;

        return GetDomain().Equals(dominio.Trim().ToLowerInvariant(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gera um email mascarado para exibição (ex: j***@example.com)
    /// </summary>
    public string ObterMascarado()
    {
        var parteLocal = GetLocalPart();
        var dominio = GetDomain();

        if (parteLocal.Length <= 2)
            return $"{parteLocal[0]}***@{dominio}";

        return $"{parteLocal[0]}***{parteLocal[^1]}@{dominio}";
    }
}