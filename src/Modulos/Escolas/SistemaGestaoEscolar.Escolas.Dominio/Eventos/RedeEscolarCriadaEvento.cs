using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Escolas.Dominio.Eventos;

public class RedeEscolarCriadaEvento : IDomainEvent
{
    public Guid RedeEscolarId { get; }
    public string NomeRede { get; }
    public string CnpjMantenedora { get; }
    public DateTime OcorridoEm { get; }

    public RedeEscolarCriadaEvento(Guid redeEscolarId, string nomeRede, string cnpjMantenedora)
    {
        RedeEscolarId = redeEscolarId;
        NomeRede = nomeRede;
        CnpjMantenedora = cnpjMantenedora;
        OcorridoEm = DateTime.UtcNow;
    }
}