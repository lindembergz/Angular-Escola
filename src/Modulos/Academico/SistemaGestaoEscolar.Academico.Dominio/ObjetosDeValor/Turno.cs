using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;

public class Turno : ValueObject
{
    public TipoTurno Tipo { get; private set; }
    public TimeOnly HorarioInicio { get; private set; }
    public TimeOnly HorarioFim { get; private set; }
    public string Descricao { get; private set; }

    private Turno(TipoTurno tipo, TimeOnly horarioInicio, TimeOnly horarioFim, string descricao)
    {
        Tipo = tipo;
        HorarioInicio = horarioInicio;
        HorarioFim = horarioFim;
        Descricao = descricao;
    }

    public static Turno Criar(TipoTurno tipo, TimeOnly horarioInicio, TimeOnly horarioFim)
    {
        if (horarioInicio >= horarioFim)
            throw new ArgumentException("Horário de início deve ser anterior ao horário de fim");

        var descricao = ObterDescricao(tipo);
        return new Turno(tipo, horarioInicio, horarioFim, descricao);
    }

    public static Turno Matutino() => Criar(TipoTurno.Matutino, new TimeOnly(7, 0), new TimeOnly(12, 0));
    public static Turno Vespertino() => Criar(TipoTurno.Vespertino, new TimeOnly(13, 0), new TimeOnly(18, 0));
    public static Turno Noturno() => Criar(TipoTurno.Noturno, new TimeOnly(19, 0), new TimeOnly(23, 0));
    public static Turno Integral() => Criar(TipoTurno.Integral, new TimeOnly(7, 0), new TimeOnly(17, 0));

    private static string ObterDescricao(TipoTurno tipo)
    {
        return tipo switch
        {
            TipoTurno.Matutino => "Matutino",
            TipoTurno.Vespertino => "Vespertino",
            TipoTurno.Noturno => "Noturno",
            TipoTurno.Integral => "Integral",
            _ => throw new ArgumentException("Tipo de turno inválido")
        };
    }

    public bool ContemHorario(TimeOnly horario)
    {
        return horario >= HorarioInicio && horario <= HorarioFim;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Tipo;
        yield return HorarioInicio;
        yield return HorarioFim;
    }

    public override string ToString() => $"{Descricao} ({HorarioInicio:HH:mm} - {HorarioFim:HH:mm})";
}

public enum TipoTurno
{
    Matutino = 1,
    Vespertino = 2,
    Noturno = 3,
    Integral = 4
}