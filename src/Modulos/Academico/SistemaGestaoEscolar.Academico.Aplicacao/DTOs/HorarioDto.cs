namespace SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

public record HorarioCreateDto(
    Guid TurmaId,
    Guid DisciplinaId,
    Guid ProfessorId,
    string DiaSemana,
    TimeSpan HoraInicio,
    TimeSpan HoraFim,
    int AnoLetivo,
    int Semestre,
    string? Sala = null
);

public record HorarioUpdateDto(
    Guid ProfessorId,
    string? Sala = null
);

public record HorarioReadDto(
    Guid Id,
    Guid TurmaId,
    Guid DisciplinaId,
    Guid ProfessorId,
    string DiaSemana,
    TimeSpan HoraInicio,
    TimeSpan HoraFim,
    string? Sala,
    int AnoLetivo,
    int Semestre,
    bool Ativo,
    DateTime DataCriacao
);