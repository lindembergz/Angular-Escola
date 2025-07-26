using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries;

public class ObterHorarioDetalheQuery : IRequest<HorarioReadDto?>
{
    public Guid Id { get; set; }
}