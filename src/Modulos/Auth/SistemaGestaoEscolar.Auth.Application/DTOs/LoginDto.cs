using System.ComponentModel.DataAnnotations;

namespace SistemaGestaoEscolar.Auth.Application.DTOs;

/// <summary>
/// DTO para requisição de login
/// </summary>
public class LoginDto
{
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres")]
    public string Senha { get; set; } = string.Empty;

    public bool LembrarMe { get; set; } = false;
    
    public string? EnderecoIp { get; set; }
    public string? AgenteUsuario { get; set; }
}