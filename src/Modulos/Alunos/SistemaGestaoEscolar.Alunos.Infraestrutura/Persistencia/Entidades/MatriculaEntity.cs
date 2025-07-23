using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestaoEscolar.Alunos.Infraestrutura.Persistencia.Entidades;

[Table("Matriculas")]
public class MatriculaEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Guid AlunoId { get; set; }
    
    [Required]
    public Guid TurmaId { get; set; }
    
    [Required]
    public int AnoLetivo { get; set; }
    
    [Required]
    public DateTime DataMatricula { get; set; }
    
    public DateTime? DataCancelamento { get; set; }
    
    [MaxLength(500)]
    public string? MotivoCancelamento { get; set; }
    
    [Required]
    public bool Ativa { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string NumeroMatricula { get; set; } = string.Empty;
    
    [Required]
    public int Status { get; set; } // StatusMatricula enum
    
    [MaxLength(1000)]
    public string? Observacoes { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    // Relacionamento
    [ForeignKey("AlunoId")]
    public virtual AlunoEntity Aluno { get; set; } = null!;
}