using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Alunos.Dominio.Eventos;

public class AlunoIdadeForaPadraoEvento : IDomainEvent
{
    public Guid AlunoId { get; }
    public int Idade { get; }
    public string FaixaEtariaEscolar { get; }
    public DateTime OcorridoEm { get; }

    public AlunoIdadeForaPadraoEvento(Guid alunoId, int idade, string faixaEtariaEscolar)
    {
        AlunoId = alunoId;
        Idade = idade;
        FaixaEtariaEscolar = faixaEtariaEscolar;
        OcorridoEm = DateTime.UtcNow;
    }
}