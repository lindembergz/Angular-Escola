using MediatR;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries;

public class ObterEstatisticasDisciplinasQuery : IRequest<Dictionary<string, int>>
{
    public Guid EscolaId { get; set; }
}