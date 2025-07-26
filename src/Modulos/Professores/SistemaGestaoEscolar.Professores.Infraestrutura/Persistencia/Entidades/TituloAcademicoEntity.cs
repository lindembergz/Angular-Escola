namespace SistemaGestaoEscolar.Professores.Infraestrutura.Persistencia.Entidades;

public class TituloAcademicoEntity
{
    public Guid Id { get; set; }
    public Guid ProfessorId { get; set; }
    public int Tipo { get; set; } // TipoTitulo enum
    public string Curso { get; set; } = string.Empty;
    public string Instituicao { get; set; } = string.Empty;
    public int AnoFormatura { get; set; }
    public DateTime DataCadastro { get; set; }

    // Navigation property
    public ProfessorEntity Professor { get; set; } = null!;
}