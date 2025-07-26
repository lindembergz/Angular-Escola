using FluentValidation;
using SistemaGestaoEscolar.Professores.Aplicacao.Commands;
using SistemaGestaoEscolar.Professores.Dominio.ObjetosDeValor;

namespace SistemaGestaoEscolar.Professores.Infraestrutura.Validadores;

public class CriarProfessorCommandValidator : AbstractValidator<CriarProfessorCommand>
{
    public CriarProfessorCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Nome é obrigatório")
            .Length(2, 100)
            .WithMessage("Nome deve ter entre 2 e 100 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("Nome deve conter apenas letras e espaços");

        RuleFor(x => x.Cpf)
            .NotEmpty()
            .WithMessage("CPF é obrigatório")
            .Must(BeValidCpf)
            .WithMessage("CPF inválido");

        RuleFor(x => x.Registro)
            .NotEmpty()
            .WithMessage("Registro é obrigatório")
            .Must(BeValidRegistro)
            .WithMessage("Registro deve ter pelo menos 4 dígitos");

        RuleFor(x => x.DataNascimento)
            .NotEmpty()
            .WithMessage("Data de nascimento é obrigatória")
            .Must(BeValidBirthDate)
            .WithMessage("Professor deve ter pelo menos 18 anos");

        RuleFor(x => x.DataContratacao)
            .NotEmpty()
            .WithMessage("Data de contratação é obrigatória")
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("Data de contratação não pode ser futura")
            .Must((command, dataContratacao) => BeValidHireDate(command.DataNascimento, dataContratacao))
            .WithMessage("Data de contratação deve ser posterior ao 16º aniversário");

        RuleFor(x => x.EscolaId)
            .NotEmpty()
            .WithMessage("Escola é obrigatória");

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

        RuleForEach(x => x.Titulos)
            .SetValidator(new TituloAcademicoValidator())
            .When(x => x.Titulos != null);
    }

    private static bool BeValidCpf(string cpf)
    {
        try
        {
            var cpfObj = new SistemaGestaoEscolar.Shared.Domain.ValueObjects.Cpf(cpf);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool BeValidRegistro(string registro)
    {
        try
        {
            var registroObj = new RegistroProfessor(registro);
            return registroObj.EhValido();
        }
        catch
        {
            return false;
        }
    }

    private static bool BeValidBirthDate(DateTime dataNascimento)
    {
        var idade = DateTime.Today.Year - dataNascimento.Year;
        if (dataNascimento.Date > DateTime.Today.AddYears(-idade))
            idade--;
        return idade >= 18;
    }

    private static bool BeValidHireDate(DateTime dataNascimento, DateTime dataContratacao)
    {
        return dataContratacao >= dataNascimento.AddYears(16);
    }
}

public class TituloAcademicoValidator : AbstractValidator<CriarTituloAcademicoDto>
{
    public TituloAcademicoValidator()
    {
        RuleFor(x => x.Tipo)
            .IsInEnum()
            .WithMessage("Tipo de título inválido");

        RuleFor(x => x.Curso)
            .NotEmpty()
            .WithMessage("Curso é obrigatório")
            .MaximumLength(100)
            .WithMessage("Curso não pode ter mais de 100 caracteres");

        RuleFor(x => x.Instituicao)
            .NotEmpty()
            .WithMessage("Instituição é obrigatória")
            .MaximumLength(100)
            .WithMessage("Instituição não pode ter mais de 100 caracteres");

        RuleFor(x => x.AnoFormatura)
            .GreaterThanOrEqualTo(1950)
            .WithMessage("Ano de formatura deve ser maior ou igual a 1950")
            .LessThanOrEqualTo(DateTime.Now.Year)
            .WithMessage("Ano de formatura não pode ser futuro");
    }
}