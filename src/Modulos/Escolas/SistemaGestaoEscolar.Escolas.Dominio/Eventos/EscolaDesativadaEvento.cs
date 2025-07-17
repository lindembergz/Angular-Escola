using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Escolas.Dominio.Eventos;

public class EscolaDesativadaEvento : IDomainEvent
{
    public Guid EscolaId { get; }
    public string NomeEscola { get; }
    public DateTime OcorridoEm { get; }

    public EscolaDesativadaEvento(Guid escolaId, string nomeEscola)
    {
        EscolaId = escolaId;
        NomeEscola = nomeEscola;
        OcorridoEm = DateTime.UtcNow;
    }
}