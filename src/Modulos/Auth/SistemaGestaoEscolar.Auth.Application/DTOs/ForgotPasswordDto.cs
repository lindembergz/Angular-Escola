using System.ComponentModel.DataAnnotations;

namespace SistemaGestaoEscolar.Auth.Application.DTOs;

/// <summary>
/// DTO para solicitação de recuperação de senha
/// </summary>
public record ForgotPasswordDto
{
    /// <summary>
    /// Email do usuário que esqueceu a senha
    /// </summary>
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    [StringLength(254, ErrorMessage = "Email não pode ter mais de 254 caracteres")]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// URL de callback para redirecionamento após reset
    /// </summary>
    public string? UrlCallback { get; init; }
}

/// <summary>
/// DTO para reset de senha
/// </summary>
public record ResetPasswordDto
{
    /// <summary>
    /// Email do usuário
    /// </summary>
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Token de reset de senha
    /// </summary>
    [Required(ErrorMessage = "Token é obrigatório")]
    public string Token { get; init; } = string.Empty;

    /// <summary>
    /// Nova senha
    /// </summary>
    [Required(ErrorMessage = "Nova senha é obrigatória")]
    [StringLength(128, MinimumLength = 8, ErrorMessage = "Nova senha deve ter entre 8 e 128 caracteres")]
    public string NovaSenha { get; init; } = string.Empty;

    /// <summary>
    /// Confirmação da nova senha
    /// </summary>
    [Required(ErrorMessage = "Confirmação da senha é obrigatória")]
    [Compare(nameof(NovaSenha), ErrorMessage = "Confirmação da senha não confere")]
    public string ConfirmarSenha { get; init; } = string.Empty;
}