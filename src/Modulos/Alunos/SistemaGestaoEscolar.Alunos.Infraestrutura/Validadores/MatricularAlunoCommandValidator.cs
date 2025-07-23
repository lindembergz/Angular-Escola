using FluentValidation;
using SistemaGestaoEscolar.Alunos.Aplicacao.Commands;

namespace SistemaGestaoEscolar.Alunos.Infraestrutura.Validadores;

public class MatricularAlunoCommandValidator : AbstractValidator<MatricularAlunoCommand>
{
    public MatricularAlunoCommandValidator()
    {
        RuleFor(x => x.AlunoId)
            .NotEmpty().WithMessage("ID do aluno é obrigatório");

        RuleFor(x => x.TurmaId)
            .NotEmpty().WithMessage("ID da turma é obrigatório");

        RuleFor(x => x.AnoLetivo)
            .GreaterThan(0).WithMessage("Ano letivo deve ser maior que zero")
            .Must(BeValidSchoolYear).WithMessage("Ano letivo deve estar entre o ano anterior e o próximo ano");

        RuleFor(x => x.Observacoes)
            .MaximumLength(1000).WithMessage("Observações não podem ter mais de 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observacoes));
    }

    private static bool BeValidSchoolYear(int anoLetivo)
    {
        var anoAtual = DateTime.Now.Year;
        return anoLetivo >= anoAtual - 1 && anoLetivo <= anoAtual + 1;
    }
}

public class TransferirAlunoCommandValidator : AbstractValidator<TransferirAlunoCommand>
{
    public TransferirAlunoCommandValidator()
    {
        RuleFor(x => x.AlunoId)
            .NotEmpty().WithMessage("ID do aluno é obrigatório");

        RuleFor(x => x.NovaEscolaId)
            .NotEmpty().WithMessage("ID da nova escola é obrigatório");

        RuleFor(x => x.Motivo)
            .NotEmpty().WithMessage("Motivo da transferência é obrigatório")
            .MaximumLength(500).WithMessage("Motivo não pode ter mais de 500 caracteres");

        RuleFor(x => x.DataTransferencia)
            .LessThanOrEqualTo(DateTime.Today.AddDays(30)).WithMessage("Data de transferência não pode ser muito futura")
            .When(x => x.DataTransferencia.HasValue);

        RuleFor(x => x.Observacoes)
            .MaximumLength(1000).WithMessage("Observações não podem ter mais de 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observacoes));
    }
}