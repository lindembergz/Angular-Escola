using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Escolas.Dominio.Eventos;

public class EscolaDesassociadaRedeEvento : IDomainEvent
{
    public Guid EscolaId { get; }
    public Guid RedeEscolarAnteriorId { get; }
    public DateTime OcorridoEm { get; }

    public EscolaDesassociadaRedeEvento(Guid escolaId, Guid redeEscolarAnteriorId)
    {
        EscolaId = escolaId;
        RedeEscolarAnteriorId = redeEscolarAnteriorId;
        OcorridoEm = DateTime.UtcNow;
    }
}