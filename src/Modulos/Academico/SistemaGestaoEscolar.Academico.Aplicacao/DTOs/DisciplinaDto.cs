namespace SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

public record DisciplinaCreateDto(
    string Nome,
    string Codigo,
    int CargaHoraria,
    string Serie,
    bool Obrigatoria,
    Guid EscolaId,
    string? Descricao = null
);

public record DisciplinaUpdateDto(
    string Nome,
    int CargaHoraria,
    bool Obrigatoria,
    string? Descricao = null
);

public record DisciplinaReadDto(
    Guid Id,
    string Nome,
    string Codigo,
    int CargaHoraria,
    string Serie,
    bool Obrigatoria,
    string? Descricao,
    Guid EscolaId,
    bool Ativa,
    List<Guid> PreRequisitos,
    DateTime DataCriacao
);