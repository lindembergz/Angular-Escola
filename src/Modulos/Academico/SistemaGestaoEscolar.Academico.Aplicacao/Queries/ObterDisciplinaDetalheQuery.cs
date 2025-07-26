using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries;

public class ObterDisciplinaDetalheQuery : IRequest<DisciplinaReadDto?>
{
    public Guid Id { get; set; }
}