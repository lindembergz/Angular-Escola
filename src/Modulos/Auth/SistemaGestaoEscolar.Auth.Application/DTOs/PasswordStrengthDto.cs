namespace SistemaGestaoEscolar.Auth.Application.DTOs;

/// <summary>
/// DTO para validação de força da senha
/// </summary>
public class PasswordStrengthDto
{
    public int Forca { get; set; }
    public bool EhValida { get; set; }
    public IEnumerable<string> Erros { get; set; } = new List<string>();
    public IEnumerable<string> Sugestoes { get; set; } = new List<string>();
    public NivelForcaSenha Nivel { get; set; }
}

/// <summary>
/// Níveis de força da senha
/// </summary>
public enum NivelForcaSenha
{
    MuitoFraca,
    Fraca,
    Razoavel,
    Boa,
    Forte,
    MuitoForte
}