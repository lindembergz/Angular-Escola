using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Escolas.Dominio.Eventos;

public class EscolaCriadaEvento : IDomainEvent
{
    public Guid EscolaId { get; }
    public string NomeEscola { get; }
    public string CnpjEscola { get; }
    public DateTime OcorridoEm { get; }

    public EscolaCriadaEvento(Guid escolaId, string nomeEscola, string cnpjEscola)
    {
        EscolaId = escolaId;
        NomeEscola = nomeEscola;
        CnpjEscola = cnpjEscola;
        OcorridoEm = DateTime.UtcNow;
    }
}