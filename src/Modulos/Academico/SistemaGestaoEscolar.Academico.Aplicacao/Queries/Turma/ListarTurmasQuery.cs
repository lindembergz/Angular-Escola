using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries.Turma;

public record ListarTurmasQuery(
    Guid? EscolaId = null,
    int? AnoLetivo = null,
    bool? Ativa = null
) : IRequest<List<TurmaReadDto>>;