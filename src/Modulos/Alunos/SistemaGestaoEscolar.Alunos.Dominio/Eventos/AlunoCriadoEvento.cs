using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Alunos.Dominio.Eventos;

public class AlunoCriadoEvento : IDomainEvent
{
    public Guid AlunoId { get; }
    public string Nome { get; }
    public string Cpf { get; }
    public Guid EscolaId { get; }
    public DateTime OcorridoEm { get; }

    public AlunoCriadoEvento(Guid alunoId, string nome, string cpf, Guid escolaId)
    {
        AlunoId = alunoId;
        Nome = nome;
        Cpf = cpf;
        EscolaId = escolaId;
        OcorridoEm = DateTime.UtcNow;
    }
}