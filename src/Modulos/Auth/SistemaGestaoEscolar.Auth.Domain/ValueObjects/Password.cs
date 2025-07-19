using SistemaGestaoEscolar.Shared.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace SistemaGestaoEscolar.Auth.Domain.ValueObjects;

/// <summary>
/// Value Object que representa uma senha com validações de segurança.
/// Implementa políticas de senha forte seguindo OWASP guidelines.
/// </summary>
public class Password : ValueObject
{
    private static readonly Regex UpperCaseRegex = new(@"[A-Z]", RegexOptions.Compiled);
    private static readonly Regex LowerCaseRegex = new(@"[a-z]", RegexOptions.Compiled);
    private static readonly Regex DigitRegex = new(@"[0-9]", RegexOptions.Compiled);
    private static readonly Regex SpecialCharRegex = new(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]", RegexOptions.Compiled);

    public string HashedValue { get; private set; } = string.Empty;

    private Password() { } // Para EF Core

    /// <summary>
    /// Cria uma nova senha a partir de texto plano (será hasheada automaticamente)
    /// </summary>
    public Password(string plainTextPassword)
    {
        ValidatePassword(plainTextPassword);
        HashedValue = BCrypt.Net.BCrypt.HashPassword(plainTextPassword, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    /// <summary>
    /// Cria uma senha a partir de um hash já existente (para reconstrução do banco)
    /// </summary>
    public static Password FromHash(string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Hash da senha não pode ser vazio", nameof(hashedPassword));

        return new Password { HashedValue = hashedPassword };
    }

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Senha não pode ser vazia", nameof(password));

        if (password.Length < 8)
            throw new ArgumentException("Senha deve ter pelo menos 8 caracteres", nameof(password));

        if (password.Length > 128)
            throw new ArgumentException("Senha não pode ter mais de 128 caracteres", nameof(password));

        if (!UpperCaseRegex.IsMatch(password))
            throw new ArgumentException("Senha deve conter pelo menos uma letra maiúscula", nameof(password));

        if (!LowerCaseRegex.IsMatch(password))
            throw new ArgumentException("Senha deve conter pelo menos uma letra minúscula", nameof(password));

        if (!DigitRegex.IsMatch(password))
            throw new ArgumentException("Senha deve conter pelo menos um número", nameof(password));

        if (!SpecialCharRegex.IsMatch(password))
            throw new ArgumentException("Senha deve conter pelo menos um caractere especial", nameof(password));

        // Verificar sequências comuns
        if (ContainsCommonSequences(password))
            throw new ArgumentException("Senha não pode conter sequências comuns (123, abc, qwerty, etc.)", nameof(password));

        // Verificar repetições excessivas
        if (ContainsExcessiveRepetition(password))
            throw new ArgumentException("Senha não pode conter muitos caracteres repetidos consecutivos", nameof(password));
    }

    private static bool ContainsCommonSequences(string password)
    {
        var commonSequences = new[]
        {
            "123", "234", "345", "456", "567", "678", "789", "890",
            "abc", "bcd", "cde", "def", "efg", "fgh", "ghi", "hij",
            "qwe", "wer", "ert", "rty", "tyu", "yui", "uio", "iop",
            "asd", "sdf", "dfg", "fgh", "ghj", "hjk", "jkl",
            "zxc", "xcv", "cvb", "vbn", "bnm"
        };

        var lowerPassword = password.ToLowerInvariant();
        return commonSequences.Any(seq => lowerPassword.Contains(seq));
    }

    private static bool ContainsExcessiveRepetition(string password)
    {
        for (int i = 0; i < password.Length - 2; i++)
        {
            if (password[i] == password[i + 1] && password[i + 1] == password[i + 2])
                return true;
        }
        return false;
    }

    /// <summary>
    /// Verifica se a senha fornecida corresponde ao hash armazenado
    /// </summary>
    public bool Verificar(string senhaTextoPlano)
    {
        if (string.IsNullOrWhiteSpace(senhaTextoPlano))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(senhaTextoPlano, HashedValue);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Verifica se a senha precisa ser re-hasheada (para upgrade de segurança)
    /// </summary>
    public bool PrecisaRehash()
    {
        try
        {
            return BCrypt.Net.BCrypt.PasswordNeedsRehash(HashedValue, 12);
        }
        catch
        {
            return true; // Se não conseguir verificar, assume que precisa rehash
        }
    }

    /// <summary>
    /// Calcula a força da senha (0-100)
    /// </summary>
    public static int CalcularForca(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
            return 0;

        int pontuacao = 0;

        // Comprimento
        if (senha.Length >= 8) pontuacao += 10;
        if (senha.Length >= 12) pontuacao += 10;
        if (senha.Length >= 16) pontuacao += 10;

        // Complexidade
        if (UpperCaseRegex.IsMatch(senha)) pontuacao += 15;
        if (LowerCaseRegex.IsMatch(senha)) pontuacao += 15;
        if (DigitRegex.IsMatch(senha)) pontuacao += 15;
        if (SpecialCharRegex.IsMatch(senha)) pontuacao += 15;

        // Diversidade de caracteres
        var caracteresUnicos = senha.Distinct().Count();
        if (caracteresUnicos >= senha.Length * 0.7) pontuacao += 10;

        // Penalidades
        if (ContainsCommonSequences(senha)) pontuacao -= 20;
        if (ContainsExcessiveRepetition(senha)) pontuacao -= 15;

        return Math.Max(0, Math.Min(100, pontuacao));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return HashedValue;
    }

    public override string ToString() => "[PROTECTED]";
}