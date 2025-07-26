using MediatR;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries;

public class ObterEstatisticasHorariosQuery : IRequest<Dictionary<string, int>>
{
    public Guid EscolaId { get; set; }
    public int AnoLetivo { get; set; }
    public int Semestre { get; set; }
}