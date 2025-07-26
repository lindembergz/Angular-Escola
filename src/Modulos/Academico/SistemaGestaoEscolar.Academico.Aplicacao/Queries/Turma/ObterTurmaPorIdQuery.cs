using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries.Turma;

public record ObterTurmaPorIdQuery(Guid Id) : IRequest<TurmaReadDto?>;