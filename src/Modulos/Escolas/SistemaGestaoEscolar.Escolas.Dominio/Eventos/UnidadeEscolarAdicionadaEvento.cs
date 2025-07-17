using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Escolas.Dominio.Eventos;

public class UnidadeEscolarAdicionadaEvento : IDomainEvent
{
    public Guid EscolaId { get; }
    public Guid UnidadeId { get; }
    public string NomeUnidade { get; }
    public DateTime OcorridoEm { get; }

    public UnidadeEscolarAdicionadaEvento(Guid escolaId, Guid unidadeId, string nomeUnidade)
    {
        EscolaId = escolaId;
        UnidadeId = unidadeId;
        NomeUnidade = nomeUnidade;
        OcorridoEm = DateTime.UtcNow;
    }
}