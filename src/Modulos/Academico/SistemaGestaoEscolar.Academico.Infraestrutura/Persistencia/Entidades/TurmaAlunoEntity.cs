using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

[Table("turma_alunos")]
public class TurmaAlunoEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Guid TurmaId { get; set; }
    
    [Required]
    public Guid AlunoId { get; set; }
    
    [Required]
    public DateTime DataMatricula { get; set; } = DateTime.UtcNow;
    
    public DateTime? DataDesmatricula { get; set; }
    
    [Required]
    public bool Ativa { get; set; } = true;
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(TurmaId))]
    public virtual TurmaEntity? Turma { get; set; }
}