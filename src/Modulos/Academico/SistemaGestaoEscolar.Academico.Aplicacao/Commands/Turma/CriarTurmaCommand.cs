using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands.Turma;

public record CriarTurmaCommand(
    string Nome,
    string Serie,
    string Turno,
    int CapacidadeMaxima,
    int AnoLetivo,
    Guid EscolaId
) : IRequest<TurmaReadDto>;