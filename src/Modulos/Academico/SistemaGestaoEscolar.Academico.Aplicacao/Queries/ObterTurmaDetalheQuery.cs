using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries;

public class ObterTurmaDetalheQuery : IRequest<TurmaReadDto?>
{
    public Guid Id { get; set; }
}