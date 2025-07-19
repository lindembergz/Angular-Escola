using FluentValidation;
using SistemaGestaoEscolar.Auth.Application.DTOs;

namespace SistemaGestaoEscolar.Auth.Infrastructure.Validators;

/// <summary>
/// Validador para LoginDto usando FluentValidation.
/// Define regras de validação para dados de login.
/// </summary>
public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email é obrigatório")
            .EmailAddress()
            .WithMessage("Formato de email inválido")
            .MaximumLength(254)
            .WithMessage("Email não pode ter mais de 254 caracteres");

        RuleFor(x => x.Senha)
            .NotEmpty()
            .WithMessage("Senha é obrigatória")
            .MinimumLength(8)
            .WithMessage("Senha deve ter pelo menos 8 caracteres")
            .MaximumLength(128)
            .WithMessage("Senha não pode ter mais de 128 caracteres");
    }
}

/// <summary>
/// Validador para RefreshTokenDto usando FluentValidation.
/// </summary>
public class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
{
    public RefreshTokenDtoValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token é obrigatório")
            .MinimumLength(10)
            .WithMessage("Refresh token inválido");
    }
}

/// <summary>
/// Validador para ChangePasswordDto usando FluentValidation.
/// </summary>
public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.SenhaAtual)
            .NotEmpty()
            .WithMessage("Senha atual é obrigatória");

        RuleFor(x => x.NovaSenha)
            .NotEmpty()
            .WithMessage("Nova senha é obrigatória")
            .MinimumLength(8)
            .WithMessage("Nova senha deve ter pelo menos 8 caracteres")
            .MaximumLength(128)
            .WithMessage("Nova senha não pode ter mais de 128 caracteres")
            .Matches(@"[A-Z]")
            .WithMessage("Nova senha deve conter pelo menos uma letra maiúscula")
            .Matches(@"[a-z]")
            .WithMessage("Nova senha deve conter pelo menos uma letra minúscula")
            .Matches(@"[0-9]")
            .WithMessage("Nova senha deve conter pelo menos um número")
            .Matches(@"[!@#$%^&*()_+\-\[\]{};':""\\|,.<>\/?]")
            .WithMessage("Nova senha deve conter pelo menos um caractere especial");

        RuleFor(x => x.ConfirmarSenha)
            .NotEmpty()
            .WithMessage("Confirmação da senha é obrigatória")
            .Equal(x => x.NovaSenha)
            .WithMessage("Confirmação da senha não confere");

        RuleFor(x => x.NovaSenha)
            .NotEqual(x => x.SenhaAtual)
            .WithMessage("A nova senha deve ser diferente da senha atual")
            .When(x => !string.IsNullOrEmpty(x.SenhaAtual));
    }
}

/// <summary>
/// Validador para ForgotPasswordDto usando FluentValidation.
/// </summary>
public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
{
    public ForgotPasswordDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email é obrigatório")
            .EmailAddress()
            .WithMessage("Formato de email inválido")
            .MaximumLength(254)
            .WithMessage("Email não pode ter mais de 254 caracteres");

        RuleFor(x => x.UrlCallback)
            .Must(BeAValidUrl)
            .WithMessage("URL de callback inválida")
            .When(x => !string.IsNullOrEmpty(x.UrlCallback));
    }

    private static bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}

/// <summary>
/// Validador para ResetPasswordDto usando FluentValidation.
/// </summary>
public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email é obrigatório")
            .EmailAddress()
            .WithMessage("Formato de email inválido");

        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token é obrigatório")
            .MinimumLength(10)
            .WithMessage("Token inválido");

        RuleFor(x => x.NovaSenha)
            .NotEmpty()
            .WithMessage("Nova senha é obrigatória")
            .MinimumLength(8)
            .WithMessage("Nova senha deve ter pelo menos 8 caracteres")
            .MaximumLength(128)
            .WithMessage("Nova senha não pode ter mais de 128 caracteres")
            .Matches(@"[A-Z]")
            .WithMessage("Nova senha deve conter pelo menos uma letra maiúscula")
            .Matches(@"[a-z]")
            .WithMessage("Nova senha deve conter pelo menos uma letra minúscula")
            .Matches(@"[0-9]")
            .WithMessage("Nova senha deve conter pelo menos um número")
            .Matches(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]")
            .WithMessage("Nova senha deve conter pelo menos um caractere especial");

        RuleFor(x => x.ConfirmarSenha)
            .NotEmpty()
            .WithMessage("Confirmação da senha é obrigatória")
            .Equal(x => x.NovaSenha)
            .WithMessage("Confirmação da senha não confere");
    }
}