using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Escolas.Dominio.Eventos;

public class EscolaAtualizadaEvento : IDomainEvent
{
    public Guid EscolaId { get; }
    public string CampoAlterado { get; }
    public string ValorAnterior { get; }
    public string NovoValor { get; }
    public DateTime OcorridoEm { get; }

    public EscolaAtualizadaEvento(Guid escolaId, string valorAnterior, string novoValor, string campoAlterado = "Nome")
    {
        EscolaId = escolaId;
        CampoAlterado = campoAlterado;
        ValorAnterior = valorAnterior;
        NovoValor = novoValor;
        OcorridoEm = DateTime.UtcNow;
    }
}