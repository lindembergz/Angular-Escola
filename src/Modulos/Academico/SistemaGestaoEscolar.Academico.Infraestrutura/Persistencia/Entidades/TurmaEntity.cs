using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

[Table("turmas")]
public class TurmaEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    [Required]
    public int TipoSerie { get; set; }
    
    [Required]
    public int AnoSerie { get; set; }
    
    [Required]
    public int TipoTurno { get; set; }
    
    [Required]
    public TimeOnly HoraInicioTurno { get; set; }
    
    [Required]
    public TimeOnly HoraFimTurno { get; set; }
    
    [Required]
    public int CapacidadeMaxima { get; set; }
    
    [Required]
    public int AnoLetivo { get; set; }
    
    [Required]
    public Guid EscolaId { get; set; }
    
    [Required]
    public bool Ativa { get; set; } = true;
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<DisciplinaEntity> Disciplinas { get; set; } = new List<DisciplinaEntity>();
    public virtual ICollection<HorarioEntity> Horarios { get; set; } = new List<HorarioEntity>();
    public virtual ICollection<TurmaAlunoEntity> TurmaAlunos { get; set; } = new List<TurmaAlunoEntity>();
}