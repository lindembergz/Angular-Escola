using FluentValidation;
using SistemaGestaoEscolar.Alunos.Aplicacao.Commands;

namespace SistemaGestaoEscolar.Alunos.Infraestrutura.Validadores;

public class AtualizarAlunoCommandValidator : AbstractValidator<AtualizarAlunoCommand>
{
    public AtualizarAlunoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID do aluno é obrigatório");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .Length(2, 100).WithMessage("Nome deve ter entre 2 e 100 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Nome deve conter apenas letras e espaços");

        RuleFor(x => x.Logradouro)
            .NotEmpty().WithMessage("Logradouro é obrigatório")
            .MaximumLength(200).WithMessage("Logradouro não pode ter mais de 200 caracteres");

        RuleFor(x => x.Numero)
            .NotEmpty().WithMessage("Número é obrigatório")
            .MaximumLength(20).WithMessage("Número não pode ter mais de 20 caracteres");

        RuleFor(x => x.Bairro)
            .NotEmpty().WithMessage("Bairro é obrigatório")
            .MaximumLength(100).WithMessage("Bairro não pode ter mais de 100 caracteres");

        RuleFor(x => x.Cidade)
            .NotEmpty().WithMessage("Cidade é obrigatória")
            .MaximumLength(100).WithMessage("Cidade não pode ter mais de 100 caracteres");

        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("Estado é obrigatório")
            .Length(2).WithMessage("Estado deve ter 2 caracteres")
            .Must(BeValidState).WithMessage("Estado inválido");

        RuleFor(x => x.Cep)
            .NotEmpty().WithMessage("CEP é obrigatório")
            .Matches(@"^\d{8}$|^\d{5}-\d{3}$").WithMessage("CEP deve ter formato válido");

        RuleFor(x => x.Telefone)
            .Matches(@"^[\d\s\(\)\-\+]+$").WithMessage("Telefone deve conter apenas números e símbolos válidos")
            .When(x => !string.IsNullOrEmpty(x.Telefone));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email deve ter formato válido")
            .MaximumLength(200).WithMessage("Email não pode ter mais de 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Observacoes)
            .MaximumLength(1000).WithMessage("Observações não podem ter mais de 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observacoes));
    }

    private static bool BeValidState(string estado)
    {
        var estadosValidos = new[]
        {
            "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA",
            "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI", "RJ", "RN",
            "RS", "RO", "RR", "SC", "SP", "SE", "TO"
        };

        return estadosValidos.Contains(estado?.ToUpper());
    }
}