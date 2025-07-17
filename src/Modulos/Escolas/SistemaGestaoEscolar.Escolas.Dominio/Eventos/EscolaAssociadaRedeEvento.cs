using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Escolas.Dominio.Eventos;

public class EscolaAssociadaRedeEvento : IDomainEvent
{
    public Guid EscolaId { get; }
    public Guid RedeEscolarId { get; }
    public DateTime OcorridoEm { get; }

    public EscolaAssociadaRedeEvento(Guid escolaId, Guid redeEscolarId)
    {
        EscolaId = escolaId;
        RedeEscolarId = redeEscolarId;
        OcorridoEm = DateTime.UtcNow;
    }
}