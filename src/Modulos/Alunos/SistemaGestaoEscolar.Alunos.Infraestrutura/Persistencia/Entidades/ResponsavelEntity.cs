using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestaoEscolar.Alunos.Infraestrutura.Persistencia.Entidades;

[Table("Responsaveis")]
public class ResponsavelEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Guid AlunoId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(11)]
    public string Cpf { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Telefone { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Email { get; set; }
    
    [Required]
    public int Tipo { get; set; } // TipoResponsavel enum
    
    [MaxLength(100)]
    public string? Profissao { get; set; }
    
    [MaxLength(200)]
    public string? LocalTrabalho { get; set; }
    
    [MaxLength(20)]
    public string? TelefoneTrabalho { get; set; }
    
    [Required]
    public bool ResponsavelFinanceiro { get; set; }
    
    [Required]
    public bool ResponsavelAcademico { get; set; }
    
    [Required]
    public bool AutorizadoBuscar { get; set; }
    
    [MaxLength(1000)]
    public string? Observacoes { get; set; }
    
    // Endereço do responsável (opcional)
    [MaxLength(200)]
    public string? Logradouro { get; set; }
    
    [MaxLength(20)]
    public string? Numero { get; set; }
    
    [MaxLength(100)]
    public string? Complemento { get; set; }
    
    [MaxLength(100)]
    public string? Bairro { get; set; }
    
    [MaxLength(100)]
    public string? Cidade { get; set; }
    
    [MaxLength(2)]
    public string? Estado { get; set; }
    
    [MaxLength(8)]
    public string? Cep { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    // Relacionamento
    [ForeignKey("AlunoId")]
    public virtual AlunoEntity Aluno { get; set; } = null!;
}