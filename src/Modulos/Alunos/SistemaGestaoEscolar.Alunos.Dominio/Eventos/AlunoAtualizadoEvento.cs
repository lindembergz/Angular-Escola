using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Alunos.Dominio.Eventos;

public class AlunoAtualizadoEvento : IDomainEvent
{
    public Guid AlunoId { get; }
    public string Campo { get; }
    public string ValorAnterior { get; }
    public string NovoValor { get; }
    public DateTime OcorridoEm { get; }

    public AlunoAtualizadoEvento(Guid alunoId, string campo, string valorAnterior, string novoValor)
    {
        AlunoId = alunoId;
        Campo = campo;
        ValorAnterior = valorAnterior;
        NovoValor = novoValor;
        OcorridoEm = DateTime.UtcNow;
    }
}