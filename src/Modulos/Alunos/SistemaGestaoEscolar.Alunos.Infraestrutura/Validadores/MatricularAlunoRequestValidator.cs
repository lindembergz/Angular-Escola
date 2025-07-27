using FluentValidation;
using SistemaGestaoEscolar.Alunos.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Alunos.Infraestrutura.Validadores;

/// <summary>
/// Validador para MatricularAlunoRequest DTO
/// </summary>
public class MatricularAlunoRequestValidator : AbstractValidator<MatricularAlunoRequest>
{
    public MatricularAlunoRequestValidator()
    {
        RuleFor(x => x.TurmaId)
            .NotEmpty().WithMessage("ID da turma é obrigatório")
            .Must(BeValidGuid).WithMessage("ID da turma deve ser um GUID válido");

        RuleFor(x => x.AnoLetivo)
            .GreaterThan(0).WithMessage("Ano letivo deve ser maior que zero")
            .Must(BeValidSchoolYear).WithMessage("Ano letivo deve estar entre o ano anterior e o próximo ano");

        RuleFor(x => x.Observacoes)
            .MaximumLength(1000).WithMessage("Observações não podem ter mais de 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observacoes));
    }

    private static bool BeValidGuid(string turmaId)
    {
        return Guid.TryParse(turmaId, out _);
    }

    private static bool BeValidSchoolYear(int anoLetivo)
    {
        var anoAtual = DateTime.Now.Year;
        return anoLetivo >= anoAtual - 1 && anoLetivo <= anoAtual + 1;
    }
}