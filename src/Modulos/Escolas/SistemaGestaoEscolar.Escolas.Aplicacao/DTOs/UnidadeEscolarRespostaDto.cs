namespace SistemaGestaoEscolar.Escolas.Aplicacao.DTOs;

public class UnidadeEscolarRespostaDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public EnderecoDto Endereco { get; set; } = new();
    public string Tipo { get; set; } = string.Empty;
    public int CapacidadeMaximaAlunos { get; set; }
    public int AlunosMatriculados { get; set; }
    public bool Ativa { get; set; }
    public DateTime DataCriacao { get; set; }
    public Guid EscolaId { get; set; }
    public string NomeEscola { get; set; } = string.Empty;
    
    // Propriedades específicas para ensino infantil
    public int? IdadeMinima { get; set; }
    public int? IdadeMaxima { get; set; }
    public bool TemBerçario { get; set; }
    public bool TemPreEscola { get; set; }
    
    // Séries atendidas
    public List<string> SeriesAtendidas { get; set; } = new();
    
    // Propriedades calculadas
    public int VagasDisponiveis { get; set; }
    public double PercentualOcupacao { get; set; }
    public bool PodeReceberNovasMatriculas { get; set; }
}