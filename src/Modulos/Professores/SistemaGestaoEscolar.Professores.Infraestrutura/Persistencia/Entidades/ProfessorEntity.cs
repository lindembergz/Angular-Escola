namespace SistemaGestaoEscolar.Professores.Infraestrutura.Persistencia.Entidades;

public class ProfessorEntity
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Registro { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public DateTime DataNascimento { get; set; }
    public DateTime DataContratacao { get; set; }
    public Guid EscolaId { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Observacoes { get; set; }

    // Navigation properties
    public List<TituloAcademicoEntity> Titulos { get; set; } = new();
    public List<ProfessorDisciplinaEntity> Disciplinas { get; set; } = new();
}