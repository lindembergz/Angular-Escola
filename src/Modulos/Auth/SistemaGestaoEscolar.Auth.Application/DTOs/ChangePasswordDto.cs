using System.ComponentModel.DataAnnotations;

namespace SistemaGestaoEscolar.Auth.Application.DTOs;

/// <summary>
/// DTO para alteração de senha
/// </summary>
public class ChangePasswordDto
{
    [Required(ErrorMessage = "Senha atual é obrigatória")]
    public string SenhaAtual { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nova senha é obrigatória")]
    [MinLength(8, ErrorMessage = "Nova senha deve ter pelo menos 8 caracteres")]
    public string NovaSenha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
    [Compare(nameof(NovaSenha), ErrorMessage = "Confirmação deve ser igual à nova senha")]
    public string ConfirmarSenha { get; set; } = string.Empty;

    public bool InvalidarTodasSessoes { get; set; } = false;
}

