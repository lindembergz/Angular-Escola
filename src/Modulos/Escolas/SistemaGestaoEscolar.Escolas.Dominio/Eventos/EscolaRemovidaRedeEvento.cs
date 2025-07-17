using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Escolas.Dominio.Eventos;

public class EscolaRemovidaRedeEvento : IDomainEvent
{
    public Guid RedeEscolarId { get; }
    public Guid EscolaId { get; }
    public string NomeEscola { get; }
    public DateTime OcorridoEm { get; }

    public EscolaRemovidaRedeEvento(Guid redeEscolarId, Guid escolaId, string nomeEscola)
    {
        RedeEscolarId = redeEscolarId;
        EscolaId = escolaId;
        NomeEscola = nomeEscola;
        OcorridoEm = DateTime.UtcNow;
    }
}