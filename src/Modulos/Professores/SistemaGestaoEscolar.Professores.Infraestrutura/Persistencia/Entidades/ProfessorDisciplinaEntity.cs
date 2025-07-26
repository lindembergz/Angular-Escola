namespace SistemaGestaoEscolar.Professores.Infraestrutura.Persistencia.Entidades;

public class ProfessorDisciplinaEntity
{
    public Guid Id { get; set; }
    public Guid ProfessorId { get; set; }
    public Guid DisciplinaId { get; set; }
    public string? Observacoes { get; set; }
    public int CargaHorariaSemanal { get; set; }
    public DateTime DataAtribuicao { get; set; }
    public bool Ativa { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public ProfessorEntity Professor { get; set; } = null!;
}