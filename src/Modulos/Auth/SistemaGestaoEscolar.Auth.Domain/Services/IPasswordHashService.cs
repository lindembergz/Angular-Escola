namespace SistemaGestaoEscolar.Auth.Domain.Services;

/// <summary>
/// Interface para serviços de hash de senha.
/// Abstrai a implementação específica de hashing para facilitar testes e mudanças futuras.
/// </summary>
public interface IPasswordHashService
{
    /// <summary>
    /// Gera um hash seguro para a senha
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifica se a senha corresponde ao hash
    /// </summary>
    bool VerifyPassword(string password, string hash);

    /// <summary>
    /// Verifica se o hash precisa ser atualizado (para upgrade de segurança)
    /// </summary>
    bool NeedsRehash(string hash);

    /// <summary>
    /// Gera um salt aleatório
    /// </summary>
    string GenerateSalt();

    /// <summary>
    /// Calcula a força de uma senha (0-100)
    /// </summary>
    int CalculatePasswordStrength(string password);

    /// <summary>
    /// Valida se uma senha atende aos critérios mínimos
    /// </summary>
    PasswordValidationResult ValidatePassword(string password);

    /// <summary>
    /// Gera uma senha temporária segura
    /// </summary>
    string GenerateTemporaryPassword(int length = 12);

    /// <summary>
    /// Verifica se uma senha está na lista de senhas comuns/vazadas
    /// </summary>
    bool IsPasswordCompromised(string password);
}

/// <summary>
/// Resultado da validação de senha
/// </summary>
public record PasswordValidationResult(
    bool IsValid,
    int Strength,
    IEnumerable<string> Errors,
    IEnumerable<string> Suggestions
);