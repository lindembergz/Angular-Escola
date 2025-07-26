using FluentValidation;
using SistemaGestaoEscolar.Professores.Aplicacao.Commands;

namespace SistemaGestaoEscolar.Professores.Infraestrutura.Validadores;

public class AtribuirDisciplinaCommandValidator : AbstractValidator<AtribuirDisciplinaCommand>
{
    public AtribuirDisciplinaCommandValidator()
    {
        RuleFor(x => x.ProfessorId)
            .NotEmpty()
            .WithMessage("ID do professor é obrigatório");

        RuleFor(x => x.DisciplinaId)
            .NotEmpty()
            .WithMessage("ID da disciplina é obrigatório");

        RuleFor(x => x.CargaHorariaSemanal)
            .GreaterThan(0)
            .WithMessage("Carga horária semanal deve ser maior que zero")
            .LessThanOrEqualTo(40)
            .WithMessage("Carga horária semanal não pode exceder 40 horas");
    }
}

public class RemoverDisciplinaCommandValidator : AbstractValidator<RemoverDisciplinaCommand>
{
    public RemoverDisciplinaCommandValidator()
    {
        RuleFor(x => x.ProfessorId)
            .NotEmpty()
            .WithMessage("ID do professor é obrigatório");

        RuleFor(x => x.DisciplinaId)
            .NotEmpty()
            .WithMessage("ID da disciplina é obrigatório");
    }
}

public class DesativarProfessorCommandValidator : AbstractValidator<DesativarProfessorCommand>
{
    public DesativarProfessorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ID do professor é obrigatório");

        RuleFor(x => x.Motivo)
            .NotEmpty()
            .WithMessage("Motivo da desativação é obrigatório")
            .MaximumLength(200)
            .WithMessage("Motivo não pode ter mais de 200 caracteres");
    }
}