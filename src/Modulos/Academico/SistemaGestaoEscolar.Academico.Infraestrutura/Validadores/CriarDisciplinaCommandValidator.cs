using FluentValidation;
using SistemaGestaoEscolar.Academico.Aplicacao.Commands;
using SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Validadores;

public class CriarDisciplinaCommandValidator : AbstractValidator<CriarDisciplinaCommand>
{
    public CriarDisciplinaCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Nome da disciplina é obrigatório")
            .MaximumLength(100)
            .WithMessage("Nome da disciplina não pode ter mais de 100 caracteres");

        RuleFor(x => x.Codigo)
            .NotEmpty()
            .WithMessage("Código da disciplina é obrigatório")
            .MaximumLength(20)
            .WithMessage("Código da disciplina não pode ter mais de 20 caracteres")
            .Matches("^[A-Z0-9]+$")
            .WithMessage("Código deve conter apenas letras maiúsculas e números");

        RuleFor(x => x.CargaHoraria)
            .GreaterThan(0)
            .WithMessage("Carga horária deve ser maior que zero")
            .LessThanOrEqualTo(1000)
            .WithMessage("Carga horária não pode ser superior a 1000 horas");

        RuleFor(x => x.TipoSerie)
            .IsInEnum()
            .WithMessage("Tipo de série inválido");

        RuleFor(x => x.AnoSerie)
            .GreaterThan(0)
            .WithMessage("Ano da série deve ser maior que zero")
            .Must((command, anoSerie) => ValidarAnoSerie(command.TipoSerie, anoSerie))
            .WithMessage("Ano da série inválido para o tipo de série selecionado");

        RuleFor(x => x.EscolaId)
            .NotEmpty()
            .WithMessage("ID da escola é obrigatório");

        RuleFor(x => x.Descricao)
            .MaximumLength(500)
            .WithMessage("Descrição não pode ter mais de 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));

        // Validação de carga horária baseada no tipo de série
        RuleFor(x => x)
            .Must(ValidarCargaHorariaPorSerie)
            .WithMessage("Carga horária não é adequada para o tipo de série selecionado");
    }

    private static bool ValidarAnoSerie(TipoSerie tipoSerie, int anoSerie)
    {
        return tipoSerie switch
        {
            TipoSerie.Infantil => anoSerie >= 1 && anoSerie <= 5,
            TipoSerie.Fundamental => anoSerie >= 1 && anoSerie <= 9,
            TipoSerie.Medio => anoSerie >= 1 && anoSerie <= 3,
            _ => false
        };
    }

    private static bool ValidarCargaHorariaPorSerie(CriarDisciplinaCommand command)
    {
        return command.TipoSerie switch
        {
            TipoSerie.Infantil => command.CargaHoraria >= 20 && command.CargaHoraria <= 200,
            TipoSerie.Fundamental => command.CargaHoraria >= 40 && command.CargaHoraria <= 400,
            TipoSerie.Medio => command.CargaHoraria >= 60 && command.CargaHoraria <= 600,
            _ => false
        };
    }
}