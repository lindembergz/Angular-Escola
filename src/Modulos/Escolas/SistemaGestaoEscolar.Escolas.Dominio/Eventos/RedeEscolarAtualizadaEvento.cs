using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Escolas.Dominio.Eventos;

public class RedeEscolarAtualizadaEvento : IDomainEvent
{
    public Guid RedeEscolarId { get; }
    public string CampoAlterado { get; }
    public string ValorAnterior { get; }
    public string NovoValor { get; }
    public DateTime OcorridoEm { get; }

    public RedeEscolarAtualizadaEvento(Guid redeEscolarId, string valorAnterior, string novoValor, string campoAlterado = "Nome")
    {
        RedeEscolarId = redeEscolarId;
        CampoAlterado = campoAlterado;
        ValorAnterior = valorAnterior;
        NovoValor = novoValor;
        OcorridoEm = DateTime.UtcNow;
    }
}