using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

[Table("disciplina_pre_requisitos")]
public class DisciplinaPreRequisitoEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Guid DisciplinaId { get; set; }
    
    [Required]
    public Guid PreRequisitoId { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey(nameof(DisciplinaId))]
    public virtual DisciplinaEntity? Disciplina { get; set; }
    
    [ForeignKey(nameof(PreRequisitoId))]
    public virtual DisciplinaEntity? PreRequisito { get; set; }
}