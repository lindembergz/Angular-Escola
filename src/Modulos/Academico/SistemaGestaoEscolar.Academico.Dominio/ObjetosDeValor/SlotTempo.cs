using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;

public class SlotTempo : ValueObject
{
    public TimeOnly HorarioInicio { get; private set; }
    public TimeOnly HorarioFim { get; private set; }
    public DayOfWeek DiaSemana { get; private set; }
    public int DuracaoMinutos { get; private set; }

    private SlotTempo(TimeOnly horarioInicio, TimeOnly horarioFim, DayOfWeek diaSemana)
    {
        HorarioInicio = horarioInicio;
        HorarioFim = horarioFim;
        DiaSemana = diaSemana;
        DuracaoMinutos = CalcularDuracaoMinutos(horarioInicio, horarioFim);
    }

    public static SlotTempo Criar(TimeOnly horarioInicio, TimeOnly horarioFim, DayOfWeek diaSemana)
    {
        if (horarioInicio >= horarioFim)
            throw new ArgumentException("Horário de início deve ser anterior ao horário de fim");

        if (diaSemana == DayOfWeek.Sunday)
            throw new ArgumentException("Não é permitido agendar aulas aos domingos");

        var duracao = CalcularDuracaoMinutos(horarioInicio, horarioFim);
        if (duracao < 30)
            throw new ArgumentException("Duração mínima de uma aula é 30 minutos");

        if (duracao > 240)
            throw new ArgumentException("Duração máxima de uma aula é 4 horas");

        return new SlotTempo(horarioInicio, horarioFim, diaSemana);
    }

    private static int CalcularDuracaoMinutos(TimeOnly inicio, TimeOnly fim)
    {
        return (int)(fim - inicio).TotalMinutes;
    }

    public bool ConflitaCom(SlotTempo outro)
    {
        if (DiaSemana != outro.DiaSemana)
            return false;

        return HorarioInicio < outro.HorarioFim && HorarioFim > outro.HorarioInicio;
    }

    public string ObterDescricaoDiaSemana()
    {
        return DiaSemana switch
        {
            DayOfWeek.Monday => "Segunda-feira",
            DayOfWeek.Tuesday => "Terça-feira",
            DayOfWeek.Wednesday => "Quarta-feira",
            DayOfWeek.Thursday => "Quinta-feira",
            DayOfWeek.Friday => "Sexta-feira",
            DayOfWeek.Saturday => "Sábado",
            _ => "Dia inválido"
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return HorarioInicio;
        yield return HorarioFim;
        yield return DiaSemana;
    }

    public override string ToString() => 
        $"{ObterDescricaoDiaSemana()} {HorarioInicio:HH:mm} - {HorarioFim:HH:mm}";
}