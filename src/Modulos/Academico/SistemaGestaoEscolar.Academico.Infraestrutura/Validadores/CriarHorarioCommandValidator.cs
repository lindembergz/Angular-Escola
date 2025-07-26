using FluentValidation;
using SistemaGestaoEscolar.Academico.Aplicacao.Commands;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Validadores;

public class CriarHorarioCommandValidator : AbstractValidator<CriarHorarioCommand>
{
    public CriarHorarioCommandValidator()
    {
        RuleFor(x => x.TurmaId)
            .NotEmpty()
            .WithMessage("ID da turma é obrigatório");

        RuleFor(x => x.DisciplinaId)
            .NotEmpty()
            .WithMessage("ID da disciplina é obrigatório");

        RuleFor(x => x.ProfessorId)
            .NotEmpty()
            .WithMessage("ID do professor é obrigatório");

        RuleFor(x => x.DiaDaSemana)
            .IsInEnum()
            .WithMessage("Dia da semana inválido")
            .NotEqual(DayOfWeek.Sunday)
            .WithMessage("Não é permitido agendar aulas aos domingos");

        RuleFor(x => x.HoraInicio)
            .NotEmpty()
            .WithMessage("Hora de início é obrigatória")
            .Must(BeValidSchoolHour)
            .WithMessage("Hora de início deve estar entre 06:00 e 22:00");

        RuleFor(x => x.HoraFim)
            .NotEmpty()
            .WithMessage("Hora de fim é obrigatória")
            .Must(BeValidSchoolHour)
            .WithMessage("Hora de fim deve estar entre 06:00 e 22:00")
            .Must((command, horaFim) => horaFim > command.HoraInicio)
            .WithMessage("Hora de fim deve ser posterior à hora de início");

        RuleFor(x => x.AnoLetivo)
            .GreaterThanOrEqualTo(DateTime.Now.Year - 1)
            .WithMessage("Ano letivo não pode ser anterior ao ano passado")
            .LessThanOrEqualTo(DateTime.Now.Year + 1)
            .WithMessage("Ano letivo não pode ser superior ao próximo ano");

        RuleFor(x => x.Semestre)
            .InclusiveBetween(1, 2)
            .WithMessage("Semestre deve ser 1 ou 2");

        RuleFor(x => x.Sala)
            .MaximumLength(50)
            .WithMessage("Nome da sala não pode ter mais de 50 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Sala));

        // Validação de duração da aula
        RuleFor(x => x)
            .Must(ValidarDuracaoAula)
            .WithMessage("Duração da aula deve estar entre 30 minutos e 4 horas");

        // Validação de horário escolar
        RuleFor(x => x)
            .Must(ValidarHorarioEscolar)
            .WithMessage("Horário deve estar dentro do período de funcionamento escolar");
    }

    private static bool BeValidSchoolHour(TimeOnly hora)
    {
        return hora >= new TimeOnly(6, 0) && hora <= new TimeOnly(22, 0);
    }

    private static bool ValidarDuracaoAula(CriarHorarioCommand command)
    {
        var duracao = command.HoraFim - command.HoraInicio;
        return duracao.TotalMinutes >= 30 && duracao.TotalMinutes <= 240;
    }

    private static bool ValidarHorarioEscolar(CriarHorarioCommand command)
    {
        // Validar se o horário não ultrapassa os limites típicos de funcionamento escolar
        var inicioEscolar = new TimeOnly(6, 0);
        var fimEscolar = new TimeOnly(22, 0);
        
        return command.HoraInicio >= inicioEscolar && 
               command.HoraFim <= fimEscolar &&
               command.HoraInicio < command.HoraFim;
    }
}