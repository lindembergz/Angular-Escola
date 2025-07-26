using SistemaGestaoEscolar.Academico.Dominio.Entidades;
using SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Mapeadores;

public static class HorarioMapper
{
    public static Horario ToDomain(HorarioEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var slotTempo = SlotTempo.Criar(entity.HoraInicio, entity.HoraFim, (DayOfWeek)entity.DiaSemana);

        var horario = Horario.Criar(entity.TurmaId, entity.DisciplinaId, entity.ProfessorId,
                                   slotTempo, entity.AnoLetivo, entity.Semestre, entity.Sala);

        // Set private fields using reflection
        SetPrivateProperty(horario, "Id", entity.Id);
        SetPrivateProperty(horario, "Ativo", entity.Ativo);
        SetPrivateProperty(horario, "CreatedAt", entity.CreatedAt);
        SetPrivateProperty(horario, "UpdatedAt", entity.UpdatedAt ?? DateTime.UtcNow);

        return horario;
    }

    public static HorarioEntity ToEntity(Horario horario)
    {
        if (horario == null) throw new ArgumentNullException(nameof(horario));

        return new HorarioEntity
        {
            Id = horario.Id,
            TurmaId = horario.TurmaId,
            DisciplinaId = horario.DisciplinaId,
            ProfessorId = horario.ProfessorId,
            DiaSemana = (int)horario.SlotTempo.DiaSemana,
            HoraInicio = horario.SlotTempo.HorarioInicio,
            HoraFim = horario.SlotTempo.HorarioFim,
            Sala = horario.Sala,
            AnoLetivo = horario.AnoLetivo,
            Semestre = horario.Semestre,
            Ativo = horario.Ativo,
            CreatedAt = horario.CreatedAt,
            UpdatedAt = horario.UpdatedAt
        };
    }

    public static void UpdateEntity(HorarioEntity entity, Horario horario)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        if (horario == null) throw new ArgumentNullException(nameof(horario));

        entity.TurmaId = horario.TurmaId;
        entity.DisciplinaId = horario.DisciplinaId;
        entity.ProfessorId = horario.ProfessorId;
        entity.DiaSemana = (int)horario.SlotTempo.DiaSemana;
        entity.HoraInicio = horario.SlotTempo.HorarioInicio;
        entity.HoraFim = horario.SlotTempo.HorarioFim;
        entity.Sala = horario.Sala;
        entity.AnoLetivo = horario.AnoLetivo;
        entity.Semestre = horario.Semestre;
        entity.Ativo = horario.Ativo;
        entity.UpdatedAt = horario.UpdatedAt;
    }

    private static void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName, 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        property?.SetValue(obj, value);
    }
}