using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestaoEscolar.Alunos.Infraestrutura.Persistencia.Entidades;

[Table("Alunos")]
public class AlunoEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(11)]
    public string Cpf { get; set; } = string.Empty;
    
    [Required]
    public DateTime DataNascimento { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Logradouro { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Numero { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Complemento { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Bairro { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Cidade { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(2)]
    public string Estado { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(8)]
    public string Cep { get; set; } = string.Empty;
    
    [Required]
    public int Genero { get; set; } = 0; // TipoGenero (padrão: NaoInformado)
    
    public int? TipoDeficiencia { get; set; } // TipoDeficiencia (opcional)
    
    [MaxLength(500)]
    public string? DescricaoDeficiencia { get; set; } // Descrição das adaptações necessárias
    
    [MaxLength(20)]
    public string? Telefone { get; set; }
    
    [MaxLength(200)]
    public string? Email { get; set; }
    
    [MaxLength(1000)]
    public string? Observacoes { get; set; }
    
    [Required]
    public Guid EscolaId { get; set; }
    
    [Required]
    public DateTime DataCadastro { get; set; }
    
    [Required]
    public bool Ativo { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    // Relacionamentos
    public virtual ICollection<ResponsavelEntity> Responsaveis { get; set; } = new List<ResponsavelEntity>();
    public virtual ICollection<MatriculaEntity> Matriculas { get; set; } = new List<MatriculaEntity>();
}