namespace SistemaGestaoEscolar.Auth.Application.DTOs;

/// <summary>
/// DTO de resposta de autenticação em português (compatível com frontend)
/// </summary>
public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string TipoToken { get; set; } = "Bearer";
    public int ExpiraEm { get; set; }
    public DateTime ExpiraEm_DateTime { get; set; }
    public UserInfoDto Usuario { get; set; } = null!;
    public bool RequerMudancaSenha { get; set; }
    public bool RequerConfirmacaoEmail { get; set; }
    public IEnumerable<string> Permissoes { get; set; } = new List<string>();
    public IEnumerable<SchoolAccessDto> Escolas { get; set; } = new List<SchoolAccessDto>();
}

/// <summary>
/// DTO para acesso a escolas
/// </summary>
public class SchoolAccessDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool EhPrincipal { get; set; }
    public IEnumerable<string> Permissoes { get; set; } = new List<string>();
}