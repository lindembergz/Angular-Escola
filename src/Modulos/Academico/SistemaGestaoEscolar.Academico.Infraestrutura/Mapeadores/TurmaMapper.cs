using SistemaGestaoEscolar.Academico.Dominio.Entidades;
using SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Mapeadores;

public static class TurmaMapper
{
    public static Turma ToDomain(TurmaEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var nomeTurma = NomeTurma.Criar(entity.Nome);
        var serie = Serie.Criar((TipoSerie)entity.TipoSerie, entity.AnoSerie);
        var turno = Turno.Criar((TipoTurno)entity.TipoTurno, entity.HoraInicioTurno, entity.HoraFimTurno);

        var turma = Turma.Criar(nomeTurma, serie, turno, entity.CapacidadeMaxima, 
                               entity.AnoLetivo, entity.EscolaId);

        // Set private fields using reflection or create a method in domain
        SetPrivateProperty(turma, "Id", entity.Id);
        SetPrivateProperty(turma, "Ativa", entity.Ativa);
        SetPrivateProperty(turma, "CreatedAt", entity.CreatedAt);
        SetPrivateProperty(turma, "UpdatedAt", entity.UpdatedAt ?? DateTime.UtcNow);

        // Add matriculated students
        foreach (var turmaAluno in entity.TurmaAlunos.Where(ta => ta.Ativa))
        {
            turma.MatricularAluno(turmaAluno.AlunoId);
        }

        return turma;
    }

    public static TurmaEntity ToEntity(Turma turma)
    {
        if (turma == null) throw new ArgumentNullException(nameof(turma));

        return new TurmaEntity
        {
            Id = turma.Id,
            Nome = turma.Nome.Valor,
            TipoSerie = (int)turma.Serie.Tipo,
            AnoSerie = turma.Serie.Ano,
            TipoTurno = (int)turma.Turno.Tipo,
            HoraInicioTurno = turma.Turno.HorarioInicio,
            HoraFimTurno = turma.Turno.HorarioFim,
            CapacidadeMaxima = turma.CapacidadeMaxima,
            AnoLetivo = turma.AnoLetivo,
            EscolaId = turma.EscolaId,
            Ativa = turma.Ativa,
            CreatedAt = turma.CreatedAt,
            UpdatedAt = turma.UpdatedAt
        };
    }

    public static void UpdateEntity(TurmaEntity entity, Turma turma)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        if (turma == null) throw new ArgumentNullException(nameof(turma));

        entity.Nome = turma.Nome.Valor;
        entity.TipoSerie = (int)turma.Serie.Tipo;
        entity.AnoSerie = turma.Serie.Ano;
        entity.TipoTurno = (int)turma.Turno.Tipo;
        entity.HoraInicioTurno = turma.Turno.HorarioInicio;
        entity.HoraFimTurno = turma.Turno.HorarioFim;
        entity.CapacidadeMaxima = turma.CapacidadeMaxima;
        entity.AnoLetivo = turma.AnoLetivo;
        entity.EscolaId = turma.EscolaId;
        entity.Ativa = turma.Ativa;
        entity.UpdatedAt = turma.UpdatedAt;
    }

    private static void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName, 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        property?.SetValue(obj, value);
    }
}