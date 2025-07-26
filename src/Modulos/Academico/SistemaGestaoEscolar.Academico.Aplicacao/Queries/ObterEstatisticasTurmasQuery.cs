using MediatR;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries;

public class ObterEstatisticasTurmasQuery : IRequest<Dictionary<string, int>>
{
    public Guid UnidadeEscolarId { get; set; }
}