using FluentValidation;
using SistemaGestaoEscolar.Professores.Aplicacao.Commands;

namespace SistemaGestaoEscolar.Professores.Infraestrutura.Validadores;

public class AtualizarProfessorCommandValidator : AbstractValidator<AtualizarProfessorCommand>
{
    public AtualizarProfessorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ID do professor é obrigatório");

        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Nome é obrigatório")
            .Length(2, 100)
            .WithMessage("Nome deve ter entre 2 e 100 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("Nome deve conter apenas letras e espaços");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Email inválido");

        RuleFor(x => x.Telefone)
            .Matches(@"^\(\d{2}\)\s\d{4,5}-\d{4}$")
            .When(x => !string.IsNullOrEmpty(x.Telefone))
            .WithMessage("Telefone deve estar no formato (XX) XXXXX-XXXX");

        RuleFor(x => x.Observacoes)
            .MaximumLength(500)
            .WithMessage("Observações não podem ter mais de 500 caracteres");
    }
}