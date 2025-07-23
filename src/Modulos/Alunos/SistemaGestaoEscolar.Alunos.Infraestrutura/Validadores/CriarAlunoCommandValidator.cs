using FluentValidation;
using SistemaGestaoEscolar.Alunos.Aplicacao.Commands;

namespace SistemaGestaoEscolar.Alunos.Infraestrutura.Validadores;

public class CriarAlunoCommandValidator : AbstractValidator<CriarAlunoCommand>
{
    public CriarAlunoCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .Length(2, 100).WithMessage("Nome deve ter entre 2 e 100 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Nome deve conter apenas letras e espaços");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF é obrigatório")
            .Matches(@"^\d{11}$|^\d{3}\.\d{3}\.\d{3}-\d{2}$").WithMessage("CPF deve ter formato válido");

        RuleFor(x => x.DataNascimento)
            .NotEmpty().WithMessage("Data de nascimento é obrigatória")
            .LessThan(DateTime.Today).WithMessage("Data de nascimento não pode ser futura")
            .GreaterThan(DateTime.Today.AddYears(-120)).WithMessage("Data de nascimento não pode ser anterior a 120 anos");

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

        RuleFor(x => x.EscolaId)
            .NotEmpty().WithMessage("Escola é obrigatória");

        RuleFor(x => x.Responsaveis)
            .NotEmpty().WithMessage("Pelo menos um responsável é obrigatório")
            .Must(x => x.Count <= 3).WithMessage("Máximo de 3 responsáveis permitidos")
            .Must(x => x.Any(r => r.ResponsavelFinanceiro)).WithMessage("Pelo menos um responsável deve ser financeiro");

        RuleForEach(x => x.Responsaveis).SetValidator(new CriarResponsavelDtoValidator());
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

public class CriarResponsavelDtoValidator : AbstractValidator<CriarResponsavelDto>
{
    public CriarResponsavelDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome do responsável é obrigatório")
            .Length(2, 100).WithMessage("Nome deve ter entre 2 e 100 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Nome deve conter apenas letras e espaços");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF do responsável é obrigatório")
            .Matches(@"^\d{11}$|^\d{3}\.\d{3}\.\d{3}-\d{2}$").WithMessage("CPF deve ter formato válido");

        RuleFor(x => x.Telefone)
            .NotEmpty().WithMessage("Telefone do responsável é obrigatório")
            .Matches(@"^[\d\s\(\)\-\+]+$").WithMessage("Telefone deve conter apenas números e símbolos válidos");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email deve ter formato válido")
            .MaximumLength(200).WithMessage("Email não pode ter mais de 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Tipo)
            .IsInEnum().WithMessage("Tipo de responsável inválido");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Responsável financeiro deve ter email")
            .When(x => x.ResponsavelFinanceiro);

        // Validações condicionais para endereço do responsável
        RuleFor(x => x.Numero)
            .NotEmpty().WithMessage("Número é obrigatório quando logradouro é informado")
            .When(x => !string.IsNullOrEmpty(x.Logradouro));

        RuleFor(x => x.Bairro)
            .NotEmpty().WithMessage("Bairro é obrigatório quando logradouro é informado")
            .When(x => !string.IsNullOrEmpty(x.Logradouro));

        RuleFor(x => x.Cidade)
            .NotEmpty().WithMessage("Cidade é obrigatória quando logradouro é informado")
            .When(x => !string.IsNullOrEmpty(x.Logradouro));

        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("Estado é obrigatório quando logradouro é informado")
            .Length(2).WithMessage("Estado deve ter 2 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Logradouro));

        RuleFor(x => x.Cep)
            .NotEmpty().WithMessage("CEP é obrigatório quando logradouro é informado")
            .Matches(@"^\d{8}$|^\d{5}-\d{3}$").WithMessage("CEP deve ter formato válido")
            .When(x => !string.IsNullOrEmpty(x.Logradouro));
    }
}