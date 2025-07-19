using System.ComponentModel.DataAnnotations;

namespace SistemaGestaoEscolar.Auth.Application.DTOs;

/// <summary>
/// DTO para renovação de token
/// </summary>
public class RefreshTokenDto
{
    [Required(ErrorMessage = "Refresh token é obrigatório")]
    public string RefreshToken { get; set; } = string.Empty;
    
    public string? TokenAcesso { get; set; }
    public string? EnderecoIp { get; set; }
    public string? AgenteUsuario { get; set; }
}