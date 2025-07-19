namespace SistemaGestaoEscolar.Auth.Application.DTOs;

/// <summary>
/// DTO para informações do usuário (compatível com frontend)
/// </summary>
public class UserInfoDto
{
    public Guid Id { get; set; }
    public string PrimeiroNome { get; set; } = string.Empty;
    public string UltimoNome { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CodigoPerfil { get; set; } = string.Empty;
    public string NomePerfil { get; set; } = string.Empty;
    public int NivelPerfil { get; set; }
    public string Iniciais { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public bool EmailConfirmado { get; set; }
    public DateTime? UltimoLoginEm { get; set; }
    public Guid? EscolaId { get; set; }
    public IEnumerable<string> Permissoes { get; set; } = new List<string>();
}