namespace SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

public record TurmaCreateDto(
    string Nome,
    string Serie,
    string Turno,
    int CapacidadeMaxima,
    int AnoLetivo,
    Guid EscolaId
);

public record TurmaUpdateDto(
    string Nome,
    int CapacidadeMaxima
);

public record TurmaReadDto(
    Guid Id,
    string Nome,
    string Serie,
    string Turno,
    int CapacidadeMaxima,
    int AnoLetivo,
    Guid EscolaId,
    bool Ativa,
    int AlunosMatriculados,
    int VagasDisponiveis,
    DateTime DataCriacao
);