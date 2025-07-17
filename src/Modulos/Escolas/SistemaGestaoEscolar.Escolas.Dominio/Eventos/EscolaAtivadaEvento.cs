using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Escolas.Dominio.Eventos;

public class EscolaAtivadaEvento : IDomainEvent
{
    public Guid EscolaId { get; }
    public string NomeEscola { get; }
    public DateTime OcorridoEm { get; }

    public EscolaAtivadaEvento(Guid escolaId, string nomeEscola)
    {
        EscolaId = escolaId;
        NomeEscola = nomeEscola;
        OcorridoEm = DateTime.UtcNow;
    }
}