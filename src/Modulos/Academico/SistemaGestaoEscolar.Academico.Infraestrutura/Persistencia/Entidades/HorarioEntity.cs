using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

[Table("horarios")]
public class HorarioEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Guid TurmaId { get; set; }
    
    [Required]
    public Guid DisciplinaId { get; set; }
    
    [Required]
    public Guid ProfessorId { get; set; }
    
    [Required]
    public int DiaSemana { get; set; }
    
    [Required]
    public TimeOnly HoraInicio { get; set; }
    
    [Required]
    public TimeOnly HoraFim { get; set; }
    
    [MaxLength(50)]
    public string? Sala { get; set; }
    
    [Required]
    public int AnoLetivo { get; set; }
    
    [Required]
    public int Semestre { get; set; }
    
    [Required]
    public bool Ativo { get; set; } = true;
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(TurmaId))]
    public virtual TurmaEntity? Turma { get; set; }
    
    [ForeignKey(nameof(DisciplinaId))]
    public virtual DisciplinaEntity? Disciplina { get; set; }
}