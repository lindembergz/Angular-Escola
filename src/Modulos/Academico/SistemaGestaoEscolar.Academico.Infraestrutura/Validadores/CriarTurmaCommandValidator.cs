using FluentValidation;
using SistemaGestaoEscolar.Academico.Aplicacao.Commands;
using SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Validadores;

public class CriarTurmaCommandValidator : AbstractValidator<CriarTurmaCommand>
{
    public CriarTurmaCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Nome da turma é obrigatório")
            .MaximumLength(100)
            .WithMessage("Nome da turma não pode ter mais de 100 caracteres");

        RuleFor(x => x.TipoSerie)
            .IsInEnum()
            .WithMessage("Tipo de série inválido");

        RuleFor(x => x.AnoSerie)
            .GreaterThan(0)
            .WithMessage("Ano da série deve ser maior que zero")
            .Must((command, anoSerie) => ValidarAnoSerie(command.TipoSerie, anoSerie))
            .WithMessage("Ano da série inválido para o tipo de série selecionado");

        RuleFor(x => x.TipoTurno)
            .IsInEnum()
            .WithMessage("Tipo de turno inválido");

        RuleFor(x => x.HoraInicioTurno)
            .NotEmpty()
            .WithMessage("Hora de início do turno é obrigatória");

        RuleFor(x => x.HoraFimTurno)
            .NotEmpty()
            .WithMessage("Hora de fim do turno é obrigatória")
            .Must((command, horaFim) => horaFim > command.HoraInicioTurno)
            .WithMessage("Hora de fim deve ser posterior à hora de início");

        RuleFor(x => x.CapacidadeMaxima)
            .GreaterThan(0)
            .WithMessage("Capacidade máxima deve ser maior que zero")
            .LessThanOrEqualTo(50)
            .WithMessage("Capacidade máxima não pode ser superior a 50 alunos");

        RuleFor(x => x.UnidadeEscolarId)
            .NotEmpty()
            .WithMessage("ID da unidade escolar é obrigatório");

        // Validação de horário de turno baseado no tipo
        RuleFor(x => x)
            .Must(ValidarHorarioTurno)
            .WithMessage("Horário do turno não é compatível com o tipo de turno selecionado");
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

    private static bool ValidarHorarioTurno(CriarTurmaCommand command)
    {
        return command.TipoTurno switch
        {
            TipoTurno.Matutino => command.HoraInicioTurno >= new TimeOnly(6, 0) && 
                                 command.HoraFimTurno <= new TimeOnly(12, 30),
            TipoTurno.Vespertino => command.HoraInicioTurno >= new TimeOnly(12, 0) && 
                                   command.HoraFimTurno <= new TimeOnly(18, 30),
            TipoTurno.Noturno => command.HoraInicioTurno >= new TimeOnly(18, 0) && 
                                command.HoraFimTurno <= new TimeOnly(23, 30),
            TipoTurno.Integral => command.HoraInicioTurno >= new TimeOnly(6, 0) && 
                                 command.HoraFimTurno <= new TimeOnly(18, 0) &&
                                 (command.HoraFimTurno - command.HoraInicioTurno).TotalHours >= 8,
            _ => false
        };
    }
}