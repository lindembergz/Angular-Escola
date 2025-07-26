using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

[Table("disciplinas")]
public class DisciplinaEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Codigo { get; set; } = string.Empty;
    
    [Required]
    public int CargaHoraria { get; set; }
    
    [Required]
    public int TipoSerie { get; set; }
    
    [Required]
    public int AnoSerie { get; set; }
    
    [Required]
    public bool Obrigatoria { get; set; }
    
    [MaxLength(500)]
    public string? Descricao { get; set; }
    
    [Required]
    public Guid EscolaId { get; set; }
    
    [Required]
    public bool Ativa { get; set; } = true;
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<TurmaEntity> Turmas { get; set; } = new List<TurmaEntity>();
    public virtual ICollection<HorarioEntity> Horarios { get; set; } = new List<HorarioEntity>();
    public virtual ICollection<DisciplinaPreRequisitoEntity> PreRequisitos { get; set; } = new List<DisciplinaPreRequisitoEntity>();
    public virtual ICollection<DisciplinaPreRequisitoEntity> DisciplinasQueRequerem { get; set; } = new List<DisciplinaPreRequisitoEntity>();
}