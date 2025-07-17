using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Escolas.Dominio.Eventos;

public class RedeEscolarAtivadaEvento : IDomainEvent
{
    public Guid RedeEscolarId { get; }
    public string NomeRede { get; }
    public DateTime OcorridoEm { get; }

    public RedeEscolarAtivadaEvento(Guid redeEscolarId, string nomeRede)
    {
        RedeEscolarId = redeEscolarId;
        NomeRede = nomeRede;
        OcorridoEm = DateTime.UtcNow;
    }
}